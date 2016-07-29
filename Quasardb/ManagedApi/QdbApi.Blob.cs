using System;
using Quasardb.Exceptions;
using Quasardb.NativeApi;

namespace Quasardb.ManagedApi
{
    partial class QdbApi
    {
        public byte[] BlobCompareAndSwap(string alias, byte[] content, byte[] comparand, DateTime? expiryTime)
        {
            using (var oldContent = new qdb_buffer(_handle))
            {
                var error = qdb_api.qdb_blob_compare_and_swap(_handle, alias,
                    content, (UIntPtr)content.LongLength,
                    comparand, (UIntPtr)comparand.LongLength,
                    qdb_time.FromOptionalDateTime(expiryTime),
                    out oldContent.Pointer, out oldContent.Size);

                switch (error)
                {
                    case qdb_error_t.qdb_e_ok:
                        return null;

                    case qdb_error_t.qdb_e_unmatched_content:
                        return oldContent.GetBytes();

                    default:
                        throw QdbExceptionFactory.Create(error);
                }
            }
        }

        public byte[] BlobGet(string alias)
        {
            using (var content = new qdb_buffer(_handle))
            {
                var error = qdb_api.qdb_blob_get(_handle, alias, out content.Pointer, out content.Size);
                QdbExceptionThrower.ThrowIfNeeded(error);
                return content.GetBytes();
            }
        }

        public byte[] BlobGetAndRemove(string alias)
        {
            using (var content = new qdb_buffer(_handle))
            {
                var error = qdb_api.qdb_blob_get_and_remove(_handle, alias, out content.Pointer, out content.Size);
                QdbExceptionThrower.ThrowIfNeeded(error);
                return content.GetBytes();
            }
        }

        public byte[] BlobGetAndUpdate(string alias, byte[] content, DateTime? expiryTime)
        {
            using (var oldContent = new qdb_buffer(_handle))
            {
                var error = qdb_api.qdb_blob_get_and_update(_handle, alias,
                    content, (UIntPtr)content.LongLength,
                    qdb_time.FromOptionalDateTime(expiryTime),
                    out oldContent.Pointer, out oldContent.Size);

                QdbExceptionThrower.ThrowIfNeeded(error);
                return oldContent.GetBytes();
            }
        }

        public void BlobPut(string alias, byte[] content, DateTime? expiryTime)
        {
            var error = qdb_api.qdb_blob_put(_handle, alias,
                content, (UIntPtr)content.LongLength,
                qdb_time.FromOptionalDateTime(expiryTime));

            QdbExceptionThrower.ThrowIfNeeded(error);
        }

        public bool BlobRemoveIf(string alias, byte[] comparand)
        {
            var error = qdb_api.qdb_blob_remove_if(_handle, alias, comparand, (UIntPtr)comparand.Length);

            switch (error)
            {
                case qdb_error_t.qdb_e_unmatched_content:
                    return false;

                case qdb_error_t.qdb_e_ok:
                    return true;

                default:
                    throw QdbExceptionFactory.Create(error);
            }
        }

        public void BlobUpdate(string alias, byte[] content, DateTime? expiryTime)
        {
            var error = qdb_api.qdb_blob_update(_handle, alias,
                content, (UIntPtr)content.Length,
                qdb_time.FromOptionalDateTime(expiryTime));

            QdbExceptionThrower.ThrowIfNeeded(error);
        }
    }
}
