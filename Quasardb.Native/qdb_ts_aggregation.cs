using System;
using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

namespace Quasardb.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct qdb_ts_aggregation
    {
        public qdb_ts_aggregation_type type;
        public qdb_ts_range range;
        public qdb_ts_double_point result;
    };
}
