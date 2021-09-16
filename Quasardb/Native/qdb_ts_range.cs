using System.Runtime.InteropServices;

namespace Quasardb.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct qdb_ts_range
    {
        internal qdb_timespec begin;
        internal qdb_timespec end;
    };
}
