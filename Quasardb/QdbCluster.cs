using System;
using Quasardb.Exceptions;
using Quasardb.Interop;

namespace Quasardb
{
    public sealed class QdbCluster : IDisposable
    {
        readonly qdb_handle _handle;

        public QdbCluster(string uri)
        {
            if (uri == null) throw new ArgumentNullException("uri");

            _handle = qdb_api.qdb_open_tcp();

            var error = qdb_api.qdb_connect(_handle, uri);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }

        public void Dispose()
        {
            _handle.Dispose();
        }

        public QdbBlob Blob(string alias)
        {
            if (alias == null) throw new ArgumentNullException("alias");
            return new QdbBlob(_handle, alias);
        }

        public QdbInteger Integer(string alias)
        {
            if (alias == null) throw new ArgumentNullException("alias");
            return new QdbInteger(_handle, alias);
        }
    }
}
