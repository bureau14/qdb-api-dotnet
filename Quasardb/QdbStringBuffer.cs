using System.Text;
using Quasardb.Native;

namespace Quasardb
{
    sealed class QdbStringBuffer : qdb_buffer
    {
        public QdbStringBuffer(qdb_handle handle) : base(handle)
        {
        }

        public string GetString()
        {
            var bytes = Helper.GetBytes(Pointer, Size);
            return bytes == null ? null : Encoding.UTF8.GetString(bytes);
        }
    }
}