using System.Collections;
using System.Runtime.InteropServices;
using Quasardb.Exceptions;
using Quasardb.Interop;

namespace Quasardb
{
    public class QdbBlob
    {
        readonly qdb_handle _handle;
        readonly string _alias;

        public QdbBlob(qdb_handle handle, string alias)
        {
            _handle = handle;
            _alias = alias;
        }

        public string Alias
        {
            get { return _alias; }
        }

        public void Put(byte[] content)
        {
            var error = qdb_api.qdb_put(_handle, _alias, content, content.Length, 0);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }

        public byte[] Get()
        {
            qdb_buffer content;
            long contentLength;
            var error = qdb_api.qdb_get(_handle, _alias, out content, out contentLength);
            QdbExceptionThrower.ThrowIfNeeded(error);
            return content.Copy(contentLength);
        }
    }
}
