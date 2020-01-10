using Quasardb.Exceptions;
using Quasardb.Native;
using Quasardb.TimeSeries;

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
            QdbExceptionThrower.ThrowIfNeeded(error, alias: alias);
            return Create(type, alias);
        }

        QdbEntry Create(qdb_entry_type type, string alias)
        {
            switch (type)
            {
                case qdb_entry_type.qdb_entry_blob:
                    return new QdbBlob(_handle, alias);

                case qdb_entry_type.qdb_entry_integer:
                    return new QdbInteger(_handle, alias);

                case qdb_entry_type.qdb_entry_tag:
                    return new QdbTag(_handle, alias);

                case qdb_entry_type.qdb_entry_ts:
                    return new QdbTable(_handle, alias);

                default:
                    return new QdbUnknownEntry(_handle, alias, type);
            }
        }
    }
}
