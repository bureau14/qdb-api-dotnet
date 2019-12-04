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
    public class QdbColumnNameCollection : IEnumerable<String>
    {
        static readonly long SizeOfT = Marshal.SizeOf(typeof(qdb_sized_string));

        private readonly IntPtr _pointer;
        private readonly UIntPtr _size;

        internal unsafe QdbColumnNameCollection(qdb_sized_string* pointer, UIntPtr size)
        {
            _pointer = new IntPtr(pointer);
            _size = size;
        }

        /// <summary>
        /// Gets the number of column names in the collection.
        /// </summary>
        public long Count => (long)_size;

        /// <summary>
        /// Gets the column name at the specified index.
        /// </summary>
        /// <param name="index">The zero-based position in the collection</param>
        /// <exception cref="ArgumentOutOfRangeException">If index is negative or above Count</exception>
        public String this[long index]
        {
            get
            {
                if (index < 0 || (ulong)index >= _size.ToUInt64()) throw new ArgumentOutOfRangeException();
                var p = new IntPtr(_pointer.ToInt64() + SizeOfT * index);
                return (qdb_sized_string)Marshal.PtrToStructure(p, typeof(qdb_sized_string));
            }
        }

        /// <inheritdoc />
        public IEnumerator<String> GetEnumerator()
        {
            for (var i = 0L; (ulong)i < _size.ToUInt64(); i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
