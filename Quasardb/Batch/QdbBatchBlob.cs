using System;
using System.Runtime.InteropServices;
using Quasardb.Native;

namespace Quasardb
{
    /// <summary>
    /// Adds blob operations to a batch
    /// </summary>
    public sealed class QdbBatchBlob : QdbBatchEntry
    {
        internal QdbBatchBlob(QdbBatch batch, string alias) : base(batch, alias)
        {
        }

        /// <summary>
        /// Adds a "get" operation to the batch: "Read the content of the blob."
        /// </summary>
        /// <returns>A future that will contain the result of the operation after the batch is run.</returns>
        public IQdbFuture<byte[]> Get()
        {
            return AddOperation(
                (ref qdb_operation op) =>
                {
                    op.type = qdb_operation_type.qdb_op_blob_get;
                    op.alias = Alias;
                },
                (ref qdb_operation op) =>
                {
                    return Helper.GetBytes(op.args.blob_get.content, op.args.blob_get.content_size);
                });
        }

        /// <summary>
        /// Adds a "Put" operation to the batch: "Create a new blob with the specified content. Fails if the blob already exists."
        /// </summary>
        /// <param name="content">The content of the blob to be created.</param>
        /// <param name="expiryTime">The expiry time of the blob.</param>
        /// <returns>A future that will contain the result of the operation after the batch is run.</returns>
        /// <seealso cref="QdbBlob.Put"/>
        public IQdbFuture Put(byte[] content, DateTime? expiryTime = null)
        {
            return PutOrUpdate(qdb_operation_type.qdb_op_blob_put, content, expiryTime);
        }

        private IQdbFuture PutOrUpdate(qdb_operation_type type, byte[] content, DateTime? expiryTime)
        {
            var pin = GCHandle.Alloc(content, GCHandleType.Pinned);

            return AddOperation(
                (ref qdb_operation op) =>
                {
                    op.type = type;
                    op.alias = Alias;
                    op.args.blob_set.content = pin.AddrOfPinnedObject();
                    op.args.blob_set.content_size = (UIntPtr) content.Length;
                    op.args.blob_set.expiry = qdb_time.FromOptionalDateTime(expiryTime);
                },
                (ref qdb_operation op) =>
                {
                    pin.Free();
                });
        }


        /// <summary>
        /// Adds an "update" operation to the batch: "Replaces the content of the blob."
        /// </summary>
        /// <param name="content">The content of the blob to be set.</param>
        /// <param name="expiryTime">The new expiry time of the blob.</param>
        /// <returns>A future that will contain the result of the operation after the batch is run.</returns>
        public IQdbFuture Update(byte[] content, DateTime? expiryTime = null)
        {
            return PutOrUpdate(qdb_operation_type.qdb_op_blob_update, content, expiryTime);
        }

        /// <summary>
        /// Adds a "compareAndSwap" operation to the batch: "Atomically compares the content of the blob and replaces it, if it matches."
        /// </summary>
        /// <param name="newContent">The content to be updated to the server, in case of match.</param>
        /// <param name="comparand">The content to be compared to.</param>
        /// <param name="expiryTime">The new expiry time of the blob, in case of match.</param>
        /// <returns>A future that will contain the result of the operation after the batch is run.</returns>
        /// <seealso cref="QdbBlob.CompareAndSwap"/>
        public IQdbFuture<byte[]> CompareAndSwap(byte[] newContent, byte[] comparand, DateTime? expiryTime = null)
        {
            var newContentPin = GCHandle.Alloc(newContent, GCHandleType.Pinned);
            var comparandPin = GCHandle.Alloc(comparand, GCHandleType.Pinned);

            return AddOperation(
                (ref qdb_operation op) =>
                {
                    op.type = qdb_operation_type.qdb_op_blob_cas;
                    op.alias = Alias;
                    op.args.blob_cas.new_content = newContentPin.AddrOfPinnedObject();
                    op.args.blob_cas.new_content_size = (UIntPtr)newContent.Length;
                    op.args.blob_cas.comparand = comparandPin.AddrOfPinnedObject();
                    op.args.blob_cas.comparand_size = (UIntPtr)comparand.Length;
                    op.args.blob_cas.expiry = qdb_time.FromOptionalDateTime(expiryTime);
                },
                (ref qdb_operation op) =>
                {
                    newContentPin.Free();
                    comparandPin.Free();

                    return Helper.GetBytes(op.args.blob_cas.original_content, op.args.blob_cas.original_content_size);
                });
        }

        /// <summary>
        /// Adds a "getAndUpdate" operation to the batch: "Atomically reads and replaces (in this order) the content of blob."
        /// </summary>
        /// <param name="newContent">The content of the blob to be set, before being replaced.</param>
        /// <param name="expiryTime">The new expiry time of the blob.</param>
        /// <returns>A future that will contain the result of the operation after the batch is run.</returns>
        public IQdbFuture<byte[]> GetAndUpdate(byte[] newContent, DateTime? expiryTime = null)
        {
            var newContentPin = GCHandle.Alloc(newContent, GCHandleType.Pinned);

            return AddOperation(
                (ref qdb_operation op) =>
                {
                    op.type = qdb_operation_type.qdb_op_blob_get_and_update;
                    op.alias = Alias;
                    op.args.blob_get_and_update.new_content = newContentPin.AddrOfPinnedObject();
                    op.args.blob_get_and_update.new_content_size = (UIntPtr)newContent.Length;
                    op.args.blob_get_and_update.expiry = qdb_time.FromOptionalDateTime(expiryTime);
                },
                (ref qdb_operation op) =>
                {
                    newContentPin.Free();

                    return Helper.GetBytes(op.args.blob_get_and_update.original_content, op.args.blob_get_and_update.original_content_size);
                });
        }
    }
}
