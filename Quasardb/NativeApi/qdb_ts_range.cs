using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

namespace Quasardb.NativeApi
{
    [StructLayout(LayoutKind.Sequential)]
    struct qdb_ts_range
    {
        public qdb_timespec begin;
        public qdb_timespec end;
    };
}
