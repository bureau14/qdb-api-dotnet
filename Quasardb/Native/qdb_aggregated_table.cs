using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

using qdb_size_t = System.UIntPtr;
using qdb_uint_t = System.UInt64;

namespace Quasardb.Native
{
    internal enum qdb_aggregation_window_type : int
    {
        qdb_window_by_duration = 0,
        qdb_window_by_row_count = 1,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct qdb_ts_aggregated_column_info
    {
        internal qdb_ts_column_info_ex info;
        internal qdb_ts_aggregation_type aggregation;
        internal qdb_size_t index;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct qdb_window_params
    {
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal qdb_duration d_size;
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal qdb_uint_t c_size;

        [System.Runtime.InteropServices.FieldOffset(8)]
        internal qdb_duration d_hopping;
        [System.Runtime.InteropServices.FieldOffset(8)]
        internal qdb_uint_t c_hopping;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct qdb_aggregated_table
    {
        internal qdb_aggregation_window_type window_type;
        internal qdb_window_params window_params;

        internal qdb_ts_aggregated_column_info* columns;
        internal qdb_size_t column_count;
        internal qdb_uint_t sample_size;
        internal qdb_duration watermark;
    }
}
