using Quasardb.Exceptions;
using Quasardb.NativeApi;

namespace Quasardb.ManagedApi
{
    static class QdbExceptionThrower
    {
        public static void ThrowIfNeeded(qdb_error_t error)
        {
            if (error == qdb_error_t.qdb_e_ok) return;

            throw QdbExceptionFactory.Create(error);
        }
    }
}
