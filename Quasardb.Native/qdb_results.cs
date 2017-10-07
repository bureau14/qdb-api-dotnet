using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
// ReSharper disable InconsistentNaming

namespace Quasardb.Native
{
    public unsafe class qdb_results : CriticalFinalizerObject, IDisposable
    {
        public struct data {}

        readonly qdb_handle _handle;

        public data* Pointer;
        public UIntPtr Size;

        public qdb_results(qdb_handle handle)
        {
            _handle = handle;
        }

        ~qdb_results()
        {
            qdb_api.qdb_free_results(_handle, Pointer, Size);
        }

        public void Dispose()
        {
            qdb_api.qdb_free_results(_handle, Pointer, Size);
            GC.SuppressFinalize(this);
            Pointer = null;
        }
    }

    public abstract unsafe class qdb_results<T> : qdb_results, IEnumerable<T>
    {
        protected abstract T Dereference(void* p, ulong i);

        protected qdb_results(qdb_handle handle) : base(handle)
        {
        }

        public T this[ulong index]
        {
            get
            {
                if (index >= Size.ToUInt64()) throw new IndexOutOfRangeException();
                return Dereference(Pointer, index);
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