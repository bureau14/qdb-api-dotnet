using System.Runtime.InteropServices;
using System.Text;
using Quasardb.ManagedApi;
using Quasardb.Native;

namespace Quasardb
{
    /// <summary>
    /// Adds operations to a batch
    /// </summary>
    public abstract class QdbBatchEntry
    {
        private readonly QdbBatch _batch;

        /// <summary>
        /// The entry's alias.
        /// </summary>
        protected readonly string Alias;

        internal QdbBatchEntry(QdbBatch batch, string alias)
        {
            _batch = batch;
            Alias = alias;
        }

        /// <summary>
        /// Adds a "HasTag" operation to the batch: "Checks if the entry has the specified tag."
        /// </summary>
        /// <param name="tag">The alias of the tag.</param>
        /// <returns>A future that will contain the result of the operation after the batch is run.</returns>
        public IQdbFuture<bool> HasTag(string tag)
        {
            var tagBytes = Encoding.UTF8.GetBytes(tag);
            var pin = GCHandle.Alloc(tagBytes, GCHandleType.Pinned);

            return AddOperation(
                (ref qdb_operation op) =>
                {
                    op.type = qdb_operation_type.qdb_op_has_tag;
                    op.alias = Alias;
                    op.args.has_tag.tag = pin.AddrOfPinnedObject();
                },
                (ref qdb_operation op) =>
                {
                    pin.Free();

                    // HACK: workaround a known bug in quasardb 2.0.0
                    if (op.error == qdb_error_t.qdb_e_alias_not_found)
                        op.error = qdb_error_t.qdb_e_tag_not_set;

                    return op.error == qdb_error_t.qdb_e_ok;
                });
        }

        internal IQdbFuture AddOperation(MarshalFunc marshal, UnmarshalFunc unmarshal = null)
        {
            return AddOperation(new DelegateOperation(marshal, unmarshal));
        }

        internal IQdbFuture<T> AddOperation<T>(MarshalFunc marshal, UnmarshalFunc<T> unmarshal)
        {
            return AddOperation(new DelegateOperation<T>(marshal, unmarshal));
        }

        internal T AddOperation<T>(T operation)
            where T : IOperation
        {
            _batch.Operations.Add(operation);
            return operation;
        }
    }
}
