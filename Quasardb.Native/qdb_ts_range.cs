using System.Runtime.InteropServices;

namespace Quasardb.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct qdb_ts_range
    {
        public qdb_timespec begin;
        public qdb_timespec end;
    };
}
