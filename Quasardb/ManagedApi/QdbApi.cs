using System;
using System.Collections.Generic;
using Quasardb.Exceptions;
using Quasardb.NativeApi;

namespace Quasardb.ManagedApi
{
    partial class QdbApi : IDisposable
    {
        readonly qdb_handle _handle;

        public QdbApi()
        {
            _handle = qdb_api.qdb_open_tcp();
        }

        public void Connect(string uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            var error = qdb_api.qdb_connect(_handle, uri);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }

        public void Dispose()
        {
            _handle.Dispose();
        }

        public qdb_entry_type GetEntryType(string alias)
        {
            qdb_entry_type type;
            var error = qdb_api.qdb_get_type(_handle, alias, out type);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: alias);
            return type;
        }

        public bool Remove(string alias)
        {
            var error = qdb_api.qdb_remove(_handle, alias);

            switch (error)
            {
                case qdb_error_t.qdb_e_ok:
                    return true;
                    
                case qdb_error_t.qdb_e_alias_not_found:
                    return false;

                default:
                    throw QdbExceptionFactory.Create(error, alias: alias);
            }
        }
    }
}
