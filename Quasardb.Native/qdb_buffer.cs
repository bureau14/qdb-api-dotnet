using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Quasardb.Native
{
    public sealed unsafe class qdb_buffer : CriticalFinalizerObject, IDisposable
    {
        readonly qdb_handle _handle;

        public byte* Pointer;
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
            qdb_api.qdb_free_buffer(_handle, Pointer);
            GC.SuppressFinalize(this);
            Pointer = null;
            Size = UIntPtr.Zero;
        }

        public byte[] GetBytes()
        {
            // CAUTION: limited to 32 bits!!!
            int size = (int)Size;
            var buffer = new byte[size];
            Marshal.Copy(new IntPtr(Pointer), buffer, 0, size);
            return buffer;
        }
    }
}
