using System;
using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

using qdb_size_t = System.UIntPtr;

namespace Quasardb.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct qdb_ts_symbol_point
    {
        public qdb_timespec timestamp;
        public char* content;
        public qdb_size_t content_size;
    };
}
