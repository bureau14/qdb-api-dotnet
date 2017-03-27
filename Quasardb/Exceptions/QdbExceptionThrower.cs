using Quasardb.Native;

namespace Quasardb.Exceptions
{
    static class QdbExceptionThrower
    {
        public static void ThrowIfNeeded(qdb_error_t error, string alias = null, string column = null)
        {
            // TODO: temporary workaround until qdb_e_column_not_found is added
            if (error == qdb_error_t.qdb_e_element_not_found && column != null)
                throw new QdbColumnNotFoundException(alias, column);

            var severity = (qdb_err_severity)((uint)error & (uint)qdb_err_severity.mask);

            switch (severity)
            {
                case qdb_err_severity.warning:
                case qdb_err_severity.error:
                case qdb_err_severity.unrecoverable:
                    throw QdbExceptionFactory.Create(error, alias);
            }
        }
    }
}
