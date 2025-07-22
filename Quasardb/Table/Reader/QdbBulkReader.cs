using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using Quasardb.Exceptions;
using Quasardb.Native;

using qdb_char_ptr_ptr = System.IntPtr;
using qdb_size_t = System.UIntPtr;
using qdb_bulk_reader_table_data = Quasardb.Native.qdb_exp_batch_push_table_data;

namespace Quasardb.TimeSeries.Reader
{
    /// <summary>
    /// Represents a QuasarDB bulk reader input specification for a single table.
    /// </summary>
    public struct QdbBulkReaderTable
    {
        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the time ranges to query.
        /// </summary>
        public QdbTimeInterval[] Ranges { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QdbBulkReaderTable"/> struct.
        /// </summary>
        /// <param name="name">Table name.</param>
        /// <param name="ranges">Time intervals to read from the table.</param>
        public QdbBulkReaderTable(string name, IEnumerable<QdbTimeInterval> ranges)
        {
            Name = name;
            Ranges = ranges?.ToArray() ?? Array.Empty<QdbTimeInterval>();
        }
    }

    /// <summary>
    /// Represents the result of a bulk read operation.
    /// </summary>
    public unsafe sealed class QdbBulkReaderResult : SafeHandle
    {
        readonly qdb_handle _handle;
        readonly qdb_bulk_reader_table_data* _data;

        /// <summary>
        /// Initializes a new instance of the <see cref="QdbBulkReaderResult"/> class.
        /// </summary>
        /// <param name="handle">Native QuasarDB handle.</param>
        /// <param name="data">Pointer to native bulk table data.</param>
        internal QdbBulkReaderResult(qdb_handle handle, qdb_bulk_reader_table_data* data) : base(IntPtr.Zero, true)
        {
            _handle = handle;
            _data = data;
        }

        /// <inheritdoc />
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle()
        {
            if (!_handle.IsClosed)
            {
                qdb_api.qdb_release(_handle, (IntPtr)_data);
            }
            return true;
        }

        /// <inheritdoc />
        public override bool IsInvalid => _handle == null || _handle.IsInvalid;

        /// <summary>
        /// Gets the raw native pointer to the data.
        /// </summary>
        internal unsafe qdb_bulk_reader_table_data* Data => _data;

        /// <summary>
        /// Gets the native pointer as <see cref="IntPtr"/>.
        /// </summary>
        internal IntPtr DataPtr => (IntPtr)_data;

        /// <summary>
        /// Gets the number of rows in the result.
        /// </summary>
        internal long RowCount => (long)Data->row_count;
    }

    /// <summary>
    /// Reads time series data from QuasarDB in bulk.
    /// </summary>
    public sealed class QdbBulkReader : SafeHandle, IEnumerable<QdbBulkRow>
    {
        readonly qdb_handle _handle;
        readonly IntPtr _reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="QdbBulkReader"/> class.
        /// </summary>
        /// <param name="handle">Native QuasarDB handle.</param>
        /// <param name="columns">Column names to query.</param>
        /// <param name="tables">Tables and time ranges to query.</param>
        internal QdbBulkReader(qdb_handle handle, string[] columns, QdbBulkReaderTable[] tables) : base(IntPtr.Zero, true)
        {
            _handle = handle;
            var pins = new List<GCHandle>();
            try
            {
                unsafe
                {
                    qdb_bulk_reader_table* tblPtr = null;
                    qdb_size_t tblCount = (qdb_size_t)0;
                    if (tables != null)
                    {
                        tblCount = (qdb_size_t)tables.Length;
                        var nativeTables = new qdb_bulk_reader_table[tables.Length];
                        for (int i = 0; i < tables.Length; i++)
                        {
                            nativeTables[i].name = BulkReaderHelper.ConvertCharArray(tables[i].Name, ref pins);
                            var ranges = new qdb_ts_range[tables[i].Ranges.Length];
                            for (int j = 0; j < ranges.Length; j++)
                                ranges[j] = tables[i].Ranges[j].ToNative();
                            nativeTables[i].ranges = (qdb_ts_range*)BulkReaderHelper.ConvertArray(ranges, ref pins);
                            nativeTables[i].range_count = (qdb_size_t)ranges.Length;
                        }
                        tblPtr = (qdb_bulk_reader_table*)BulkReaderHelper.ConvertArray(nativeTables, ref pins);
                    }

                    qdb_char_ptr_ptr colPtr = IntPtr.Zero;
                    qdb_size_t colCount = (qdb_size_t)0;
                    if (columns != null)
                    {
                        colCount = (qdb_size_t)columns.Length;
                        IntPtr[] arr = new IntPtr[columns.Length];
                        for (int i = 0; i < columns.Length; i++)
                            arr[i] = BulkReaderHelper.ConvertCharArray(columns[i], ref pins);
                        colPtr = BulkReaderHelper.ConvertArray(arr, ref pins);
                    }

                    var err = qdb_api.qdb_bulk_reader_fetch(_handle, colPtr, colCount, tblPtr, tblCount, out _reader);
                    QdbExceptionThrower.ThrowIfNeededWithMsg(_handle, err);
                }
            }
            finally
            {
                foreach (var p in pins) p.Free();
            }
        }

