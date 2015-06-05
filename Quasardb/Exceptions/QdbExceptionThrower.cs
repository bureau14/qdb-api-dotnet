using Quasardb.Interop;

namespace Quasardb.Exceptions
{
    static class QdbExceptionThrower
    {
        public static void ThrowIfNeeded(qdb_error error)
        {
            if (error == qdb_error.qdb_e_ok) return;

            throw QdbExceptionFactory.Create(error);
        }
    }
}
