using System;
using Quasardb.Exceptions;
using Quasardb.Interop;

namespace Quasardb
{
    public class QdbHashSet : QdbEntry
    {
        public QdbHashSet(qdb_handle handle, string alias) : base(handle, alias)
        {
        }

        public bool Insert(byte[] content)
        {
            if (content == null) throw new ArgumentNullException("content");

            var error = qdb_api.qdb_hset_insert(_handle, _alias, content, content.LongLength);
            if (error == qdb_error.qdb_e_element_already_exists) return false;
            QdbExceptionThrower.ThrowIfNeeded(error);
            return true;
        }

        public bool Erase(byte[] content)
        {
            if (content == null) throw new ArgumentNullException("content");

            var error = qdb_api.qdb_hset_erase(_handle, _alias, content, content.LongLength);
            if (error == qdb_error.qdb_e_element_not_found) return false;
            QdbExceptionThrower.ThrowIfNeeded(error);
            return true;
        }
    }
}
