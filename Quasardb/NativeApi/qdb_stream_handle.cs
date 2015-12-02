using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Quasardb.NativeApi
{
    sealed class qdb_stream_handle : SafeHandle
    {
        public qdb_stream_handle()
            : base(IntPtr.Zero, true)
        {
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle()
        {
            return qdb_api.qdb_stream_close(handle) == qdb_error.qdb_e_ok;
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }
    }
}
