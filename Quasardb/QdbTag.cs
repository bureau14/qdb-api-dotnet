using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            return Enumerable.Empty<QdbEntry>();
        }
    }
}