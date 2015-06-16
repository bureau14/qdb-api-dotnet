using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Quasardb.Exceptions;
using Quasardb.Interop;

namespace Quasardb
{
    public class QdbTag : QdbEntry
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
    }
}