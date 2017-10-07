using System.Runtime.InteropServices;
using Quasardb.Native;

namespace Quasardb
{
    sealed class QdbBlobBuffer : qdb_buffer
    {
        public QdbBlobBuffer(qdb_handle handle) : base(handle)
        {
        }

        public byte[] GetBytes()
        {
            // CAUTION: limited to 32 bits!!!
            int size = (int)Size;
            var buffer = new byte[size];
            Marshal.Copy(Pointer, buffer, 0, size);
            return buffer;
        }
    }
}
