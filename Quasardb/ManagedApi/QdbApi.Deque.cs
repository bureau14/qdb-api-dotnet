using System;
using Quasardb.Native;

namespace Quasardb.ManagedApi
{
    partial class QdbApi
    {
        public byte[] DequeBack(string alias)
        {
            using (var content = new qdb_buffer(_handle))
            {
                var error = qdb_api.qdb_deque_back(_handle, alias, out content.Pointer, out content.Size);
                if (error == qdb_error_t.qdb_e_container_empty) return null;
                QdbExceptionThrower.ThrowIfNeeded(error, alias: alias);
                return content.GetBytes();
            }
        }

        public byte[] DequeFront(string alias)
        {
            using (var content = new qdb_buffer(_handle))
            {
                var error = qdb_api.qdb_deque_front(_handle, alias, out content.Pointer, out content.Size);
                if (error == qdb_error_t.qdb_e_container_empty) return null;
                QdbExceptionThrower.ThrowIfNeeded(error, alias: alias);
                return content.GetBytes();
            }
        }

        public byte[] DequePopBack(string alias)
        {
            using (var content = new qdb_buffer(_handle))
            {
                var error = qdb_api.qdb_deque_pop_back(_handle, alias, out content.Pointer, out content.Size);
                if (error == qdb_error_t.qdb_e_container_empty) return null;
                QdbExceptionThrower.ThrowIfNeeded(error, alias: alias);
                return content.GetBytes();
            }
        }

        public byte[] DequePopFront(string alias)
        {
            using (var content = new qdb_buffer(_handle))
            {
                var error = qdb_api.qdb_deque_pop_front(_handle, alias, out content.Pointer, out content.Size);
                if (error == qdb_error_t.qdb_e_container_empty) return null;
                QdbExceptionThrower.ThrowIfNeeded(error, alias: alias);
                return content.GetBytes();
            }
        }

        public void DequePushBack(string alias, byte[] content)
        {
            var error = qdb_api.qdb_deque_push_back(_handle, alias, content, (UIntPtr)content.LongLength);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: alias);
        }

        public void DequePushFront(string alias, byte[] content)
        {
            var error = qdb_api.qdb_deque_push_front(_handle, alias, content, (UIntPtr)content.LongLength);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: alias);
        }

        public long DequeSize(string alias)
        {
            UIntPtr size;
            var error = qdb_api.qdb_deque_size(_handle, alias, out size);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: alias);
            return (long)size;
        }

        public byte[] DequeGetAt(string alias, long index)
        {
            using (var content = new qdb_buffer(_handle))
            {
                var error = qdb_api.qdb_deque_get_at(_handle, alias, index, out content.Pointer, out content.Size);
                QdbExceptionThrower.ThrowIfNeeded(error, alias: alias);
                return content.GetBytes();
            }
        }

        public void DequeSetAt(string alias, long index, byte[] content)
        {
            var error = qdb_api.qdb_deque_set_at(_handle, alias, index, content, (UIntPtr)content.LongLength);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: alias);
        }
    }
}
