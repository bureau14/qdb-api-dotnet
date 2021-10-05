using System;
using System.Collections.Generic;
using System.Text;
using Quasardb.Exceptions;
using Quasardb.Native;

using qdb_size_t = System.UIntPtr;

namespace Quasardb.TimeSeries.Writer
{
    /// <summary>
    /// A batch table for bulk insertion into tables.
    /// </summary>
    public sealed class QdbTableWriter : IDisposable
    {
        private bool disposed = false;

        private readonly qdb_handle _handle;
        private readonly IntPtr _table;
        private readonly InteropableList<qdb_ts_batch_column_info> _columns;

        internal QdbTableWriter(qdb_handle handle, IEnumerable<QdbBatchColumnDefinition> columnDefinitions)
        {
            _handle = handle;

            var count = Helpers.GetCountOrDefault(columnDefinitions);
            _columns = new InteropableList<qdb_ts_batch_column_info>(count);

            foreach (var def in columnDefinitions)
            {
                _columns.Add(new qdb_ts_batch_column_info
                {
                    timeseries = def.Alias,
                    column = def.Column
                });
            }

            var err = qdb_api.qdb_ts_batch_table_init(
                _handle,
                _columns.Buffer, _columns.Count,
                out _table);
            QdbExceptionThrower.ThrowIfNeeded(err);
        }

        /// <inheritdoc />
        ~QdbTableWriter()
        {
            Dispose();
        }

        void Free()
        {
            qdb_api.qdb_release(_handle, _table);
        }

        /// <summary>
        /// Release the batch table.
        /// </summary>
        public void Dispose()
        {
            if(!this.disposed)
            {
                Free();
                GC.SuppressFinalize(this);
                this.disposed = true;
            }
        }

