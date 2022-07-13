using System.Runtime.InteropServices;
using qdb_time_t = System.Int64;

// ReSharper disable InconsistentNaming
// ReSharper disable BuiltInTypeReferenceStyle

namespace Quasardb.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct qdb_timespec
    {
        internal qdb_time_t tv_sec;
        internal qdb_time_t tv_nsec;

        internal static qdb_timespec Zero => new qdb_timespec {tv_sec = 0, tv_nsec = 0};
        internal static qdb_timespec Null => new qdb_timespec { tv_sec = qdb_time_t.MinValue, tv_nsec = qdb_time_t.MinValue };
        internal static qdb_timespec MinValue => new qdb_timespec {tv_sec = qdb_time_t.MinValue, tv_nsec = 0};
        internal static qdb_timespec MaxValue => new qdb_timespec {tv_sec = qdb_time_t.MaxValue, tv_nsec = 999999999};
    };
}