        /// <inheritdoc />
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle()
        {
            if (!_handle.IsClosed)
            {
                qdb_api.qdb_release(_handle, _reader);
            }
            return true;
        }

        /// <inheritdoc />
        public override bool IsInvalid => _handle == null || _handle.IsInvalid;

        /// <summary>
        /// Retrieves the next chunk of rows from the reader.
        /// </summary>
        /// <param name="rowsToGet">Number of rows to retrieve. Zero means all available.</param>
        /// <returns>The next <see cref="QdbBulkReaderResult"/> or <c>null</c> if the end is reached.</returns>
        public QdbBulkReaderResult GetData(long rowsToGet = 0)
        {
            unsafe
            {
                qdb_bulk_reader_table_data* data;
                var err = qdb_api.qdb_bulk_reader_get_data(_reader, out data, (qdb_size_t)rowsToGet);
                if (err == qdb_error.qdb_e_iterator_end)
                    return null;
                QdbExceptionThrower.ThrowIfNeededWithMsg(_handle, err);
                return new QdbBulkReaderResult(_handle, data);
            }
        }

        /// <inheritdoc />
        public IEnumerator<QdbBulkRow> GetEnumerator()
        {
            foreach (var row in GetRowsSafe()) yield return row;
        }

        /// <inheritdoc />
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        private IEnumerable<QdbBulkRow> GetRowsSafe()
        {
            foreach (var result in EnumerateResults())
            {
                using (result)
                {
                    foreach (var row in ExtractRows(result))
                        yield return row;
                }
            }
        }

        private IEnumerable<QdbBulkRow> ExtractRows(QdbBulkReaderResult result)
        {
            var row = new QdbBulkRow(result.DataPtr);
            long count = result.RowCount;
            for (long i = 0; i < count; ++i)
            {
                row.RowIndex = i;
                yield return row;
            }
        }

        private IEnumerable<QdbBulkReaderResult> EnumerateResults()
        {
            QdbBulkReaderResult result = null;
            try
            {
                while ((result = GetData()) != null)
                {
                    yield return result;
                    result = null;
                }
            }
            finally
            {
                result?.Dispose();
            }
        }
    }

    /// <summary>
    /// Internal helper class for pinning and marshalling bulk reader arrays.
    /// </summary>
    internal static class BulkReaderHelper
    {
        /// <summary>
        /// Converts a string to a pinned UTF-8 array and returns pointer to it.
        /// </summary>
        internal static IntPtr ConvertCharArray(string str, ref List<GCHandle> pins)
        {
            var content = System.Text.Encoding.UTF8.GetBytes(str);
            GCHandle pin = GCHandle.Alloc(content, GCHandleType.Pinned);
            pins.Add(pin);
            return pin.AddrOfPinnedObject();
        }

        /// <summary>
        /// Converts a managed array to pinned memory and returns pointer.
        /// </summary>
        internal static IntPtr ConvertArray<T>(T[] array, ref List<GCHandle> pins)
        {
            GCHandle pin = GCHandle.Alloc(array, GCHandleType.Pinned);
            pins.Add(pin);
            return pin.AddrOfPinnedObject();
        }
    }
}
