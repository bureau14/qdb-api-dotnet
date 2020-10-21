using System;
using System.Runtime.InteropServices;

namespace Quasardb
{
    /// <summary>
    /// Helper class
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Safely Get Bytes
        /// </summary>
        public static byte[] GetBytes(IntPtr Pointer, int size)
        {
            // CAUTION: limited to 32 bits!!!
            if (size == 0)
                return null;
            if (Pointer == null)
                return null;
            var buffer = new byte[size];
            Marshal.Copy(Pointer, buffer, 0, size);
            return buffer;
        }
    }

}
