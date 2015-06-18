using System;
using Quasardb.Exceptions;
using Quasardb.ManagedApi;

namespace Quasardb
{
    /// <summary>
    /// A queue in the database.
    /// </summary>
    /// <example>
    /// Here is how to put a blob in the database:
    /// <code language="c#">
    /// byte[] myData;
    /// var cluster = new QdbCluster("qdb://127.0.0.1:2836");
    /// 
    /// cluster.Queue("some name").PushBack(myData);
    /// </code>
    /// <code language="vb">
    /// Dim myData As Byte()
    /// Dim cluster = New QdbCluster("qdb://127.0.0.1:2836")
    /// 
    /// cluster.Queue("some name").PushBack(myData)
    /// </code>
    /// </example>
    public sealed class QdbQueue : QdbEntry
    {
        internal QdbQueue(QdbApi api, string alias) : base(api, alias)
        {
        }
        
        /// <summary>
        /// Gets the value at the end of the queue.
        /// </summary>
        /// <returns>The value at the end of the queue.</returns>
        /// <exception cref="QdbAliasNotFoundException">The queue doesn't exists in the database.</exception>
        /// <exception cref="QdbEmptyContainerException">The queue is empty.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The matching entry in the database is not a queue.</exception>
        public byte[] Back()
        {
            return Api.QueueBack(Alias);
        }

        /// <summary>
        /// Gets the value at the beginning of the queue.
        /// </summary>
        /// <returns>The value at the beginning of the queue.</returns>
        /// <exception cref="QdbAliasNotFoundException">The queue doesn't exists in the database.</exception>
        /// <exception cref="QdbEmptyContainerException">The queue is empty.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The matching entry in the database is not a queue.</exception>
        public byte[] Front()
        {
            return Api.QueueFront(Alias);
        }

        /// <summary>
        /// Dequeues a value from the end of the queue.
        /// </summary>
        /// <returns>The value from the end of the queue.</returns>
        /// <exception cref="QdbAliasNotFoundException">The queue doesn't exists in the database.</exception>
        /// <exception cref="QdbEmptyContainerException">The queue is empty.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The matching entry in the database is not a queue.</exception>
        public byte[] PopBack()
        {
            return Api.QueuePopBack(Alias);
        }

        /// <summary>
        /// Dequeues a value from the beginning of the queue.
        /// </summary>
        /// <returns>The value from the beginning of the queue.</returns>
        /// <exception cref="QdbAliasNotFoundException">The queue doesn't exists in the database.</exception>
        /// <exception cref="QdbEmptyContainerException">The queue is empty.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The matching entry in the database is not a queue.</exception>
        public byte[] PopFront()
        {
            return Api.QueuePopFront(Alias);
        }

        /// <summary>
        /// Enqueues a value at the end of the queue.
        /// </summary>
        /// <param name="content">The value to enqueue.</param>
        /// <exception cref="QdbIncompatibleTypeException">The matching entry in the database is not a queue.</exception>
        public void PushBack(byte[] content)
        {
            if (content == null) throw new ArgumentNullException("content");

            Api.QueuePushBack(Alias, content);
        }

        /// <summary>
        /// Enqueues a value at the beginning of the queue.
        /// </summary>
        /// <param name="content">The value to enqueue.</param>
        /// <exception cref="QdbIncompatibleTypeException">The matching entry in the database is not a queue.</exception>
        public void PushFront(byte[] content)
        {
            if (content == null) throw new ArgumentNullException("content");

            Api.QueuePushFront(Alias, content);
        }
    }
}
