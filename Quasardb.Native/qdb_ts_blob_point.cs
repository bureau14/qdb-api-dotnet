using System;
using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

using qdb_size_t = System.UIntPtr;

namespace Quasardb.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct qdb_ts_blob_point
    {
        public qdb_timespec timestamp;
        public void* content;
        public qdb_size_t content_size;
    };
}
