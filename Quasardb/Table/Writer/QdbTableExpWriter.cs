using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using Quasardb.Exceptions;
using Quasardb.Native;

using qdb_size_t = System.UIntPtr;

namespace Quasardb.TimeSeries.ExpWriter
{
    /// <summary>
    /// Push options
    /// </summary>
    public sealed class QdbTableExpWriterOptions
    {
        private qdb_exp_batch_push_mode _mode = qdb_exp_batch_push_mode.transactional;
        private qdb_exp_batch_push_options _option = qdb_exp_batch_push_options.standard;
        private QdbTimeInterval _interval = QdbTimeInterval.Nothing;

        /// <summary>
        /// Provides information about the push.
        /// </summary>
        public QdbTableExpWriterOptions()
        {
        }

        /// <summary>
        /// Sets the push mode to transactional.
        /// </summary>
        public QdbTableExpWriterOptions Transactional()
        {
            _mode = qdb_exp_batch_push_mode.transactional;
            return this;
        }

        /// <summary>
        /// Sets the push mode to fast.
        /// </summary>
        public QdbTableExpWriterOptions Fast()
        {
            _mode = qdb_exp_batch_push_mode.fast;
            return this;
        }

        /// <summary>
        /// Sets the push mode to asynchronous.
        /// </summary>
        public QdbTableExpWriterOptions Async()
        {
            _mode = qdb_exp_batch_push_mode.async;
            return this;
        }

        /// <summary>
        /// Sets the push mode to truncate, will truncate the range provided then insert data.
        /// </summary>
        /// <param name="interval">The interval truncated during the push.</param>
        public QdbTableExpWriterOptions Truncate(QdbTimeInterval interval)
        {
            _mode = qdb_exp_batch_push_mode.truncate;
            _interval = interval;
            return this;
        }

        /// <summary>
        /// Removes duplicate found while inserting.
        /// </summary>
        public QdbTableExpWriterOptions RemoveDuplicate()
        {
            _option = qdb_exp_batch_push_options.unique;
            return this;
        }

        internal qdb_exp_batch_push_mode Mode()
        {
            return _mode;
        }

        internal qdb_exp_batch_push_options Option()
        {
            return _option;
        }

        internal QdbTimeInterval Interval()
        {
            return _interval;
        }
    }

    /// <summary>
    /// A batch table for bulk insertion into tables.
    /// </summary>
    public unsafe sealed class QdbTableExpWriter : SafeHandle
    {
        private readonly qdb_handle _handle;
        private List<GCHandle> _pins;

        string _table;
        QdbTableExpWriterOptions _options;

        private qdb_timespec[] _timestamps;
        private qdb_ts_column_info_ex[] _columns;
        private qdb_exp_batch_push_column_data[] _data;
        private qdb_exp_batch_push_table_schema* _schemas = null;
        private Dictionary<string, long> _column_name_to_index;


        internal QdbTableExpWriter(qdb_handle handle, string table, QdbTableExpWriterOptions options) : base(IntPtr.Zero, true)
        {
            _handle = handle;
            _pins = new List<GCHandle>(1024);
            _table = table;
            _options = options;
            _column_name_to_index = new Dictionary<string, long>();

            using (var columns = new qdb_buffer<qdb_ts_column_info_ex>(handle))
            {
                var err = qdb_api.qdb_ts_list_columns_ex(handle, _table, out columns.Pointer, out columns.Size);
                QdbExceptionThrower.ThrowIfNeeded(err, alias: table);

                long index = 0;
                _columns = new qdb_ts_column_info_ex[(int)columns.Size];
                _data = new qdb_exp_batch_push_column_data[(int)columns.Size];
                foreach (var column in columns)
                {
                    _columns[index] = column;
                    _column_name_to_index[column.name] = index;
                    _data[index].blobs = null;
                    index++;
                }
            }
        }

        private void Free()
        {
            foreach (var pin in _pins)
                pin.Free();
        }

        /// <inheritdoc />
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle()
        {
            Free();
            return true;
        }

        /// <inheritdoc />
        public override bool IsInvalid
        {
            get { return _handle == null || _handle.IsInvalid; }
        }

        internal long IndexOf(string column)
        {
            try
            {
                return _column_name_to_index[column];
            }
            catch (KeyNotFoundException /*e*/)
            {
                throw new QdbException(String.Format("Column '{0}' not found in '{1}'.", column, _table));
            }
        }

