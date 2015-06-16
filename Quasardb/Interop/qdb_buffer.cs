using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Quasardb.Interop
{
    sealed class qdb_buffer : CriticalFinalizerObject, IDisposable
    {
        readonly qdb_handle _handle;

        internal IntPtr Pointer;
        internal IntPtr Size;

        public qdb_buffer(qdb_handle handle)
        {
            _handle = handle;
        }

        ~qdb_buffer()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /*protected virtual*/ void Dispose(bool disposing)
        {
            qdb_api.qdb_free_buffer(_handle, Pointer);
            Pointer = IntPtr.Zero;
            Size = IntPtr.Zero;
        }

        public byte[] GetBytes()
        {
            var buffer = new byte[Size.ToInt64()];
            Marshal.Copy(Pointer, buffer, 0, Size.ToInt32()); // TODO: find how to avoid the cast to int32
            return buffer;
        }
    }
}
