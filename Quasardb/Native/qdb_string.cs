using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Quasardb.Native
{
    internal unsafe struct qdb_string
    {
        internal byte* data;

        public override string ToString()
        {
            if (data == null) return String.Empty;
            // Copies all characters up to the first null character from an unmanaged ANSI or UTF-8 string to a managed String,
            // and widens each character to UTF-16.
            return Marshal.PtrToStringAnsi(new IntPtr(data));
        }

        public static implicit operator string(qdb_string s)
        {
            return s.ToString();
        }
    }
}
