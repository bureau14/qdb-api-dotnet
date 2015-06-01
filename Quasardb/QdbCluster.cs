using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quasardb.Exceptions;
using Quasardb.Interop;

namespace Quasardb
{
    public sealed class QdbCluster : IDisposable
    {
        readonly qdb_handle _handle;

        public QdbCluster(string uri)
        {
            _handle = qdb_api.qdb_open_tcp();

            var error = qdb_api.qdb_connect(_handle, uri);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }

        public void Dispose()
        {
            _handle.Dispose();
        }
    }
}
