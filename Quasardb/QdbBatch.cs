using System.Collections.Generic;
using Quasardb.Exceptions;
using Quasardb.ManagedApi;

namespace Quasardb
{
    /// <summary>
    /// Contains the result of an operation of a QdbBatch
    /// </summary>
    public interface IQdbFuture
    {
        /// <summary>
        /// The error that occured when the operation was executed, or null if success.
        /// </summary>
        QdbException Exception { get; }
    }

    /// <summary>
    /// Contains the result of an operation of a QdbBatch
    /// </summary>
    /// <typeparam name="T">The type of result for the operation.</typeparam>
    public interface IQdbFuture<out T> : IQdbFuture
    {
        /// <summary>
        /// The result of the operation.
        /// </summary>
        T Result { get; }
    }

    /// <summary>
    /// A collection of operation
    /// </summary>
    public sealed class QdbBatch
    {
        readonly List<IOperation> _operations = new List<IOperation>();

        /// <summary>
        /// Gets the number of operation in the batch.
        /// </summary>
        public int Size => _operations.Count;

        /// <summary>
        /// Add blob operations to the batch.
        /// </summary>
        /// <param name="alias">The alias of the blob.</param>
        /// <returns>A handle to a virtual blob on with to perform the operation.</returns>
        public QdbBatchBlob Blob(string alias)
        {
            return new QdbBatchBlob(this, alias);
        }

        /// <summary>
        /// Add integer operations to the batch.
        /// </summary>
        /// <param name="alias">The alias of the integer.</param>
        /// <returns>A handle to a virtual integer on with to perform the operation.</returns>
        public QdbBatchInteger Integer(string alias)
        {
            return new QdbBatchInteger(this, alias);
        }

        internal List<IOperation> Operations => _operations;
    };
}
