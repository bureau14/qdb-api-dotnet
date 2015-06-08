using System;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace Quasardb.Interop
{
    sealed class qdb_buffer : SafeHandle
    {
        public qdb_buffer() : base(IntPtr.Zero, true)
        {
        }

        protected override bool ReleaseHandle()
        {
            qdb_api.qdb_free_buffer(handle);
            return true;
        }

        public override bool IsInvalid
        {
            get { return handle != IntPtr.Zero; }
        }

        public byte[] Copy(IntPtr length)
        {
            var buffer = new byte[length.ToInt64()];
            Marshal.Copy(handle, buffer, 0, length.ToInt32()); // TODO: find how to avoid the cast to int32
            return buffer;
        }
    }
}
