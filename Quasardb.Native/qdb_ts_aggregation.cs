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
        public qdb_ts_filtered_range range;
        public qdb_size_t count;
        public qdb_ts_blob_point result;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct qdb_ts_double_aggregation
    {
        public qdb_ts_aggregation_type type;
        public qdb_ts_filtered_range range;
        public qdb_size_t count;
        public qdb_ts_double_point result;
    };
}
