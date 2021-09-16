using System;
using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

using qdb_size_t = System.UIntPtr;

namespace Quasardb.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct qdb_ts_symbol_point
    {
        internal qdb_timespec timestamp;
        internal char* content;
        internal qdb_size_t content_size;
    };
}
