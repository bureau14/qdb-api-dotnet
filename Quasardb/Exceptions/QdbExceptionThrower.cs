using Quasardb.Native;
using System;

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

        public static unsafe void ThrowIfNeededWithMsg(qdb_handle handle, qdb_error error, string alias = null, string column = null)
        {
            var severity = (qdb_err_severity)((uint)error & (uint)qdb_err_severity.mask);

            switch (severity)
            {
                case qdb_err_severity.warning:
                case qdb_err_severity.error:
                case qdb_err_severity.unrecoverable:
                    {
                        qdb_error err;
                        qdb_sized_string* message;
                        var ec = qdb_api.qdb_get_last_error(handle, out err, out message);
                        ThrowIfNeeded(ec);
                        var msg = $"{error}: {message->ToString()}.";
                        qdb_api.qdb_release(handle, (IntPtr)message);
                        throw new QdbException(msg);
                }
            }
        }
    }
}
