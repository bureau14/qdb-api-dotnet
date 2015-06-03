using Quasardb.Exceptions;
using Quasardb.Interop;

namespace Quasardb
{
    public class QdbInteger : QdbExpirableEntry
    {
        public QdbInteger(qdb_handle handle, string alias) : base(handle, alias)
        {
        }

        public long Get()
        {
            long value;
            var error = qdb_api.qdb_int_get(_handle, _alias, out value);
            QdbExceptionThrower.ThrowIfNeeded(error);
            return value;
        }
    }
}
