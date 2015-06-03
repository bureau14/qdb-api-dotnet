using System;
using Quasardb.Exceptions;
using Quasardb.Interop;

namespace Quasardb
{
    public sealed class QdbQueue : QdbEntry
    {
        internal QdbQueue(qdb_handle handle, string alias) : base(handle, alias)
        {
        }

        public void PushFront(byte[] content)
        {
            if (content == null) throw new ArgumentNullException("content");

            var error = qdb_api.qdb_queue_push_front(_handle, _alias, content, content.LongLength);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }
    }
}
