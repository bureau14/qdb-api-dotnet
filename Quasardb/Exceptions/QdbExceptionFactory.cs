using Quasardb.NativeApi;

namespace Quasardb.Exceptions
{
    static class QdbExceptionFactory
    {
        public static QdbException Create(qdb_error_t error)
        {
            switch (error)
            {
                case qdb_error_t.qdb_e_alias_already_exists:
                    return new QdbAliasAlreadyExistsException();
                
                case qdb_error_t.qdb_e_alias_not_found:
                    return new QdbAliasNotFoundException();

                case qdb_error_t.qdb_e_incompatible_type:
                    return new QdbIncompatibleTypeException();

                case qdb_error_t.qdb_e_invalid_argument:
                    return new QdbInvalidArgumentException();

                case qdb_error_t.qdb_e_container_empty:
                    return new QdbEmptyContainerException();

                case qdb_error_t.qdb_e_resource_locked:
                    return new QdbResourceLockedException();

                default:
                    return new QdbException(qdb_api.qdb_error(error));
            }
        }
    }
}
