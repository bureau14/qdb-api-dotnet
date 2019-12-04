using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Quasardb.Exceptions;
using Quasardb.Native;

// ReSharper disable InconsistentNaming

namespace Quasardb.Query
{
    /// <summary>
    /// A row of results.
    /// </summary>
    public class QdbRow : IEnumerable<QdbCell>
    {
        static readonly long SizeOfT = Marshal.SizeOf(typeof(qdb_point_result));

        private readonly IntPtr _pointer;
        private readonly QdbColumnNameCollection _columns;

        internal unsafe QdbRow(qdb_point_result* pointer, QdbColumnNameCollection columns)
        {
            _pointer = new IntPtr(pointer);
            _columns = columns;
        }

        /// <summary>
        /// Gets the number of results in the row.
        /// </summary>
        public long Count => _columns.Count;

        /// <summary>
        /// Gets the result value at the specified index.
        /// </summary>
        /// <param name="index">The zero-based position in the row</param>
        /// <exception cref="ArgumentOutOfRangeException">If index is negative or above Count</exception>
        public QdbCell this[long index]
        {
            get
            {
                if (index < 0 || index >= _columns.Count) throw new ArgumentOutOfRangeException();
                var p = new IntPtr(_pointer.ToInt64() + SizeOfT * (long)index);
                return new QdbCell((qdb_point_result)Marshal.PtrToStructure(p, typeof(qdb_point_result)));
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
                long index = _columns.IndexOf(name);
                if (index != -1) return this[index];

                long tableIndex = _columns.IndexOf("$table");
                string alias = tableIndex != -1 ? this[tableIndex].StringValue : null;
                throw new QdbColumnNotFoundException(alias, name);
            }
        }

        /// <inheritdoc />
        public IEnumerator<QdbCell> GetEnumerator()
        {
            for (var i = 0L; (long)i < _columns.Count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
