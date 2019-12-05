using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Quasardb.Native;

// ReSharper disable InconsistentNaming

namespace Quasardb.Query
{
    /// <summary>
    /// A read-only collection of column names.
    /// </summary>
    public class QdbColumnNameCollection : IEnumerable<string>
    {
        private readonly string[] _columns;

        internal unsafe QdbColumnNameCollection(qdb_sized_string* pointer, UIntPtr size)
        {
            _columns = new string[size.ToUInt64()];
            for (var i = 0UL; i < size.ToUInt64(); i++)
            {
                var p = new IntPtr(pointer + i);
                _columns[i] = (qdb_sized_string)Marshal.PtrToStructure(p, typeof(qdb_sized_string));
            }
        }

        internal int IndexOf(string column) => Array.IndexOf(_columns, column);

        /// <summary>
        /// Gets the number of column names in the collection.
        /// </summary>
        public long Count => _columns.LongLength;

        /// <summary>
        /// Gets the column name at the specified index.
        /// </summary>
        /// <param name="index">The zero-based position in the collection</param>
        /// <exception cref="ArgumentOutOfRangeException">If index is negative or above Count</exception>
        public string this[long index]
        {
            get
            {
                return _columns[index];
            }
        }

        /// <inheritdoc />
        public IEnumerator<string> GetEnumerator()
        {
            for (var i = 0L; i < _columns.LongLength; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
