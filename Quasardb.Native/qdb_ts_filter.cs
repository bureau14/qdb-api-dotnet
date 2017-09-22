using System.Runtime.InteropServices;

using qdb_size_t = System.UIntPtr;

namespace Quasardb.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct qdb_ts_filter
    {
        public qdb_ts_filter_type type;
        public qdb_filter_params parameters;

        [StructLayout(LayoutKind.Explicit)]
        public struct qdb_filter_params
        {
            [FieldOffset(0)]
            public qdb_filter_sample_args sample;

            [FieldOffset(0)]
            public qdb_filter_double_range_args double_range;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct qdb_filter_sample_args
        {
            public qdb_size_t size;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct qdb_filter_double_range_args
        {
            public double min;
            public double max;
        }

    };
}
