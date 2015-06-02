using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable InconsistentNaming

namespace Quasardb.Interop
{
    class qdb_buffer : SafeHandle
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

        public byte[] Copy(long length)
        {
            var buffer = new byte[length];
            Marshal.Copy(handle, buffer, 0, (int)length); // TODO: find how to avoid the cast to int
            return buffer;
        }
    }
}
