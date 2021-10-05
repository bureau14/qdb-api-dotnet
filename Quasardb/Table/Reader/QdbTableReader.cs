using Quasardb.Exceptions;
using Quasardb.Native;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Quasardb.TimeSeries.Reader
{
    /// <summary>
    /// A local table used for reading from a table.
    /// </summary>
    public sealed class QdbTableReader : IEnumerable<QdbRow>, IDisposable
    {
        private bool disposed = false;

        private readonly qdb_handle _handle;
        private readonly string _alias;
        private readonly IntPtr _table;
        private QdbRow _row;

        internal QdbTableReader(qdb_handle handle, string alias, IntPtr table, InteropableList<qdb_ts_column_info> columns)
        {
            _handle = handle;
            _alias = alias;
            _table = table;
            _row = new QdbRow(_table, alias, columns);
        }

        void Free()
        {
            qdb_api.qdb_release(_handle, _table);
        }

        /// <summary>
        /// Release the table reader.
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

        private bool NextRow()
        {
            var err = qdb_api.qdb_ts_table_next_row(_table, out qdb_timespec timestamp);
            if (err == qdb_error.qdb_e_iterator_end)
            {
                _row = null;
                return false;
            }

            QdbExceptionThrower.ThrowIfNeeded(err, alias: _alias);
            _row.Timestamp = TimeConverter.ToDateTime(timestamp);
            return true;
        }

        /// <inheritdoc />
        public IEnumerator<QdbRow> GetEnumerator()
        {
            while (NextRow())
            {
                yield return _row;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
