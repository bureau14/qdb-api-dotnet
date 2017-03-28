using Quasardb.Native;

namespace Quasardb.Exceptions
{
    static class QdbExceptionThrower
    {
        public static void ThrowIfNeeded(qdb_error error, string alias = null, string column = null)
        {
            var severity = (qdb_err_severity)((uint)error & (uint)qdb_err_severity.mask);

            switch (severity)
            {
                case qdb_err_severity.warning:
                case qdb_err_severity.error:
                case qdb_err_severity.unrecoverable:
                    throw QdbExceptionFactory.Create(error, alias, column);
            }
        }
    }
}
