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
        internal byte* data;
        internal qdb_size_t length;

        public override string ToString()
        {
            if (length.ToUInt64() == 0UL) return string.Empty;
            // Allocates a managed String, copies a specified number of characters from an unmanaged ANSI or UTF-8 string into it,
            // and widens each character to UTF-16.
            return Marshal.PtrToStringAnsi(new IntPtr(data), (int)length);
        }

        public static implicit operator string(qdb_sized_string s)
        {
            return s.ToString();
        }
    }
}
