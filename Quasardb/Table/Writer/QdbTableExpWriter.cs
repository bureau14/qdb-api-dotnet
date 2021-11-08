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
    /// A batch table for bulk insertion into tables.
    /// </summary>
    public unsafe sealed class QdbTableExpWriter : SafeHandle
    {
        private readonly qdb_handle _handle;
        readonly List<GCHandle> _pins;
        private string _table;
        private qdb_timespec[] _timestamps;
        private qdb_ts_column_info_ex[] _columns;
        private qdb_exp_batch_push_column_data[] _data;
        private qdb_exp_batch_push_table_schema* _schemas = null;

        qdb_sized_string convert_string(string str)
        {
            GCHandle pin;
            var ss = new qdb_sized_string(str, ref pin);
            _pins.Add(pin);
            return ss;
        }

        unsafe IntPtr convert_array<T>(T[] array)
        {
            GCHandle pin = GCHandle.Alloc(array, GCHandleType.Pinned);
            _pins.Add(pin);
            return pin.AddrOfPinnedObject();
        }

        qdb_exp_batch_push_column convert_column(qdb_ts_column_info_ex info, qdb_exp_batch_push_column_data data)
        {
            var column = new qdb_exp_batch_push_column();
            column.name = convert_string(info.name);
            column.data_type = (info.type == qdb_ts_column_type.qdb_ts_column_symbol ? qdb_ts_column_type.qdb_ts_column_string : info.type);
            column.data = data;
            return column;
        }

        qdb_exp_batch_push_column[] convert_columns(qdb_ts_column_info_ex[] infos, qdb_exp_batch_push_column_data[] data, ref qdb_size_t columnCount)
        {
            var columns = new List<qdb_exp_batch_push_column>();
            for (int index = 0; index < infos.Length; index++)
            {
                if (data[index].blobs != null)
                {
                    columns.Add(convert_column(infos[index], data[index]));
                }
            }
            columnCount = (qdb_size_t)columns.Count;
            return columns.ToArray();
        }

        qdb_exp_batch_push_table_data convert_data(qdb_ts_column_info_ex[] infos, qdb_exp_batch_push_column_data[] data)
        {
            qdb_exp_batch_push_table_data d;

            d.row_count = (qdb_size_t)_timestamps.Length;
            d.column_count = (qdb_size_t)infos.Length;
            d.timestamps = (qdb_timespec*)convert_array(_timestamps);
            d.columns = (qdb_exp_batch_push_column*)convert_array(convert_columns(infos, data, ref d.column_count));
            return d;
        }

        qdb_exp_batch_push_table convert_table(string name, qdb_timespec[] timestamps, qdb_ts_column_info_ex[] infos, qdb_exp_batch_push_column_data[] data)
        {
            qdb_exp_batch_push_table table;
            table.name = convert_string(name);
            table.data = convert_data(infos, data);
            table.truncate_ranges = null;
            table.truncate_range_count = (qdb_size_t)0;
            return table;
        }

        internal QdbTableExpWriter(qdb_handle handle, string table) : base(IntPtr.Zero, true)
        {
            _handle = handle;
            _pins = new List<GCHandle>(1024);
            _table = table;

            using (var columns = new qdb_buffer<qdb_ts_column_info_ex>(handle))
            {
                var err = qdb_api.qdb_ts_list_columns_ex(handle, table, out columns.Pointer, out columns.Size);
                QdbExceptionThrower.ThrowIfNeeded(err, alias: table);

                long index = 0;
                _columns = new qdb_ts_column_info_ex[(int)columns.Size];
                _data = new qdb_exp_batch_push_column_data[(int)columns.Size];
                foreach (var column in columns)
                {
                    _columns[index] = column;
                    _data[index].blobs = null;
                    index++;
                }
            }
        }

        /// <inheritdoc />
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle()
        {
            foreach (var pin in _pins)
                pin.Free();
            return true;
        }

        /// <inheritdoc />
        public override bool IsInvalid
        {
            get { return _handle == null || _handle.IsInvalid; }
        }

        internal long IndexOf(string column)
        {
            for (int i = 0; i < _columns.Length; ++i)
            {
                if (_columns[i].name == column)
                    return i;
            }
            return -1;
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
            _data[index].blobs = (qdb_blob*)convert_array<qdb_blob>(blobs);

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
            _data[index].doubles = (double*)convert_array<double>(values);
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
            _data[index].ints = (long*)convert_array<long>(values);
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
            _data[index].strings = (qdb_sized_string*)convert_array<qdb_sized_string>(strings);
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
            _data[index].timestamps = (qdb_timespec*)convert_array<qdb_timespec>(timestamps);
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
            tables[0] = convert_table(_table, _timestamps, _columns, _data);
            var err = qdb_api.qdb_exp_batch_push(_handle, qdb_exp_batch_push_mode.qdb_exp_batch_push_transactional, tables, null, 1);
            QdbExceptionThrower.ThrowIfNeeded(err);
        }

        /// <summary>
        /// Fast, in-place batch push that is efficient when doing lots of small, incremental pushes.
        /// </summary>
        public void PushFast()
        {
            var tables = new qdb_exp_batch_push_table[1];
            tables[0] = convert_table(_table, _timestamps, _columns, _data);
            var err = qdb_api.qdb_exp_batch_push(_handle, qdb_exp_batch_push_mode.qdb_exp_batch_push_fast, tables, null, 1);
            QdbExceptionThrower.ThrowIfNeeded(err);
        }

        /// <summary>
        /// Asynchronous batch push that buffers data inside the QuasarDB daemon.
        /// </summary>
        public void PushAsync()
        {
            var tables = new qdb_exp_batch_push_table[1];
            tables[0] = convert_table(_table, _timestamps, _columns, _data);
            var err = qdb_api.qdb_exp_batch_push(_handle, qdb_exp_batch_push_mode.qdb_exp_batch_push_async, tables, null, 1);
            QdbExceptionThrower.ThrowIfNeeded(err);
        }
    }
}
