﻿using Quasardb.NativeApi;

namespace Quasardb.Exceptions
{
    static class QdbExceptionFactory
    {
        public static QdbException Create(qdb_error_t error)
        {
            var origin = (qdb_err_origin_t) ((uint) error & (uint) qdb_err_origin_t.qdb_e_origin_mask);

            switch (origin)
            {
                case qdb_err_origin_t.qdb_e_origin_connection:
                    return CreateConnectionException(error);

                case qdb_err_origin_t.qdb_e_origin_input:
                    return CreateInputException(error);

                case qdb_err_origin_t.qdb_e_origin_operation:
                    return CreateOperationException(error);

                case qdb_err_origin_t.qdb_e_origin_protocol:
                    return CreateProtocolException(error);

                case qdb_err_origin_t.qdb_e_origin_system_local:
                    return CreateLocalSystemException(error);

                case qdb_err_origin_t.qdb_e_origin_system_remote:
                    return CreateRemoteSystemException(error);

                default:
                    return new QdbException(qdb_api.qdb_error(error));
            }
        }

        private static QdbConnectionException CreateConnectionException(qdb_error_t error)
        {
            return new QdbConnectionException(qdb_api.qdb_error(error));
        }

        private static QdbInputException CreateInputException(qdb_error_t error)
        {
            switch (error)
            {
                case qdb_error_t.qdb_e_invalid_argument:
                    return new QdbInvalidArgumentException();

                default:
                    return new QdbInputException(qdb_api.qdb_error(error));
            }
        }

        private static QdbOperationException CreateOperationException(qdb_error_t error)
        {
            switch (error)
            {
                case qdb_error_t.qdb_e_alias_already_exists:
                    return new QdbAliasAlreadyExistsException();

                case qdb_error_t.qdb_e_alias_not_found:
                    return new QdbAliasNotFoundException();

                case qdb_error_t.qdb_e_incompatible_type:
                    return new QdbIncompatibleTypeException();

                case qdb_error_t.qdb_e_resource_locked:
                    return new QdbResourceLockedException();

                default:
                    return new QdbOperationException(qdb_api.qdb_error(error));
            }
        }

        private static QdbLocalSystemException CreateLocalSystemException(qdb_error_t error)
        {
            return new QdbLocalSystemException(qdb_api.qdb_error(error));
        }

        private static QdbProtocolException CreateProtocolException(qdb_error_t error)
        {
            return new QdbProtocolException(qdb_api.qdb_error(error));
        }

        private static QdbRemoteSystemException CreateRemoteSystemException(qdb_error_t error)
        {
            return new QdbRemoteSystemException(qdb_api.qdb_error(error));
        }
    }
}