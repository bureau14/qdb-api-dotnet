using System.Collections.Generic;
using Quasardb.Exceptions;
using Quasardb.NativeApi;

namespace Quasardb.ManagedApi
{
    partial class QdbApi
    {
        public IEnumerable<string> PrefixGet(string prefix, long max)
        {
            var result = new QdbAliasCollection(_handle);

            var error = qdb_api.qdb_prefix_get(_handle, prefix, max, out result.Pointer, out result.Size);
            QdbExceptionThrower.ThrowIfNeeded(error);

            return result;
        }

        public IEnumerable<string> SuffixGet(string suffix, long max)
        {
            var result = new QdbAliasCollection(_handle);

            var error = qdb_api.qdb_suffix_get(_handle, suffix, max, out result.Pointer, out result.Size);
            QdbExceptionThrower.ThrowIfNeeded(error);

            return result;
        }
    }
}