        internal long IndexOf(string column)
        {
            for (int i = 0; i < (int)_columns.Count; ++i)
            {
                if (_columns[i].column == column)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Start a new row to the outcoming data buffer.
        /// </summary>
        /// <param name="timestamp">The timestamp of the new row</param>
        public unsafe void StartRow(DateTime timestamp)
        {
            qdb_timespec converted = TimeConverter.ToTimespec(timestamp);
            var err = qdb_api.qdb_ts_batch_start_row(
                _table,
                &converted);
            QdbExceptionThrower.ThrowIfNeeded(err);
        }

        /// <summary>
        /// Set a value in the current row in a column of blobs.
        /// </summary>
        /// <param name="index">The index to the column you want to modify</param>
        /// <param name="value">The value of the point to insert</param>
        public unsafe void SetBlob(long index, byte[] value)
        {
            var err = qdb_api.qdb_ts_batch_row_set_blob(
                _table,
                (qdb_size_t)index, value, (qdb_size_t)value.LongLength);
            QdbExceptionThrower.ThrowIfNeeded(err);
        }

        /// <summary>
        /// Set a value in the current row in a column of blobs.
        /// </summary>
        /// <param name="name">The name of the column you want to modify</param>
        /// <param name="value">The value of the point to insert</param>
        public unsafe void SetBlob(string name, byte[] value)
        {
            long index = IndexOf(name);
            if (index == -1)
                throw new QdbColumnNotFoundException(null, name);

            SetBlob(index, value);
        }

        /// <summary>
        /// Set a value in the current row in a column of doubles.
        /// </summary>
        /// <param name="index">The index to the column you want to modify</param>
        /// <param name="value">The value of the point to insert</param>
        public void SetDouble(long index, double? value)
        {
            var err = qdb_api.qdb_ts_batch_row_set_double(
                _table,
                (qdb_size_t)index, value ?? double.NaN);
            QdbExceptionThrower.ThrowIfNeeded(err);
        }

        /// <summary>
        /// Set a value in the current row in a column of doubles.
        /// </summary>
        /// <param name="name">The name of the column you want to modify</param>
        /// <param name="value">The value of the point to insert</param>
        public void SetDouble(string name, double? value)
        {
            long index = IndexOf(name);
            if (index == -1)
                throw new QdbColumnNotFoundException(null, name);

            SetDouble(index, value);
        }

        /// <summary>
        /// Set a value in the current row in a column of integers.
        /// </summary>
        /// <param name="index">The index to the column you want to modify</param>
        /// <param name="value">The value of the point to insert</param>
        public void SetInt64(long index, long? value)
        {
            var err = qdb_api.qdb_ts_batch_row_set_int64(
                _table,
                (qdb_size_t)index, value ?? long.MinValue);
            QdbExceptionThrower.ThrowIfNeeded(err);
        }

        /// <summary>
        /// Set a value in the current row in a column of integers.
        /// </summary>
        /// <param name="name">The name of the column you want to modify</param>
        /// <param name="value">The value of the point to insert</param>
        public void SetInt64(string name, long? value)
        {
            long index = IndexOf(name);
            if (index == -1)
                throw new QdbColumnNotFoundException(null, name);

            SetInt64(index, value);
        }

        /// <summary>
        /// Set a value in the current row in a column of strings.
        /// </summary>
        /// <param name="index">The index to the column you want to modify</param>
        /// <param name="value">The value of the point to insert</param>
        public unsafe void SetString(long index, string value)
        {
            var content = Encoding.UTF8.GetBytes(value);
            var err = qdb_api.qdb_ts_batch_row_set_string(
                _table,
                (qdb_size_t)index, content, (qdb_size_t)content.LongLength);
            QdbExceptionThrower.ThrowIfNeeded(err);
        }

        /// <summary>
        /// Set a value in the current row in a column of strings.
        /// </summary>
        /// <param name="name">The name of the column you want to modify</param>
        /// <param name="value">The value of the point to insert</param>
        public unsafe void SetString(string name, string value)
        {
            long index = IndexOf(name);
            if (index == -1)
                throw new QdbColumnNotFoundException(null, name);

            SetString(index, value);
        }

        /// <summary>
        /// Set a value in the current row in a column of symbols.
        /// </summary>
        /// <param name="index">The index to the column you want to modify</param>
        /// <param name="value">The value of the point to insert</param>
        public unsafe void SetSymbol(long index, string value)
        {
            var content = Encoding.UTF8.GetBytes(value);
            var err = qdb_api.qdb_ts_batch_row_set_symbol(
                _table,
                (qdb_size_t)index, content, (qdb_size_t)content.LongLength);
            QdbExceptionThrower.ThrowIfNeeded(err);
        }

        /// <summary>
        /// Set a value in the current row in a column of symbols.
        /// </summary>
        /// <param name="name">The name of the column you want to modify</param>
        /// <param name="value">The value of the point to insert</param>
        public unsafe void SetSymbol(string name, string value)
        {
            long index = IndexOf(name);
            if (index == -1)
                throw new QdbColumnNotFoundException(null, name);

            SetSymbol(index, value);
        }

        /// <summary>
        /// Set a value in the current row in a column of timestamps.
        /// </summary>
        /// <param name="index">The index to the column you want to modify</param>
        /// <param name="value">The value of the point to insert</param>
        public unsafe void SetTimestamp(long index, DateTime? value)
        {
            qdb_timespec converted = TimeConverter.ToTimespec(value);
            var err = qdb_api.qdb_ts_batch_row_set_timestamp(
                _table,
                (qdb_size_t)index, &converted);
            QdbExceptionThrower.ThrowIfNeeded(err);
        }

        /// <summary>
        /// Set a value in the current row in a column of timestamps.
        /// </summary>
        /// <param name="name">The name of the column you want to modify</param>
        /// <param name="value">The value of the point to insert</param>
        public unsafe void SetTimestamp(string name, DateTime? value)
        {
            long index = IndexOf(name);
            if (index == -1)
                throw new QdbColumnNotFoundException(null, name);

            SetTimestamp(index, value);
        }

        /// <summary>
        /// Regular batch push.
        /// </summary>
        public void Push()
        {
            var err = qdb_api.qdb_ts_batch_push(
                _table);
            QdbExceptionThrower.ThrowIfNeeded(err);
        }

        /// <summary>
        /// Fast, in-place batch push that is efficient when doing lots of small, incremental pushes.
        /// </summary>
        public void PushFast()
        {
            var err = qdb_api.qdb_ts_batch_push_fast(
                _table);
            QdbExceptionThrower.ThrowIfNeeded(err);
        }

        /// <summary>
        /// Asynchronous batch push that buffers data inside the QuasarDB daemon.
        /// </summary>
        public void PushAsync()
        {
            var err = qdb_api.qdb_ts_batch_push_async(
                _table);
            QdbExceptionThrower.ThrowIfNeeded(err);
        }
    }
}
