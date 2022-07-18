using System;
using Quasardb.Exceptions;
using Quasardb.Native;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Quasardb.Query
{
    /// <summary>
    /// Holds the result of a query.
    /// </summary>
    public unsafe sealed class QdbQueryResult : SafeHandle
    {
        readonly qdb_handle _handle;
        readonly qdb_query_result* _result;

        internal QdbQueryResult(qdb_handle handle, string query) : base(IntPtr.Zero, true)
        {
            _handle = handle;

            var error = qdb_api.qdb_query(_handle, query, out _result);
            if (error != qdb_error.qdb_e_ok)
            {
                string error_message = null;
                if (_result != null)
                {
                    if (_result->error_message.length.ToUInt64() > 0UL)
                        error_message = _result->error_message;
                    qdb_api.qdb_release(_handle, new IntPtr(_result));
                }
                throw new QdbQueryException(error_message ?? qdb_api.qdb_error(error));
            }

            if (_result != null)
            {
                ColumnNames = new QdbColumnNameCollection(_result->column_names, _result->column_count);
                Rows = new QdbRowCollection(_result->rows, _result->row_count, ColumnNames);
            }
            else
            {
                ColumnNames = new QdbColumnNameCollection();
                Rows = new QdbRowCollection();
            }
        }

        internal QdbQueryResult(qdb_handle handle, qdb_query_result* result) : base(IntPtr.Zero, true)
        {
            _handle = handle;
            _result = result;

            if (_result != null)
            {
                ColumnNames = new QdbColumnNameCollection(_result->column_names, _result->column_count);
                Rows = new QdbRowCollection(_result->rows, _result->row_count, ColumnNames);
            }
            else
            {
                ColumnNames = new QdbColumnNameCollection();
                Rows = new QdbRowCollection();
            }
        }

        /// <inheritdoc />
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle()
        {
            if (!_handle.IsClosed)
            {
                qdb_api.qdb_release(_handle, new IntPtr(_result));
            }
            return true;
        }

        /// <inheritdoc />
        public override bool IsInvalid
        {
            get { return _handle == null || _handle.IsInvalid; }
        }

        /// <summary>
        /// The returned columns.
        /// </summary>
        public QdbColumnNameCollection ColumnNames { get; }

        /// <summary>
        /// The returned rows.
        /// </summary>
        public QdbRowCollection Rows { get; }

        /// <summary>
        /// The number of scanned points, for information purposes.
        /// </summary>
        /// <remarks>The actual number of scanned points may be greater</remarks>
        public long ScannedPointCount => (long)_result->scanned_point_count;

        /// <summary>
        /// An optional, detailed error message about the query failure.
        /// </summary>
        public string ErrorMessage => _result->error_message;
    }
}
