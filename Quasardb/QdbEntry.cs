using Quasardb.Exceptions;
using Quasardb.Interop;

namespace Quasardb
{
    public class QdbEntry
    {
        protected qdb_handle _handle;
        protected string _alias;

        public QdbEntry(qdb_handle handle, string alias)
        {
            _handle = handle;
            _alias = alias;
        }

        public string Alias
        {
            get { return _alias; }
        }

        public void Remove()
        {
            var error = qdb_api.qdb_remove(_handle, _alias);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }
    }
}