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

        public byte[] Back()
        {
            qdb_buffer content;
            long contentLength;
            var error = qdb_api.qdb_queue_back(_handle, _alias, out content, out contentLength);
            QdbExceptionThrower.ThrowIfNeeded(error);
            return content.Copy(contentLength);
        }

        public byte[] Front()
        {
            qdb_buffer content;
            long contentLength;
            var error = qdb_api.qdb_queue_front(_handle, _alias, out content, out contentLength);
            QdbExceptionThrower.ThrowIfNeeded(error);
            return content.Copy(contentLength);
        }

        public byte[] PopBack()
        {
            qdb_buffer content;
            long contentLength;
            var error = qdb_api.qdb_queue_pop_back(_handle, _alias, out content, out contentLength);
            QdbExceptionThrower.ThrowIfNeeded(error);
            return content.Copy(contentLength);
        }

        public byte[] PopFront()
        {
            qdb_buffer content;
            long contentLength;
            var error = qdb_api.qdb_queue_pop_front(_handle, _alias, out content, out contentLength);
            QdbExceptionThrower.ThrowIfNeeded(error);
            return content.Copy(contentLength);
        }

        public void PushBack(byte[] content)
        {
            if (content == null) throw new ArgumentNullException("content");

            var error = qdb_api.qdb_queue_push_back(_handle, _alias, content, content.LongLength);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }

        public void PushFront(byte[] content)
        {
            if (content == null) throw new ArgumentNullException("content");

            var error = qdb_api.qdb_queue_push_front(_handle, _alias, content, content.LongLength);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }
    }
}
