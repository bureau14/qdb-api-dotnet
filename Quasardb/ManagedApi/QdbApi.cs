using System;
using System.Collections.Generic;
using Quasardb.Exceptions;
using Quasardb.NativeApi;

namespace Quasardb.ManagedApi
{
    partial class QdbApi : IDisposable
    {
        static readonly Dictionary<qdb_entry_type, QdbEntryType> _typeMap = 
            new Dictionary <qdb_entry_type, QdbEntryType>
        {
            {qdb_entry_type.qdb_entry_blob, QdbEntryType.Blob},
            {qdb_entry_type.qdb_entry_hset, QdbEntryType.HashSet},
            {qdb_entry_type.qdb_entry_integer, QdbEntryType.Integer},
            {qdb_entry_type.qdb_entry_deque, QdbEntryType.Deque},
            {qdb_entry_type.qdb_entry_tag, QdbEntryType.Tag},
            {qdb_entry_type.qdb_entry_stream, QdbEntryType.Stream}
        };

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

        public QdbEntryType GetEntryType(string alias)
        {
            qdb_entry_type nativeType;
            var error = qdb_api.qdb_get_type(_handle, alias, out nativeType);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: alias);

            QdbEntryType managedType;
            if (!_typeMap.TryGetValue(nativeType, out managedType))
                throw new NotSupportedException("Unknown entry type " + nativeType +
                                                ", please upgrade Quasardb .NET API");

            return managedType;
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