        /// <summary>
        /// Set all timestamps.
        /// </summary>
        /// <param name="timestamps">The timestamps </param>
        public unsafe void SetTimestamps(DateTime[] timestamps)
        {
            _timestamps = new qdb_timespec[timestamps.Length];
            long index = 0;
            foreach (var timestamp in timestamps)
            {
                _timestamps[index] = TimeConverter.ToTimespec(timestamp);
                index++;
            }
        }

        /// <summary>
        /// Set a blob column.
        /// </summary>
        /// <param name="index">The index to the column you want to modify</param>
        /// <param name="values">The values as an array of byte arrays</param>
        public unsafe void SetBlobColumn(long index, byte[][] values)
        {
            qdb_blob[] blobs = new qdb_blob[values.Length];
            long idx = 0;
            foreach (byte[] content in values)
            {
                GCHandle pin = GCHandle.Alloc(content, GCHandleType.Pinned);
                _pins.Add(pin);
                blobs[idx].content = (byte*)pin.AddrOfPinnedObject();
                blobs[idx].content_size = (qdb_size_t)content.Length;
                idx++;
            }
            _data[index].blobs = (qdb_blob*)ExpWriterHelper.convert_array<qdb_blob>(blobs, ref _pins);

        }

        /// <summary>
        /// Set a blob column.
        /// </summary>
        /// <param name="name">The name to the column you want to modify</param>
        /// <param name="values">The values as an array of byte arrays</param>
        public unsafe void SetBlobColumn(string name, byte[][] values)
        {
            SetBlobColumn(IndexOf(name), values);
        }

        /// <summary>
        /// Set a double column.
        /// </summary>
        /// <param name="index">The index to the column you want to modify</param>
        /// <param name="values">The values as a double array</param>
        public void SetDoubleColumn(long index, double[] values)
        {
            _data[index].doubles = (double*)ExpWriterHelper.convert_array<double>(values, ref _pins);
        }

        /// <summary>
        /// Set a double column.
        /// </summary>
        /// <param name="name">The name to the column you want to modify</param>
        /// <param name="values">The values as a double array</param>
        public unsafe void SetDoubleColumn(string name, double[] values)
        {
            SetDoubleColumn(IndexOf(name), values);
        }

        /// <summary>
        /// Set an integer column.
        /// </summary>
        /// <param name="index">The index to the column you want to modify</param>
        /// <param name="values">The values as an int64 array</param>
        public void SetInt64Column(long index, long[] values)
        {
            _data[index].ints = (long*)ExpWriterHelper.convert_array<long>(values, ref _pins);
        }

        /// <summary>
        /// Set an integer column.
        /// </summary>
        /// <param name="name">The name to the column you want to modify</param>
        /// <param name="values">The values as an int64 array</param>
        public unsafe void SetInt64Column(string name, long[] values)
        {
            SetInt64Column(IndexOf(name), values);
        }

        /// <summary>
        /// Set a string column.
        /// </summary>
        /// <param name="index">The index to the column you want to modify</param>
        /// <param name="values">The values as a utf8 string array</param>
        public unsafe void SetStringColumn(long index, string[] values)
        {
            qdb_sized_string[] strings = new qdb_sized_string[values.Length];
            long idx = 0;
            foreach (string content in values)
            {
                byte[] str = System.Text.Encoding.UTF8.GetBytes(content);
                GCHandle pin = GCHandle.Alloc(str, GCHandleType.Pinned);
                _pins.Add(pin);
                strings[idx].data = (byte*)pin.AddrOfPinnedObject();
                strings[idx].length = (qdb_size_t)str.Length;
                idx++;
            }
            _data[index].strings = (qdb_sized_string*)ExpWriterHelper.convert_array<qdb_sized_string>(strings, ref _pins);
        }

        /// <summary>
        /// Set a string column.
        /// </summary>
        /// <param name="name">The name to the column you want to modify</param>
        /// <param name="values">The values as a utf8 string array</param>
        public unsafe void SetStringColumn(string name, string[] values)
        {
            SetStringColumn(IndexOf(name), values);
        }

