using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Quasardb.Native
{
    internal abstract class qdb_buffer : SafeHandle
    {
        protected readonly qdb_handle _handle;

        public IntPtr Pointer;
        public UIntPtr Size;

        protected qdb_buffer(qdb_handle handle) : base(IntPtr.Zero, true)
        {
            _handle = handle;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle()
        {
            qdb_api.qdb_release(_handle, Pointer);
            return true;
        }

        public override bool IsInvalid
        {
            get { return _handle == null || _handle.IsInvalid; }
        }
    }

    internal class qdb_buffer<T> : qdb_buffer, IEnumerable<T>
    {
        readonly long SizeOfT = Marshal.SizeOf(typeof(T));

        public qdb_buffer(qdb_handle handle) : base(handle)
        {
        }

        public T this[ulong index]
        {
            get
            {
                if (index >= Size.ToUInt64()) throw new IndexOutOfRangeException();
                var p = new IntPtr(Pointer.ToInt64() + SizeOfT * (long)index);
                return (T)Marshal.PtrToStructure(p, typeof(T));
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0UL; i < Size.ToUInt64(); i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}