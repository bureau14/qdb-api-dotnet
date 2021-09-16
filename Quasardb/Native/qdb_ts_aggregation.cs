using System;
using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

using qdb_size_t = System.UIntPtr;

namespace Quasardb.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct qdb_ts_blob_aggregation
    {
        internal qdb_ts_aggregation_type type;
        internal qdb_ts_range range;
        internal qdb_size_t count;
        internal qdb_ts_blob_point result;
    };

    [StructLayout(LayoutKind.Sequential)]
    internal struct qdb_ts_double_aggregation
    {
        internal qdb_ts_aggregation_type type;
        internal qdb_ts_range range;
        internal qdb_size_t count;
        internal qdb_ts_double_point result;
    };

    [StructLayout(LayoutKind.Sequential)]
    internal struct qdb_ts_int64_aggregation
    {
        internal qdb_ts_aggregation_type type;
        internal qdb_ts_range range;
        internal qdb_size_t count;
        internal qdb_ts_int64_point result;
        internal double exact_result;
    };

    [StructLayout(LayoutKind.Sequential)]
    internal struct qdb_ts_string_aggregation
    {
        internal qdb_ts_aggregation_type type;
        internal qdb_ts_range range;
        internal qdb_size_t count;
        internal qdb_ts_string_point result;
    };

    [StructLayout(LayoutKind.Sequential)]
    internal struct qdb_ts_timestamp_aggregation
    {
        internal qdb_ts_aggregation_type type;
        internal qdb_ts_range range;
        internal qdb_size_t count;
        internal qdb_ts_timestamp_point result;
    };
}