        /// <summary>
        /// Set a timestamp column.
        /// </summary>
        /// <param name="index">The index to the column you want to modify</param>
        /// <param name="values">The values as a DateTime array</param>
        public unsafe void SetTimestampColumn(long index, DateTime[] values)
        {
            qdb_timespec[] timestamps = new qdb_timespec[values.Length];
            long idx = 0;
            foreach (var timestamp in values)
            {
                timestamps[idx] = TimeConverter.ToTimespec(timestamp);
                idx++;
            }
            _data[index].timestamps = (qdb_timespec*)ExpWriterHelper.convert_array<qdb_timespec>(timestamps, ref _pins);
        }

        /// <summary>
        /// Set a timestamp column.
        /// </summary>
        /// <param name="name">The name to the column you want to modify</param>
        /// <param name="values">The values as a DateTime array</param>
        public unsafe void SetTimestampColumn(string name, DateTime[] values)
        {
            SetTimestampColumn(IndexOf(name), values);
        }

        /// <summary>
        /// Regular batch push.
        /// </summary>
        public void Push()
        {
            var tables = new qdb_exp_batch_push_table[1];
            tables[0] = ExpWriterHelper.convert_table(_table, _options, _timestamps, _columns, _data, ref _pins);
            var err = qdb_api.qdb_exp_batch_push(_handle, _options.Mode(), tables, null, 1);
            Free();
            QdbExceptionThrower.ThrowIfNeeded(err);
        }
    }

    unsafe class ExpWriterHelper
    {
        private static qdb_sized_string convert_string(string str, ref List<GCHandle> pins)
        {
            GCHandle pin;
            var ss = new qdb_sized_string(str, ref pin);
            pins.Add(pin);
            return ss;
        }
        
        internal unsafe static IntPtr convert_array<T>(T[] array, ref List<GCHandle> pins)
        {
            GCHandle pin = GCHandle.Alloc(array, GCHandleType.Pinned);
            pins.Add(pin);
            return pin.AddrOfPinnedObject();
        }

        static qdb_exp_batch_push_column convert_column(qdb_ts_column_info_ex info, qdb_exp_batch_push_column_data data, ref List<GCHandle> pins)
        {
            var column = new qdb_exp_batch_push_column();
            column.name = convert_string(info.name, ref pins);
            column.data_type = (info.type == qdb_ts_column_type.qdb_ts_column_symbol ? qdb_ts_column_type.qdb_ts_column_string : info.type);
            column.data = data;
            return column;
        }

        static qdb_exp_batch_push_column[] convert_columns(qdb_ts_column_info_ex[] infos, qdb_exp_batch_push_column_data[] data, ref qdb_size_t columnCount, ref List<GCHandle> pins)
        {
            var columns = new List<qdb_exp_batch_push_column>();
            for (int index = 0; index < infos.Length; index++)
            {
                if (data[index].blobs != null)
                {
                    columns.Add(convert_column(infos[index], data[index], ref pins));
                }
            }
            columnCount = (qdb_size_t)columns.Count;
            return columns.ToArray();
        }

        static qdb_exp_batch_push_table_data convert_data(qdb_ts_column_info_ex[] infos, qdb_timespec[] timestamps, qdb_exp_batch_push_column_data[] data, ref List<GCHandle> pins)
        {
            qdb_exp_batch_push_table_data d;

            d.row_count = (qdb_size_t)timestamps.Length;
            d.column_count = (qdb_size_t)infos.Length;
            d.timestamps = (qdb_timespec*)convert_array(timestamps, ref pins);
            d.columns = (qdb_exp_batch_push_column*)convert_array(convert_columns(infos, data, ref d.column_count, ref pins), ref pins);
            return d;
        }

        internal static qdb_exp_batch_push_table convert_table(string name, QdbTableExpWriterOptions options, qdb_timespec[] timestamps, qdb_ts_column_info_ex[] infos, qdb_exp_batch_push_column_data[] data, ref List<GCHandle> pins)
        {
            qdb_exp_batch_push_table table;
            table.name = convert_string(name, ref pins);
            table.data = convert_data(infos, timestamps, data, ref pins);

            if (options.Mode() == qdb_exp_batch_push_mode.truncate)
            {
                qdb_ts_range[] ranges = new qdb_ts_range[1];
                ranges[0] = options.Interval().ToNative();
                table.truncate_ranges = (qdb_ts_range*)convert_array(ranges, ref pins);
                table.truncate_range_count = (qdb_size_t)1;
            }
            else
            {
                table.truncate_ranges = null;
                table.truncate_range_count = (qdb_size_t)0;
            }
            table.options = options.Option();
            return table;
        }
    }
}
