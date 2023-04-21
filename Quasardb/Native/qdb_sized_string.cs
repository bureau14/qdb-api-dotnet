using System;
using System.Text;
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

        public qdb_sized_string(byte * d, qdb_size_t l)
        {
            data = d;
            length = l;
        }

        public override string ToString()
        {
            if (length.ToUInt64() == 0UL) return string.Empty;
            if (length.ToUInt64() > int.MaxValue) throw new ArgumentException($"String is too long {length}");
            // Allocates a managed String, copies a specified number of characters from an unmanaged ANSI or UTF-8 string into it,
            // and widens each character to UTF-16.
            return Marshal.PtrToStringAnsi(new IntPtr(data), (int)length);
        }

        public static implicit operator string(qdb_sized_string s)
        {
            return s.ToString();
        }

        internal static qdb_sized_string Null => new qdb_sized_string { data = null, length = (qdb_size_t)0 };
    }
}
