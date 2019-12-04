using System;
using Quasardb.Exceptions;
using Quasardb.Native;

// ReSharper disable InconsistentNaming

namespace Quasardb.Query
{
    /// <summary>
    /// Holds the result of a query.
    /// </summary>
    public unsafe sealed class QdbQueryResult : IDisposable
    {
        readonly qdb_handle _handle;
        readonly qdb_query_result* _result;

        internal QdbQueryResult(qdb_handle handle, string query)
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
                    qdb_api.qdb_release(_handle, _result);
                }
                throw new QdbQueryException(error_message ?? qdb_api.qdb_error(error));
            }

            ColumnNames = new QdbColumnNameCollection(_result->column_names, _result->column_count);
            Rows = new QdbRowCollection(_result->rows, _result->row_count, ColumnNames);
        }

        /// <summary>
        /// Release the query result.
        /// </summary>
        public void Dispose()
        {
            qdb_api.qdb_release(_handle, _result);
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
