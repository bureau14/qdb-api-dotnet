using System;
using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

using qdb_size_t = System.UIntPtr;

namespace Quasardb.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct qdb_ts_blob_aggregation
    {
        public qdb_ts_aggregation_type type;
        public qdb_ts_range range;
        public qdb_size_t count;
        public qdb_ts_blob_point result;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct qdb_ts_double_aggregation
    {
        public qdb_ts_aggregation_type type;
        public qdb_ts_range range;
        public qdb_size_t count;
        public qdb_ts_double_point result;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct qdb_ts_int64_aggregation
    {
        public qdb_ts_aggregation_type type;
        public qdb_ts_range range;
        public qdb_size_t count;
        public qdb_ts_int64_point result;
        public double exact_result;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct qdb_ts_string_aggregation
    {
        public qdb_ts_aggregation_type type;
        public qdb_ts_range range;
        public qdb_size_t count;
        public qdb_ts_string_point result;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct qdb_ts_timestamp_aggregation
    {
        public qdb_ts_aggregation_type type;
        public qdb_ts_range range;
        public qdb_size_t count;
        public qdb_ts_timestamp_point result;
    };
    
    [StructLayout(LayoutKind.Sequential)]
    public struct qdb_ts_symbol_aggregation
    {
        public qdb_ts_aggregation_type type;
        public qdb_ts_range range;
        public qdb_size_t count;
        public qdb_ts_symbol_point result;
    };
}
