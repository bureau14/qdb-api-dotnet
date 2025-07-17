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
    public struct QdbBulkReaderTable
    {
        public string Name { get; }
        public QdbTimeInterval[] Ranges { get; }

        public QdbBulkReaderTable(string name, IEnumerable<QdbTimeInterval> ranges)
        {
            Name = name;
            Ranges = ranges?.ToArray() ?? Array.Empty<QdbTimeInterval>();
        }
    }

    public unsafe sealed class QdbBulkReaderResult : SafeHandle
    {
        readonly qdb_handle _handle;
        readonly qdb_bulk_reader_table_data* _data;

        internal QdbBulkReaderResult(qdb_handle handle, qdb_bulk_reader_table_data* data) : base(IntPtr.Zero, true)
        {
            _handle = handle;
            _data = data;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle()
        {
            if (!_handle.IsClosed)
            {
                qdb_api.qdb_release(_handle, (IntPtr)_data);
            }
            return true;
        }

        public override bool IsInvalid => _handle == null || _handle.IsInvalid;

        public qdb_bulk_reader_table_data* Data => _data;
    }

    internal static class BulkReaderHelper
    {
        internal static IntPtr ConvertCharArray(string str, ref List<GCHandle> pins)
        {
            var content = System.Text.Encoding.UTF8.GetBytes(str);
            GCHandle pin = GCHandle.Alloc(content, GCHandleType.Pinned);
            pins.Add(pin);
            return pin.AddrOfPinnedObject();
        }

        internal static IntPtr ConvertArray<T>(T[] array, ref List<GCHandle> pins)
        {
            GCHandle pin = GCHandle.Alloc(array, GCHandleType.Pinned);
            pins.Add(pin);
            return pin.AddrOfPinnedObject();
        }
    }

    public unsafe sealed class QdbBulkReader : SafeHandle
    {
        readonly qdb_handle _handle;
        readonly IntPtr _reader;

        internal QdbBulkReader(qdb_handle handle, string[] columns, QdbBulkReaderTable[] tables) : base(IntPtr.Zero, true)
        {
            _handle = handle;
            var pins = new List<GCHandle>();
            try
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
            finally
            {
                foreach (var p in pins) p.Free();
            }
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle()
        {
            if (!_handle.IsClosed)
            {
                qdb_api.qdb_release(_handle, _reader);
            }
            return true;
        }

        public override bool IsInvalid => _handle == null || _handle.IsInvalid;

        public QdbBulkReaderResult GetData(long rowsToGet = 0)
        {
            qdb_bulk_reader_table_data* data;
            var err = qdb_api.qdb_bulk_reader_get_data(_reader, &data, (qdb_size_t)rowsToGet);
            if (err == qdb_error.qdb_e_iterator_end)
                return null;
            QdbExceptionThrower.ThrowIfNeededWithMsg(_handle, err);
            return new QdbBulkReaderResult(_handle, data);
        }
    }
}
