using System.IO;
using Quasardb.ManagedApi;

namespace Quasardb
{
    public sealed class QdbStream : QdbEntry
    {
        internal QdbStream(QdbApi api, string alias) : base(api, alias)
        {
        }

        public Stream Open(FileMode mode)
        {
            return null;
        }
    }
}
