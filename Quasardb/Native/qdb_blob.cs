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

        public qdb_blob(byte* c, qdb_size_t l)
        {
            content = c;
            content_size = l;
        }

        internal static qdb_blob Null => new qdb_blob { content = null, content_size = (qdb_size_t)0 };
    }
}
