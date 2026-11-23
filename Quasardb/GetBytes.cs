using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Quasardb
{
    /// <summary>
    /// Helper class
    /// </summary>
    public static class Helper
    {
        internal static byte[] GetNullTerminatedUtf8Bytes(string value)
        {
            var bytes = new byte[Encoding.UTF8.GetByteCount(value) + 1];
            Encoding.UTF8.GetBytes(value, 0, value.Length, bytes, 0);
            return bytes;
        }

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
