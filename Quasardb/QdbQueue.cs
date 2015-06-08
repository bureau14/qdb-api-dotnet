using System;
using Quasardb.Exceptions;
using Quasardb.Interop;

namespace Quasardb
{
    /// <summary>
    /// A queue in the database.
    /// </summary>
    public sealed class QdbQueue : QdbEntry
    {
        internal QdbQueue(qdb_handle handle, string alias) : base(handle, alias)
        {
        }
        
        /// <summary>
        /// Gets the value at the end of the queue.
        /// </summary>
        /// <returns>The value at the end of the queue.</returns>
        /// <exception cref="QdbAliasNotFoundException">The queue doesn't exists in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The matching entry in the database is not a queue.</exception>
        public byte[] Back()
        {
            qdb_buffer content;
            IntPtr contentLength;
            var error = qdb_api.qdb_queue_back(Handle, Alias, out content, out contentLength);
            QdbExceptionThrower.ThrowIfNeeded(error);
            return content.Copy(contentLength);
        }

        /// <summary>
        /// Gets the value at the beginning of the queue.
        /// </summary>
        /// <returns>The value at the beginning of the queue.</returns>
        /// <exception cref="QdbAliasNotFoundException">The queue doesn't exists in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The matching entry in the database is not a queue.</exception>
        public byte[] Front()
        {
            qdb_buffer content;
            IntPtr contentLength;
            var error = qdb_api.qdb_queue_front(Handle, Alias, out content, out contentLength);
            QdbExceptionThrower.ThrowIfNeeded(error);
            return content.Copy(contentLength);
        }

        /// <summary>
        /// Dequeues a value from the end of the queue.
        /// </summary>
        /// <returns>The value from the end of the queue.</returns>
        /// <exception cref="QdbAliasNotFoundException">The queue doesn't exists in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The matching entry in the database is not a queue.</exception>
        /// <exception cref="QdbEmptyContainerException">The queue is empty.</exception>
        public byte[] PopBack()
        {
            qdb_buffer content;
            IntPtr contentLength;
            var error = qdb_api.qdb_queue_pop_back(Handle, Alias, out content, out contentLength);
            QdbExceptionThrower.ThrowIfNeeded(error);
            return content.Copy(contentLength);
        }

        /// <summary>
        /// Dequeues a value from the beginning of the queue.
        /// </summary>
        /// <returns>The value from the beginning of the queue.</returns>
        /// <exception cref="QdbAliasNotFoundException">The queue doesn't exists in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The matching entry in the database is not a queue.</exception>
        /// <exception cref="QdbEmptyContainerException">The queue is empty.</exception>
        public byte[] PopFront()
        {
            qdb_buffer content;
            IntPtr contentLength;
            var error = qdb_api.qdb_queue_pop_front(Handle, Alias, out content, out contentLength);
            QdbExceptionThrower.ThrowIfNeeded(error);
            return content.Copy(contentLength);
        }

        /// <summary>
        /// Enqueues a value at the end of the queue.
        /// </summary>
        /// <param name="content">The value to enqueue.</param>
        /// <exception cref="QdbIncompatibleTypeException">The matching entry in the database is not a queue.</exception>
        public void PushBack(byte[] content)
        {
            if (content == null) throw new ArgumentNullException("content");

            var error = qdb_api.qdb_queue_push_back(Handle, Alias, content, (IntPtr) content.LongLength);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }

        /// <summary>
        /// Enqueues a value at the beginning of the queue.
        /// </summary>
        /// <param name="content">The value to enqueue.</param>
        /// <exception cref="QdbIncompatibleTypeException">The matching entry in the database is not a queue.</exception>
        public void PushFront(byte[] content)
        {
            if (content == null) throw new ArgumentNullException("content");

            var error = qdb_api.qdb_queue_push_front(Handle, Alias, content, (IntPtr) content.LongLength);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }
    }
}
