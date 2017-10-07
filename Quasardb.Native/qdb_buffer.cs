using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
// ReSharper disable InconsistentNaming

namespace Quasardb.Native
{
    public abstract unsafe class qdb_buffer : CriticalFinalizerObject, IDisposable
    {
        protected readonly qdb_handle _handle;

        public IntPtr Pointer;
        public UIntPtr Size;

        public qdb_buffer(qdb_handle handle)
        {
            _handle = handle;
        }

        ~qdb_buffer()
        {
            qdb_api.qdb_free_buffer(_handle, Pointer);
        }

        public void Dispose()
        {
            Free();
            GC.SuppressFinalize(this);
            Pointer = IntPtr.Zero;
        }

        protected abstract void Free();
    }

    public abstract unsafe class qdb_buffer<T> : qdb_buffer, IEnumerable<T>
    {
        protected abstract T Dereference(void* p, ulong i);

        protected qdb_buffer(qdb_handle handle) : base(handle)
        {
        }

        public T this[ulong index]
        {
            get
            {
                if (index >= Size.ToUInt64()) throw new IndexOutOfRangeException();
                return Dereference(Pointer.ToPointer(), index);
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