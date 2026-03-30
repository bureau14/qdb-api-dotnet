using System;
using System.Collections;
using System.Collections.Generic;
using Quasardb.Exceptions;
using Quasardb.Native;

using qdb_size_t = System.UIntPtr;

// ReSharper disable InconsistentNaming

namespace Quasardb.TimeSeries.Reader
{
    /// <summary>
    /// A row of values.
    /// </summary>
    public class QdbRow : IEnumerable<QdbCell>
    {
        private readonly IntPtr _table;
        private readonly string _alias;
        private readonly qdb_ts_column_info[] _columns;
        private readonly QdbCell[] _cells;

        internal QdbRow(IntPtr table, string alias, InteropableList<qdb_ts_column_info> columns)
        {
            _table = table;
            _alias = alias;
            _columns = new qdb_ts_column_info[(long)columns.Count];
            for (var i = 0; i < (long)columns.Count; ++i)
                _columns[i] = columns[i];
        }

        internal QdbRow(string alias, qdb_ts_column_info[] columns, QdbCell[] cells)
        {
            _table = IntPtr.Zero;
            _alias = alias;
            _columns = columns;
            _cells = cells;
        }

        internal int IndexOf(string column)
        {
            for (var i = 0; i < _columns.Length; ++i)
            {
                if (_columns[i].name == column)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Gets the timestamp of the row.
        /// </summary>
        public DateTime Timestamp { get; internal set; }

        /// <summary>
        /// Gets the number of values in the row.
        /// </summary>
        public long Count => _columns.LongLength;

        /// <summary>
        /// Gets the result value at the specified index.
        /// </summary>
        /// <param name="index">The zero-based position in the row</param>
        /// <exception cref="ArgumentOutOfRangeException">If index is negative or above Count</exception>
        public QdbCell this[long index]
        {
            get
            {
                if (index < 0 || index >= _columns.LongLength) throw new ArgumentOutOfRangeException();
                if (_cells != null) return _cells[index];
                return new QdbCell(_table, _alias, _columns[index], (qdb_size_t)index);
            }
        }

        /// <summary>
        /// Gets the result value at the column with the specified name.
        /// </summary>
        /// <param name="name">The name of the column</param>
        /// <exception cref="QdbColumnNotFoundException">If a column with the given name is not found.</exception>
        public QdbCell this[string name]
        {
            get
            {
                long index = IndexOf(name);
                if (index != -1) return this[index];

                throw new QdbColumnNotFoundException(_alias, name);
            }
        }

        /// <inheritdoc />
        public IEnumerator<QdbCell> GetEnumerator()
        {
            for (var i = 0L; i < _columns.LongLength; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        internal QdbRow Snapshot()
        {
            var cells = new QdbCell[_columns.Length];
            for (var i = 0; i < _columns.Length; ++i)
            {
                var cell = this[i];
                object value;
                switch (cell.Type)
                {
                    case QdbColumnType.Double:
                        value = cell.DoubleValue;
                        break;
                    case QdbColumnType.Blob:
                        value = cell.BlobValue;
                        break;
                    case QdbColumnType.Int64:
                        value = cell.Int64Value;
                        break;
                    case QdbColumnType.String:
                    case QdbColumnType.Symbol:
                        value = cell.StringValue;
                        break;
                    case QdbColumnType.Timestamp:
                        value = cell.TimestampValue;
                        break;
                    default:
                        value = cell.Value;
                        break;
                }

                cells[i] = new QdbCell(_alias, _columns[i], value);
            }

            return new QdbRow(_alias, _columns, cells)
            {
                Timestamp = Timestamp
            };
        }
    }
}
