using System;
using Quasardb.ManagedApi;
using Quasardb.NativeApi;
using Quasardb.TimeSeries;

namespace Quasardb
{
    class QdbEntryFactory
    {
        readonly QdbApi _handle;

        public QdbEntryFactory(QdbApi handle)
        {
            _handle = handle;
        }

        public QdbEntry Create(string alias)
        {
            return Create(_handle.GetEntryType(alias), alias);
        }
        
        QdbEntry Create(qdb_entry_type type, string alias)
        {
            switch (type)
            {
                case qdb_entry_type.qdb_entry_blob:
                    return new QdbBlob(_handle, alias);

                case qdb_entry_type.qdb_entry_deque:
                    return new QdbDeque(_handle, alias);

                case qdb_entry_type.qdb_entry_hset:
                    return new QdbHashSet(_handle, alias);

                case qdb_entry_type.qdb_entry_integer:
                    return new QdbInteger(_handle, alias);

                case qdb_entry_type.qdb_entry_stream:
                    return new QdbStream(_handle, alias);

                case qdb_entry_type.qdb_entry_tag:
                    return new QdbTag(_handle, alias);

                case qdb_entry_type.qdb_entry_ts:
                    return new QdbTimeSeries(_handle, alias);

                default:
                    return new QdbUnknownEntry(_handle, alias);
            }
        }
    }

    class QdbUnknownEntry : QdbEntry
    {
        public QdbUnknownEntry(QdbApi api, string alias) : base(api, alias)
        {
        }
    }
}
