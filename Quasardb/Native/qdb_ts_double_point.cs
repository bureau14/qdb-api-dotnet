using System;
using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

namespace Quasardb.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct qdb_ts_double_point
    {
        internal qdb_timespec timestamp;
        internal double value;
    };
}
