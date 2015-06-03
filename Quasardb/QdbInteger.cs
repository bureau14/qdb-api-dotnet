using Quasardb.Exceptions;
using Quasardb.Interop;

namespace Quasardb
{
    public sealed class QdbInteger : QdbExpirableEntry
    {
        internal QdbInteger(qdb_handle handle, string alias) : base(handle, alias)
        {
        }

        public long Add(long addend)
        {
            long result;
            var error = qdb_api.qdb_int_add(m_handle, m_alias, addend, out result);
            QdbExceptionThrower.ThrowIfNeeded(error);
            return result;
        }

        public long Get()
        {
            long value;
            var error = qdb_api.qdb_int_get(m_handle, m_alias, out value);
            QdbExceptionThrower.ThrowIfNeeded(error);
            return value;
        }

        public void Put(long value)
        {
            var error = qdb_api.qdb_int_put(m_handle, m_alias, value);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }

        public void Update(long value)
        {
            var error = qdb_api.qdb_int_update(m_handle, m_alias, value);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }
    }
}
