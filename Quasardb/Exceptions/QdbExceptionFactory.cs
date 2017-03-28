using Quasardb.Native;

namespace Quasardb.Exceptions
{
    static class QdbExceptionFactory
    {
        public static QdbException Create(qdb_error error, string alias = null, string column = null)
        {
            var origin = (qdb_err_origin) ((uint) error & (uint) qdb_err_origin.mask);

            switch (origin)
            {
                case qdb_err_origin.connection:
                    return CreateConnectionException(error);

                case qdb_err_origin.input:
                    return CreateInputException(error);

                case qdb_err_origin.operation:
                    return CreateOperationException(error, alias, column);

                case qdb_err_origin.protocol:
                    return CreateProtocolException(error);

                case qdb_err_origin.system_local:
                    return CreateLocalSystemException(error);

                case qdb_err_origin.system_remote:
                    return CreateRemoteSystemException(error);

                default:
                    return new QdbException(qdb_api.qdb_error(error));
            }
        }

        private static QdbConnectionException CreateConnectionException(qdb_error error)
        {
            return new QdbConnectionException(qdb_api.qdb_error(error));
        }

        private static QdbInputException CreateInputException(qdb_error error)
        {
            switch (error)
            {
                case qdb_error.qdb_e_invalid_argument:
                    return new QdbInvalidArgumentException();

                case qdb_error.qdb_e_out_of_bounds:
                    return new QdbOutOfBoundsException();

                default:
                    return new QdbInputException(qdb_api.qdb_error(error));
            }
        }

        static QdbOperationException CreateOperationException(qdb_error error, string alias, string column)
        {
            switch (error)
            {
                case qdb_error.qdb_e_alias_already_exists:
                    return new QdbAliasAlreadyExistsException(alias);

                case qdb_error.qdb_e_alias_not_found:
                    return new QdbAliasNotFoundException(alias);

                case qdb_error.qdb_e_incompatible_type:
                    return new QdbIncompatibleTypeException(alias);

                case qdb_error.qdb_e_resource_locked:
                    return new QdbResourceLockedException(alias);

                case qdb_error.qdb_e_column_not_found:
                    return new QdbColumnNotFoundException(alias, column);

                default:
                    return new QdbOperationException(qdb_api.qdb_error(error), alias);
            }
        }

        private static QdbLocalSystemException CreateLocalSystemException(qdb_error error)
        {
            return new QdbLocalSystemException(qdb_api.qdb_error(error));
        }

        private static QdbProtocolException CreateProtocolException(qdb_error error)
        {
            return new QdbProtocolException(qdb_api.qdb_error(error));
        }

        private static QdbRemoteSystemException CreateRemoteSystemException(qdb_error error)
        {
            return new QdbRemoteSystemException(qdb_api.qdb_error(error));
        }
    }
}
