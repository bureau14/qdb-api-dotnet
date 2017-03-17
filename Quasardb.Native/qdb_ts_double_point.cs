using System;
using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

using qdb_int_t = System.Int64;
using qdb_time_t = System.Int64;
using size_t = System.UIntPtr;
using qdb_size_t = System.UIntPtr;

namespace Quasardb.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct qdb_ts_double_point
    {
        public qdb_timespec timestamp;
        public double value;
    };
}
