using System.Runtime.InteropServices;

namespace Quasardb.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct qdb_ts_filtered_range
    {
        public qdb_ts_range range;
        public qdb_ts_filter filter;
    };
}
