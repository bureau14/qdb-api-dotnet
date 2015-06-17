using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Quasardb.Exceptions;
using Quasardb.Internals;
using Quasardb.Interop;

namespace Quasardb
{
    public sealed class QdbTag : QdbEntry
    {
        internal QdbTag(qdb_handle handle, string alias) : base(handle, alias)
        {
        }

        public IEnumerable<QdbEntry> GetEntries()
        {
            var entryCollection = new QdbEntryCollection(Handle);
            
            var error = qdb_api.qdb_get_tagged(Handle, Alias, out entryCollection.Pointer, out entryCollection.Size);
            QdbExceptionThrower.ThrowIfNeeded(error);

            return entryCollection;
        }

        public bool AddEntry(QdbEntry entry)
        {
            return QdbTagHelper.AddTag(Handle, entry.Alias, Alias);
        }

        public bool AddEntry(string entry)
        {
            return QdbTagHelper.AddTag(Handle, entry, Alias);
        }

        public bool RemoveEntry(QdbEntry entry)
        {
            return QdbTagHelper.RemoveTag(Handle, entry.Alias, Alias);
        }

        public bool RemoveEntry(string entry)
        {
            return QdbTagHelper.RemoveTag(Handle, entry, Alias);
        }
    }
}