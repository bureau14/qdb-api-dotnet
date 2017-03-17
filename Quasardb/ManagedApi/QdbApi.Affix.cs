using Quasardb.Exceptions;
using Quasardb.Native;

namespace Quasardb.ManagedApi
{
    partial class QdbApi
    {
        public unsafe QdbStringCollection PrefixGet(string prefix, long max)
        {
            var result = new QdbStringCollection(_handle);

            var error = qdb_api.qdb_prefix_get(_handle, prefix, max, out result.Pointer, out result.Size);

            switch (error)
            {
                case qdb_error_t.qdb_e_ok:
                case qdb_error_t.qdb_e_alias_not_found:
                    return result;

                default:
                    throw QdbExceptionFactory.Create(error);
            }
        }

        public unsafe QdbStringCollection SuffixGet(string suffix, long max)
        {
            var result = new QdbStringCollection(_handle);

            var error = qdb_api.qdb_suffix_get(_handle, suffix, max, out result.Pointer, out result.Size);

            switch (error)
            {
                case qdb_error_t.qdb_e_ok:
                case qdb_error_t.qdb_e_alias_not_found:
                    return result;

                default:
                    throw QdbExceptionFactory.Create(error);
            }
        }
    }
}
