using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
        static readonly long SizeOfT = Marshal.SizeOf(typeof(qdb_point_result));

        private readonly IntPtr _table;
        private readonly string _alias;
        private readonly qdb_ts_column_info[] _columns;

        internal QdbRow(IntPtr table, string alias, InteropableList<qdb_ts_column_info> columns)
        {
            _table = table;
            _alias = alias;
            _columns = new qdb_ts_column_info[(long)columns.Count];
            for (int i = 0; i < (long)columns.Count; ++i)
                _columns[i] = columns[i];
        }

        internal int IndexOf(string column)
        {
            for (int i = 0; i < _columns.Length; ++i)
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
    }
}
