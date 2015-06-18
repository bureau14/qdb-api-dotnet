using System;
using Quasardb.NativeApi;

namespace Quasardb.ManagedApi
{
    partial class QdbApi
    {
        public byte[] QueueBack(string alias)
        {
            using (var content = new qdb_buffer(_handle))
            {
                var error = qdb_api.qdb_queue_back(_handle, alias, out content.Pointer, out content.Size);
                QdbExceptionThrower.ThrowIfNeeded(error);
                return content.GetBytes();
            }
        }

        public byte[] QueueFront(string alias)
        {
            using (var content = new qdb_buffer(_handle))
            {
                var error = qdb_api.qdb_queue_front(_handle, alias, out content.Pointer, out content.Size);
                QdbExceptionThrower.ThrowIfNeeded(error);
                return content.GetBytes();
            }
        }

        public byte[] QueuePopBack(string alias)
        {
            using (var content = new qdb_buffer(_handle))
            {
                var error = qdb_api.qdb_queue_pop_back(_handle, alias, out content.Pointer, out content.Size);
                QdbExceptionThrower.ThrowIfNeeded(error);
                return content.GetBytes();
            }
        }

        public byte[] QueuePopFront(string alias)
        {
            using (var content = new qdb_buffer(_handle))
            {
                var error = qdb_api.qdb_queue_pop_front(_handle, alias, out content.Pointer, out content.Size);
                QdbExceptionThrower.ThrowIfNeeded(error);
                return content.GetBytes();
            }
        }

        public void QueuePushBack(string alias, byte[] content)
        {
            var error = qdb_api.qdb_queue_push_back(_handle, alias, content, (IntPtr)content.LongLength);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }

        public void QueuePushFront(string alias, byte[] content)
        {
            var error = qdb_api.qdb_queue_push_front(_handle, alias, content, (IntPtr)content.LongLength);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }
    }
}
