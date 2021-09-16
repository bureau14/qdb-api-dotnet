using System;
using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

using qdb_size_t = System.UIntPtr;

namespace Quasardb.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct qdb_sized_string
    {
        internal Char* data;
        internal qdb_size_t length;

        public override string ToString()
        {
            if (length.ToUInt64() == 0UL) return string.Empty;
            // TODO: limited to 32-bit
            // TODO: should be UTF8, not ANSI
            return Marshal.PtrToStringAnsi(new IntPtr(data), (int)length);
        }

        public static implicit operator string(qdb_sized_string s)
        {
            return s.ToString();
        }
    }
}
