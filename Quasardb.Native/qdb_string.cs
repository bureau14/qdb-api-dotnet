using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Quasardb.Native
{
    public unsafe struct qdb_string
    {
        public byte* Chars;

        public override string ToString()
        {
            if (Chars == null) return String.Empty;
            // TODO: should be UTF8, not ANSI
            return Marshal.PtrToStringAnsi(new IntPtr(Chars));
        }

        public static implicit operator string(qdb_string s)
        {
            return s.ToString();
        }
    }
}
