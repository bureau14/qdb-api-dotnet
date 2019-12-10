using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Quasardb.Exceptions;
using Quasardb.Native;

using qdb_size_t = System.UIntPtr;

namespace Quasardb.TimeSeries
{
    /// <summary>
    /// A batch table for bulk insertion in time series
    /// </summary>
    public sealed class QdbTimeSeriesBatch : IDisposable
    {
        private readonly qdb_handle _handle;
        private readonly IntPtr _table;

        internal QdbTimeSeriesBatch(qdb_handle handle, IEnumerable<QdbBatchColumnDefinition> columnDefinitions)
        {
            _handle = handle;

            var count = Helpers.GetCountOrDefault(columnDefinitions);
            var columns = new InteropableList<qdb_ts_batch_column_info>(count);

            foreach (var def in columnDefinitions)
            {
                columns.Add(new qdb_ts_batch_column_info
                {
                    timeseries = def.Alias,
                    column = def.Column
                });
            }

            var err = qdb_api.qdb_ts_batch_table_init(
                _handle,
                columns.Buffer, columns.Count,
                out _table);
            QdbExceptionThrower.ThrowIfNeeded(err);
        }

        /// <summary>
        /// Release the batch table.
        /// </summary>
        public void Dispose()
        {
            qdb_api.qdb_release(_handle, _table);
        }

        /// <summary>
        /// Start a new row to the outcoming data buffer.
        /// </summary>
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
        /// Set a value in the current row in a column of doubles.
        /// </summary>
        /// <param name="index">The index to the column you want to modify</param>
        /// <param name="value">The value of the point to insert</param>
        public void SetDouble(long index, double value)
        {
            var err = qdb_api.qdb_ts_batch_row_set_double(
                _table,
                (qdb_size_t)index, value);
            QdbExceptionThrower.ThrowIfNeeded(err);
        }

        /// <summary>
        /// Set a value in the current row in a column of integers.
        /// </summary>
        /// <param name="index">The index to the column you want to modify</param>
        /// <param name="value">The value of the point to insert</param>
        public void SetInt64(long index, long value)
        {
            var err = qdb_api.qdb_ts_batch_row_set_int64(
                _table,
                (qdb_size_t)index, value);
            QdbExceptionThrower.ThrowIfNeeded(err);
        }

        /// <summary>
        /// Set a value in the current row in a column of timestamps.
        /// </summary>
        /// <param name="index">The index to the column you want to modify</param>
        /// <param name="value">The value of the point to insert</param>
        public unsafe void SetTimestamp(long index, DateTime value)
        {
            qdb_timespec converted = TimeConverter.ToTimespec(value);
            var err = qdb_api.qdb_ts_batch_row_set_timestamp(
                _table,
                (qdb_size_t)index, &converted);
            QdbExceptionThrower.ThrowIfNeeded(err);
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
