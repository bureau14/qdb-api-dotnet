using System;
using Quasardb.Exceptions;
using Quasardb.Interop;

namespace Quasardb
{
    class QdbEntryFactory
    {
        readonly qdb_handle _handle;

        public QdbEntryFactory(qdb_handle handle)
        {
            _handle = handle;
        }

        public QdbEntry Create(string alias)
        {
            qdb_entry_type type;
            var error = qdb_api.qdb_get_type(_handle, alias, out type);
            QdbExceptionThrower.ThrowIfNeeded(error);

            return Create(type, alias);
        }

        QdbEntry Create(qdb_entry_type type, string alias)
        {
            switch (type)
            {
                case qdb_entry_type.qdb_entry_blob:
                    return new QdbBlob(_handle, alias);

                case qdb_entry_type.qdb_entry_queue:
                    return new QdbQueue(_handle, alias);

                case qdb_entry_type.qdb_entry_hset:
                    return new QdbHashSet(_handle, alias);

                case qdb_entry_type.qdb_entry_tag:
                    return new QdbTag(_handle, alias);

                case qdb_entry_type.qdb_entry_integer:
                    return new QdbInteger(_handle, alias);

                case qdb_entry_type.qdb_entry_uninitialized:
                    throw new QdbInternalException();

                default:
                    throw new NotSupportedException("Entry type " + type +
                                                    " is not supported, please upgrade quasardb .NET API");
            }
        }
    }
}
