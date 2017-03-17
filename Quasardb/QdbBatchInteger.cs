using System;
using Quasardb.Native;

namespace Quasardb
{
    /// <summary>
    /// Adds blob operations to a batch
    /// </summary>
    public sealed class QdbBatchInteger : QdbBatchEntry
    {
        internal QdbBatchInteger(QdbBatch batch, string alias) : base(batch, alias)
        {
        }

        /// <summary>
        /// Adds a "Get" operation to the batch: "Read the value of the integer."
        /// </summary>
        /// <returns>A future that will contain the result of the operation after the batch is run.</returns>
        /// <seealso cref="QdbInteger.Get"/>
        public IQdbFuture<long> Get()
        {
            return AddOperation(
                (ref qdb_operation op) =>
                {
                    op.type = qdb_operation_type.qdb_op_int_get;
                    op.alias = Alias;
                },
                (ref qdb_operation op) =>
                {
                    return op.args.int_get.result;
                });
        }

        /// <summary>
        /// Adds a "Add" operation to the batch: "Atomically adds the given value to the current value."
        /// </summary>
        /// <param name="addend">The value to add</param>
        /// <returns>A future that will contain the result of the operation after the batch is run.</returns>
        /// <seealso cref="QdbInteger.Add"/>
        public IQdbFuture<long> Add(long addend)
        {
            return AddOperation(
                (ref qdb_operation op) =>
                {
                    op.type = qdb_operation_type.qdb_op_int_add;
                    op.alias = Alias;
                    op.args.int_add.addend = addend;
                },
                (ref qdb_operation op) =>
                {
                    return op.args.int_get.result;
                });
        }

        /// <summary>
        /// Adds a "Put" operation to the batch: "Creates a new integer. Errors if the integer already exists."
        /// </summary>
        /// <param name="value">The initial value of the integer</param>
        /// <param name="expiry">The optional expirt time for the integer</param>
        /// <returns>A future that will contain the result of the operation after the batch is run.</returns>
        /// <seealso cref="QdbInteger.Put"/>
        public IQdbFuture Put(long value, DateTime? expiry = null)
        {
            return AddOperation(
                (ref qdb_operation op) =>
                {
                    op.type = qdb_operation_type.qdb_op_int_put;
                    op.alias = Alias;
                    op.args.int_set.value = value;
                    op.args.int_set.expiry = qdb_time.FromOptionalDateTime(expiry);
                });
        }

        /// <summary>
        /// Adds a "Update" operation to the batch: "Updates an existing integer or creates one if it does not exist."
        /// </summary>
        /// <param name="value">The new value of the integer</param>
        /// <param name="expiry">The optional expiry time of the integer</param>
        /// <returns>A future that will contain the result of the operation after the batch is run.</returns>
        /// <seealso cref="QdbInteger.Update"/>
        public IQdbFuture Update(long value, DateTime? expiry = null)
        {
            return AddOperation(
                (ref qdb_operation op) =>
                {
                    op.type = qdb_operation_type.qdb_op_int_update;
                    op.alias = Alias;
                    op.args.int_set.value = value;
                    op.args.int_set.expiry = qdb_time.FromOptionalDateTime(expiry);
                });
        }
    }
}
