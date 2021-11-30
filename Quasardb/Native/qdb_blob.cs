using System;
using System.Runtime.InteropServices;

using qdb_size_t = System.UIntPtr;

// ReSharper disable InconsistentNaming

namespace Quasardb.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct qdb_blob
    {
        internal byte* content;
        internal qdb_size_t content_size;
    }
}
