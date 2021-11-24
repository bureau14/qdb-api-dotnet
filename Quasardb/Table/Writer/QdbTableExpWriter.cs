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

    [StructLayout(LayoutKind.Explicit)]
    internal unsafe sealed class QdbColumnData
    {
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal List<qdb_timespec> timestamps;
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal List<qdb_sized_string> strings;
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal List<qdb_blob> blobs;
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal List<long> ints;
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal List<double> doubles;
    }

    internal unsafe sealed class QdbTableExpWriterData
    {
        public qdb_ts_column_info_ex[] columns;
        public QdbColumnData[] data;
        public Dictionary<string, long> column_name_to_index;
    }


    /// <summary>
    /// A batch table for bulk insertion into tables.
    /// </summary>
    public unsafe sealed class QdbTableExpWriter : SafeHandle
    {
        private readonly qdb_handle _handle;
        private List<GCHandle> _pins;

        string[] _tables;
        QdbTableExpWriterOptions _options;

        private qdb_timespec[] _timestamps;
        private QdbTableExpWriterData[] _data;
        private qdb_exp_batch_push_table_schema* _schemas = null;
        private Dictionary<string, long> _table_name_to_index;

        internal QdbTableExpWriter(qdb_handle handle, string[] tables, QdbTableExpWriterOptions options) : base(IntPtr.Zero, true)
        {
            _handle = handle;
            _pins = new List<GCHandle>(1024);
            _tables = tables;
            _options = options;
            _data = new QdbTableExpWriterData[tables.Length];
            _table_name_to_index = new Dictionary<string, long>();


            _table_name_to_index = new Dictionary<string, long>();
            long table_index = 0;
            foreach (var table in tables)
            {
                _table_name_to_index[table] = table_index;
                using (var columns = new qdb_buffer<qdb_ts_column_info_ex>(handle))
                {
                    var err = qdb_api.qdb_ts_list_columns_ex(handle, table, out columns.Pointer, out columns.Size);
                    QdbExceptionThrower.ThrowIfNeeded(err, alias: table);

                    long column_index = 0;
                    _data[table_index] = new QdbTableExpWriterData();
                    _data[table_index].data = new QdbColumnData[(int)columns.Size];
                    _data[table_index].columns = new qdb_ts_column_info_ex[(int)columns.Size];
                    _data[table_index].column_name_to_index = new Dictionary<string, long>();
                    foreach (var column in columns)
                    {
                        _data[table_index].columns[column_index] = column;
                        _data[table_index].column_name_to_index[column.name] = column_index;
                        _data[table_index].data[column_index] = new QdbColumnData();
                        _data[table_index].data[column_index].blobs = null;
                        column_index++;
                    }
                }
                table_index++;
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

        internal long IndexOfTable(string table)
        {
            try
            {
                return _table_name_to_index[table];
            }
            catch (KeyNotFoundException /*e*/)
            {
                throw new QdbException(String.Format("Table '{0}' not found.", table));
            }
        }

        internal qdb_ts_column_type ObjectTypeToColumnType(object obj)
        {
            Type objType = obj.GetType();

            if (objType.Equals(typeof(byte[])))
            {
                return qdb_ts_column_type.qdb_ts_column_blob;
            }
            else if (objType.Equals(typeof(string)))
            {
                return qdb_ts_column_type.qdb_ts_column_string;
            }
            else if (objType.Equals(typeof(double)))
            {
                return qdb_ts_column_type.qdb_ts_column_double;
            }
            else if (objType.Equals(typeof(long)))
            {
                return qdb_ts_column_type.qdb_ts_column_int64;
            }
            else if (objType.Equals(typeof(DateTime)))
            {
                return qdb_ts_column_type.qdb_ts_column_timestamp;
            }
            return qdb_ts_column_type.qdb_ts_column_uninitialized;
        }

        internal void CheckType(long table_index, long column_index, qdb_ts_column_type type)
        {
            var column_type = _data[table_index].columns[column_index].type;
            if (type == qdb_ts_column_string && (column_type != qdb_ts_column_string || column_type != qdb_ts_column_symbol))
            if (column_type != type)
            {
                if (!(type == qdb_ts_column_type.qdb_ts_column_string && column_type == qdb_ts_column_type.qdb_ts_column_symbol)))
                {
                    throw new QdbException(String.Format("Invalid type for column {0} of table {1}", _tables[table_index], _data[table_index].columns[column_index].name.ToString()));
                }
            }
        }

        internal long IndexOfColumn(long table_index, string column)
        {
            try
            {
                return _data[table_index].column_name_to_index[column];
            }
            catch (KeyNotFoundException /*e*/)
            {
                throw new QdbException(String.Format("Column '{0}' not found in '{1}'.", column, _tables[table_index]));
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
        /// <param name="table_index">The index to the table you want to modify</param>
        /// <param name="column_index">The index to the column you want to modify</param>
        /// <param name="values">The values as an array of byte arrays</param>
        public unsafe void SetBlobColumn(long table_index, long column_index, byte[][] values)
        {
            CheckType(table_index, column_index, qdb_ts_column_type.qdb_ts_column_blob);
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
            _data[table_index].data[column_index].blobs = new List<qdb_blob>(blobs);

        }

        /// <summary>
        /// Set a blob column.
        /// </summary>
        /// <param name="table_name">The name to the table you want to modify</param>
        /// <param name="column_name">The name to the column you want to modify</param>
        /// <param name="values">The values as an array of byte arrays</param>
        public unsafe void SetBlobColumn(string table_name, string column_name, byte[][] values)
        {
            long table_index = IndexOfTable(table_name);
            long column_index = IndexOfColumn(table_index, column_name);
            SetBlobColumn(table_index, column_index, values);
        }

        /// <summary>
        /// Set a double column.
        /// </summary>
        /// <param name="table_index">The index to the table you want to modify</param>
        /// <param name="column_index">The index to the column you want to modify</param>
        /// <param name="values">The values as a double array</param>
        public void SetDoubleColumn(long table_index, long column_index, double[] values)
        {
            CheckType(table_index, column_index, qdb_ts_column_type.qdb_ts_column_double);
            _data[table_index].data[column_index].doubles = new List<double>(values);
        }

        /// <summary>
        /// Set a double column.
        /// </summary>
        /// <param name="table_name">The name to the table you want to modify</param>
        /// <param name="column_name">The name to the column you want to modify</param>
        /// <param name="values">The values as a double array</param>
        public unsafe void SetDoubleColumn(string table_name, string column_name, double[] values)
        {
            long table_index = IndexOfTable(table_name);
            long column_index = IndexOfColumn(table_index, column_name);
            SetDoubleColumn(table_index, column_index, values);
        }

        /// <summary>
        /// Set an integer column.
        /// </summary>
        /// <param name="table_index">The index to the table you want to modify</param>
        /// <param name="column_index">The index to the column you want to modify</param>
        /// <param name="values">The values as an int64 array</param>
        public void SetInt64Column(long table_index, long column_index, long[] values)
        {
            CheckType(table_index, column_index, qdb_ts_column_type.qdb_ts_column_int64);
            _data[table_index].data[column_index].ints = new List<long>(values);
        }

        /// <summary>
        /// Set an integer column.
        /// </summary>
        /// <param name="table_name">The name to the table you want to modify</param>
        /// <param name="column_name">The name to the column you want to modify</param>
        /// <param name="values">The values as an int64 array</param>
        public unsafe void SetInt64Column(string table_name, string column_name, long[] values)
        {
            long table_index = IndexOfTable(table_name);
            long column_index = IndexOfColumn(table_index, column_name);
            SetInt64Column(table_index, column_index, values);
        }

        /// <summary>
        /// Set a string column.
        /// </summary>
        /// <param name="table_index">The index to the table you want to modify</param>
        /// <param name="column_index">The index to the column you want to modify</param>
        /// <param name="values">The values as a utf8 string array</param>
        public unsafe void SetStringColumn(long table_index, long column_index, string[] values)
        {
            CheckType(table_index, column_index, qdb_ts_column_type.qdb_ts_column_string);
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
            _data[table_index].data[column_index].strings = new List<qdb_sized_string>(strings);
        }

        /// <summary>
        /// Set a string column.
        /// </summary>
        /// <param name="table_name">The name to the table you want to modify</param>
        /// <param name="column_name">The name to the column you want to modify</param>
        /// <param name="values">The values as a utf8 string array</param>
        public unsafe void SetStringColumn(string table_name, string column_name, string[] values)
        {
            long table_index = IndexOfTable(table_name);
            long column_index = IndexOfColumn(table_index, column_name);
            SetStringColumn(table_index, column_index, values);
        }

        /// <summary>
        /// Set a timestamp column.
        /// </summary>
        /// <param name="table_index">The index to the table you want to modify</param>
        /// <param name="column_index">The index to the column you want to modify</param>
        /// <param name="values">The values as a DateTime array</param>
        public unsafe void SetTimestampColumn(long table_index, long column_index, DateTime[] values)
        {
            CheckType(table_index, column_index, qdb_ts_column_type.qdb_ts_column_timestamp);
            qdb_timespec[] timestamps = new qdb_timespec[values.Length];
            long idx = 0;
            foreach (var timestamp in values)
            {
                timestamps[idx] = TimeConverter.ToTimespec(timestamp);
                idx++;
            }
            _data[table_index].data[column_index].timestamps = new List<qdb_timespec>(timestamps);
        }

        /// <summary>
        /// Set a timestamp column.
        /// </summary>
        /// <param name="table_name">The name to the table you want to modify</param>
        /// <param name="column_name">The name to the column you want to modify</param>
        /// <param name="values">The values as a DateTime array</param>
        public unsafe void SetTimestampColumn(string table_name, string column_name, DateTime[] values)
        {
            long table_index = IndexOfTable(table_name);
            long column_index = IndexOfColumn(table_index, column_name);
            SetTimestampColumn(table_index, column_index, values);
        }

        /// <summary>
        /// Regular batch push.
        /// </summary>
        public void Push()
        {
            var tables = new qdb_exp_batch_push_table[_tables.Length];
            long index = 0;
            foreach (var table in _tables)
            {
                tables[index] = ExpWriterHelper.convert_table(table, _options, _timestamps, _data[index].columns, _data[index].data, ref _pins);
                index++;
            }
            var err = qdb_api.qdb_exp_batch_push(_handle, _options.Mode(), tables, null, _tables.Length);
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

        static qdb_exp_batch_push_column convert_column(qdb_ts_column_info_ex info, QdbColumnData data, ref List<GCHandle> pins)
        {
            var column = new qdb_exp_batch_push_column();
            column.name = convert_string(info.name, ref pins);
            column.data_type = (info.type == qdb_ts_column_type.qdb_ts_column_symbol ? qdb_ts_column_type.qdb_ts_column_string : info.type);
            column.data = new qdb_exp_batch_push_column_data();
            switch (info.type)
            {
                case qdb_ts_column_type.qdb_ts_column_double:
                    column.data.doubles = (double*)convert_array<double>(data.doubles.ToArray(), ref pins);
                    break;
                case qdb_ts_column_type.qdb_ts_column_blob:
                    column.data.blobs = (qdb_blob*)convert_array<qdb_blob>(data.blobs.ToArray(), ref pins);
                    break;
                case qdb_ts_column_type.qdb_ts_column_int64:
                    column.data.ints = (long*)convert_array<long>(data.ints.ToArray(), ref pins);
                    break;
                case qdb_ts_column_type.qdb_ts_column_timestamp:
                    column.data.timestamps = (qdb_timespec*)convert_array<qdb_timespec>(data.timestamps.ToArray(), ref pins);
                    break;
                case qdb_ts_column_type.qdb_ts_column_string:
                case qdb_ts_column_type.qdb_ts_column_symbol:
                    column.data.strings = (qdb_sized_string*)convert_array<qdb_sized_string>(data.strings.ToArray(), ref pins);
                    break;
            }
            return column;
        }

        static qdb_exp_batch_push_column[] convert_columns(qdb_ts_column_info_ex[] infos, QdbColumnData[] data, ref qdb_size_t columnCount, ref List<GCHandle> pins)
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

        static qdb_exp_batch_push_table_data convert_data(qdb_ts_column_info_ex[] infos, qdb_timespec[] timestamps, QdbColumnData[] data, ref List<GCHandle> pins)
        {
            qdb_exp_batch_push_table_data d;

            d.row_count = (qdb_size_t)timestamps.Length;
            d.column_count = (qdb_size_t)infos.Length;
            d.timestamps = (qdb_timespec*)convert_array(timestamps, ref pins);
            d.columns = (qdb_exp_batch_push_column*)convert_array(convert_columns(infos, data, ref d.column_count, ref pins), ref pins);
            return d;
        }

        internal static qdb_exp_batch_push_table convert_table(string name, QdbTableExpWriterOptions options, qdb_timespec[] timestamps, qdb_ts_column_info_ex[] infos, QdbColumnData[] data, ref List<GCHandle> pins)
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
