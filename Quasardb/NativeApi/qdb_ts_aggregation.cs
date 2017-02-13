using System;
using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

namespace Quasardb.NativeApi
{
    [StructLayout(LayoutKind.Sequential)]
    struct qdb_ts_aggregation
    {
        public qdb_timespec begin;
        public qdb_timespec end;

        public qdb_timespec result_timestamp;
        public double result_value;
    };
}
