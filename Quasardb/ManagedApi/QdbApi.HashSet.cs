using System;
using Quasardb.Exceptions;
using Quasardb.NativeApi;

namespace Quasardb.ManagedApi
{
    partial class QdbApi
    {
        public bool HashSetInsert(string alias, byte[] content)
        {
            var error = qdb_api.qdb_hset_insert(_handle, alias, content, (UIntPtr)content.LongLength);

            switch (error)
            {
                case qdb_error_t.qdb_e_element_already_exists:
                    return false;

                case qdb_error_t.qdb_e_ok:
                    return true;

                default:
                    throw QdbExceptionFactory.Create(error);
            }
        }

        public bool HashSetErase(string alias, byte[] content)
        {
            var error = qdb_api.qdb_hset_erase(_handle, alias, content, (UIntPtr)content.LongLength);

            switch (error)
            {
                case qdb_error_t.qdb_e_element_not_found:
                    return false;

                case qdb_error_t.qdb_e_ok:
                    return true;

                default:
                    throw QdbExceptionFactory.Create(error);
            }
        }

        public bool HashSetContains(string alias, byte[] content)
        {
            var error = qdb_api.qdb_hset_contains(_handle, alias, content, (UIntPtr)content.LongLength);

            switch (error)
            {
                case qdb_error_t.qdb_e_element_not_found:
                    return false;

                case qdb_error_t.qdb_e_ok:
                    return true;

                default:
                    throw QdbExceptionFactory.Create(error);
            }
        }
    }
}
