using System;
using System.Runtime.InteropServices;

using qdb_size_t = System.UIntPtr;

// ReSharper disable InconsistentNaming

namespace Quasardb.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct qdb_blob
    {
        internal byte* content;
        internal qdb_size_t content_size;

        public qdb_blob(byte[] arr, ref GCHandle pin)
        {
            pin = GCHandle.Alloc(arr, GCHandleType.Pinned);
            content = (byte*)pin.AddrOfPinnedObject();
            content_size = (qdb_size_t)arr.Length;
        }
    }
}
