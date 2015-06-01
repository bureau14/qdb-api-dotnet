using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Quasardb.Interop
{
    public sealed class qdb_handle : SafeHandle
    {
        public qdb_handle() : base(IntPtr.Zero, true)
        {
        }

        protected override bool ReleaseHandle()
        {
            return qdb_api.qdb_close(handle) == qdb_error.qdb_e_ok;
        }

        public override bool IsInvalid
        {
            get { return handle != IntPtr.Zero; }
        }
    }
}
