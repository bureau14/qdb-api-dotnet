using System;
using Quasardb.Exceptions;
using Quasardb.ManagedApi;

namespace Quasardb
{
    /// <summary>
    /// A blob (unstructured data) in a quasardb database.
    /// </summary>
    /// <remarks>
    /// QdbBlob can be constructed via <see cref="QdbCluster.Blob" />.
    /// </remarks>
    /// <example>
    /// Here is how to put a blob in the database:
    /// <code language="c#">
    /// byte[] myData;
    /// var cluster = new QdbCluster("qdb://127.0.0.1:2836");
    /// 
    /// cluster.Blob("some name").Put(myData);
    /// </code>
    /// <code language="vb">
    /// Dim myData As Byte()
    /// Dim cluster = New QdbCluster("qdb://127.0.0.1:2836")
    /// 
    /// cluster.Blob("some name").Put(myData)
    /// </code>
    /// </example>
    public sealed class QdbBlob : QdbExpirableEntry
    {
        internal QdbBlob(QdbApi api, string alias) : base(api, alias)
        {
        }

        /// <summary>
        /// Atomically compares and replaces the content when it matches.
        /// </summary>
        /// <param name="content">The new content to put in the blob.</param>
        /// <param name="comparand">The content to be compared to.</param>
        /// <param name="expiryTime">The expiry time to set if the blob's content is replaced.</param>
        /// <returns>The previous content of the blob.</returns>
        /// <exception cref="QdbAliasNotFoundException">The blob is not present in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The database entry is not a blob.</exception>
        public byte[] CompareAndSwap(byte[] content, byte[] comparand, DateTime? expiryTime = null)
        {
            if (content == null) throw new ArgumentNullException("content");
            if (comparand == null) throw new ArgumentNullException("comparand");

            return Api.BlobCompareAndSwap(Alias, content, comparand, expiryTime);
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <returns>The current content of the blob.</returns>
        /// <exception cref="QdbAliasNotFoundException">The blob is not present in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The database entry is not a blob.</exception>
        public byte[] Get()
        {
            return Api.BlobGet(Alias);
        }

        /// <summary>
        /// Atomically gets the content and delete the blob.
        /// </summary>
        /// <returns>The previous content of the blob.</returns>
        /// <exception cref="QdbAliasNotFoundException">The blob is not present in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The database entry is not a blob.</exception>
        public byte[] GetAndRemove()
        {
            return Api.BlobGetAndRemove(Alias);
        }

        /// <summary>
        /// Atomically gets the content and replaces it.
        /// </summary>
        /// <returns>The previous content of the blob.</returns>
        /// <param name="content">The new content of the blob.</param>
        /// <param name="expiryTime">The new expiry time of the blob.</param>
        /// <exception cref="QdbAliasNotFoundException">The blob is not present in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The database entry is not a blob.</exception>
        public byte[] GetAndUpdate(byte[] content, DateTime? expiryTime = null)
        {
            if (content == null) throw new ArgumentNullException("content");

            return Api.BlobGetAndUpdate(Alias, content, expiryTime);
        }

        /// <summary>
        /// Sets the content, but fails if the blob already exists.
        /// </summary>
        /// <param name="content">The new content of the blob.</param>
        /// <param name="expiryTime">The expiry time to set.</param>
        /// <exception cref="QdbAliasAlreadyExistsException">The entry already exists.</exception>
        public void Put(byte[] content, DateTime? expiryTime=null)
        {
            if (content == null) throw new ArgumentNullException("content");

            Api.BlobPut(Alias, content, expiryTime);
        }

        /// <summary>
        /// Atomically compares the content and deletes the blob when it matches.
        /// </summary>
        /// <param name="comparand">The content to compare to.</param>
        /// <returns>true if remove, false if not.</returns>
        /// <exception cref="QdbAliasNotFoundException">The blob is not present in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The database entry is not a blob.</exception>
        public bool RemoveIf(byte[] comparand)
        {
            if (comparand == null) throw new ArgumentNullException("comparand");

            return Api.BlobRemoveIf(Alias, comparand);
        }

        /// <summary>
        /// Replaces the content.
        /// </summary>
        /// <param name="content">The new content of the blob.</param>
        /// <param name="expiryTime">The expiry time to set.</param>
        /// <returns><c>true</c> if remove, <c>false</c> if not.</returns>
        /// <exception cref="QdbAliasNotFoundException">The blob is not present in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The database entry is not a blob.</exception>
        public void Update(byte[] content, DateTime? expiryTime = null)
        {
            if (content == null) throw new ArgumentNullException("content");

            Api.BlobUpdate(Alias, content, expiryTime);
        }
    }
}
