using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Quasardb.NativeApi
{
    unsafe struct qdb_string
    {
        public byte* Chars;
        
        public override string ToString()
        {
            // TODO: should be UTF8, not ANSI
            return Marshal.PtrToStringAnsi(new IntPtr(Chars));
        }

        public static implicit operator string(qdb_string s)
        {
            return s.ToString();
        }
    }
}
