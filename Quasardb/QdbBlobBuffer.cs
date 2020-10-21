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
            return Helper.GetBytes((int)Size, Pointer);
        }
    }
}
