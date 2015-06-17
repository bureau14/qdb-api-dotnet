using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Quasardb.Interop;

namespace Quasardb
{
    sealed class QdbTagCollection : IDisposable, IEnumerable<QdbTag>
    {
        readonly qdb_handle _handle;
        internal IntPtr Pointer;
        internal IntPtr Size;
        
        internal QdbTagCollection(qdb_handle handle)
        {
            _handle = handle;
        }

        ~QdbTagCollection()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        void Dispose(bool disposing)
        {
            qdb_api.qdb_free_results(_handle, Pointer, Size);
            Pointer = IntPtr.Zero;
            Size = IntPtr.Zero;
        }

        public IEnumerator<QdbTag> GetEnumerator()
        {
            ThrowIfDisposed();

            // CAUTION: limited to 32 bits!!!
            for (var i = 0; i < (int) Size; i++)
            {
                var aliasPointer = Marshal.ReadIntPtr(Pointer,  i*IntPtr.Size);
                var alias = Marshal.PtrToStringAnsi(aliasPointer);

                yield return new QdbTag(_handle, alias);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ThrowIfDisposed()
        {
            if (Pointer != IntPtr.Zero) return;
            if (Size == IntPtr.Zero) return;
            throw new ObjectDisposedException("The tag collection has been disposed", (Exception)null);
        }
    }
}
