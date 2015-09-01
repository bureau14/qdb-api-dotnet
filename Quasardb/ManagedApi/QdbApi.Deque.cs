using System;
using Quasardb.NativeApi;

namespace Quasardb.ManagedApi
{
    partial class QdbApi
    {
        public byte[] DequeBack(string alias)
        {
            using (var content = new qdb_buffer(_handle))
            {
                var error = qdb_api.qdb_deque_back(_handle, alias, out content.Pointer, out content.Size);
                QdbExceptionThrower.ThrowIfNeeded(error);
                return content.GetBytes();
            }
        }

        public byte[] DequeFront(string alias)
        {
            using (var content = new qdb_buffer(_handle))
            {
                var error = qdb_api.qdb_deque_front(_handle, alias, out content.Pointer, out content.Size);
                QdbExceptionThrower.ThrowIfNeeded(error);
                return content.GetBytes();
            }
        }

        public byte[] DequePopBack(string alias)
        {
            using (var content = new qdb_buffer(_handle))
            {
                var error = qdb_api.qdb_deque_pop_back(_handle, alias, out content.Pointer, out content.Size);
                QdbExceptionThrower.ThrowIfNeeded(error);
                return content.GetBytes();
            }
        }

        public byte[] DequePopFront(string alias)
        {
            using (var content = new qdb_buffer(_handle))
            {
                var error = qdb_api.qdb_deque_pop_front(_handle, alias, out content.Pointer, out content.Size);
                QdbExceptionThrower.ThrowIfNeeded(error);
                return content.GetBytes();
            }
        }

        public void DequePushBack(string alias, byte[] content)
        {
            var error = qdb_api.qdb_deque_push_back(_handle, alias, content, (IntPtr)content.LongLength);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }

        public void DequePushFront(string alias, byte[] content)
        {
            var error = qdb_api.qdb_deque_push_front(_handle, alias, content, (IntPtr)content.LongLength);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }
    }
}
