using Quasardb.Exceptions;
using Quasardb.Interop;

namespace Quasardb
{
    public class QdbEntry
    {
        protected qdb_handle m_handle;
        protected string m_alias;

        public QdbEntry(qdb_handle handle, string alias)
        {
            m_handle = handle;
            m_alias = alias;
        }

        public string Alias
        {
            get { return m_alias; }
        }

        public void Remove()
        {
            var error = qdb_api.qdb_remove(m_handle, m_alias);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }
    }
}