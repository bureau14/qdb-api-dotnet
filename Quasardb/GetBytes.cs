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
        public static byte[] GetBytes(IntPtr pointer, int size)
        {
            if (size == 0 || pointer == IntPtr.Zero)
                return null;

            var buffer = new byte[size];
            Marshal.Copy(pointer, buffer, 0, size);
            return buffer;
        }

        /// <summary>
        /// Safely get bytes from an unmanaged buffer using a <see cref="UIntPtr"/> size.
        /// </summary>
        [CLSCompliant(false)]
        public static byte[] GetBytes(IntPtr pointer, UIntPtr size)
        {
            long length = (long)size;

            if (length == 0 || pointer == IntPtr.Zero)
                return null;

            if (length > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(size), "Buffer too large.");

            return GetBytes(pointer, (int)length);
        }
    }
}
