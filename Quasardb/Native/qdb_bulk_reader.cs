using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

using qdb_size_t = System.UIntPtr;
using qdb_char_ptr = System.IntPtr;

namespace Quasardb.Native
{
    internal abstract class qdb_reader_handle : SafeHandle
    {
        protected readonly qdb_handle _handle;

        public IntPtr Pointer;
        public UIntPtr Size;

        protected qdb_reader_handle(qdb_handle handle) : base(IntPtr.Zero, true)
        {
            _handle = handle;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle()
        {
            if (!_handle.IsClosed)
            {
                qdb_api.qdb_release(_handle, Pointer);
            }
            return true;
        }

        public override bool IsInvalid
        {
            get { return _handle == null || _handle.IsInvalid; }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct qdb_bulk_reader_table
    {
        internal qdb_char_ptr name;
        internal qdb_ts_range* ranges;
        internal qdb_size_t range_count;
    }
}
