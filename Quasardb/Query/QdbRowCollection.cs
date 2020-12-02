using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Quasardb.Native;

// ReSharper disable InconsistentNaming

namespace Quasardb.Query
{
    /// <summary>
    /// A read-only collection of result rows.
    /// </summary>
    public class QdbRowCollection : IEnumerable<QdbRow>
    {
        static readonly long SizeOfT = Marshal.SizeOf(typeof(qdb_point_result*));

        private readonly IntPtr _pointer;
        private readonly UIntPtr _size;
        private readonly QdbColumnNameCollection _columns;

        internal unsafe QdbRowCollection(qdb_point_result** pointer, UIntPtr size, QdbColumnNameCollection columns)
        {
            _pointer = new IntPtr(pointer);
            _size = size;
            _columns = columns;
        }

        internal unsafe QdbRowCollection()
        {
            _pointer = new IntPtr(null);
            _size = new UIntPtr(0);
            _columns = new QdbColumnNameCollection();
        }

        /// <summary>
        /// Gets the number of result rows in the collection.
        /// </summary>
        public long Count => (long)_size;

        /// <summary>
        /// Gets the result row at the specified index.
        /// </summary>
        /// <param name="index">The zero-based position in the collection</param>
        /// <exception cref="ArgumentOutOfRangeException">If index is negative or above Count</exception>
        public unsafe QdbRow this[long index]
        {
            get
            {
                if (index < 0 || (ulong)index >= _size.ToUInt64()) throw new ArgumentOutOfRangeException();
                var p = new IntPtr(_pointer.ToInt64() + SizeOfT * index);
                return new QdbRow((qdb_point_result*)Marshal.ReadIntPtr(p), _columns);
            }
        }

        /// <inheritdoc />
        public IEnumerator<QdbRow> GetEnumerator()
        {
            for (var i = 0L; (ulong)i < _size.ToUInt64(); i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
