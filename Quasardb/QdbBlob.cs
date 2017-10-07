using System;
using Quasardb.Exceptions;
using Quasardb.Native;

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
        internal QdbBlob(qdb_handle handle, string alias) : base(handle, alias)
        {
        }

        /// <summary>
        /// Atomically compares and replaces the content when it matches.
        /// </summary>
        /// <param name="content">The new content to put in the blob.</param>
        /// <param name="comparand">The content to be compared to.</param>
        /// <param name="expiryTime">The expiry time to set if the blob's content is replaced.</param>
        /// <returns>The previous content of the blob if it didn't match; <c>null</c> if it matched.</returns>
        /// <exception cref="QdbAliasNotFoundException">The blob is not present in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The database entry is not a blob.</exception>
        public byte[] CompareAndSwap(byte[] content, byte[] comparand, DateTime? expiryTime = null)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (comparand == null) throw new ArgumentNullException(nameof(comparand));

            using (var oldContent = new QdbBlobBuffer(Handle))
            {
                var error = qdb_api.qdb_blob_compare_and_swap(Handle, Alias,
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
                        throw QdbExceptionFactory.Create(error, alias: Alias);
                }
            }
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <returns>The current content of the blob.</returns>
        /// <exception cref="QdbAliasNotFoundException">The blob is not present in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The database entry is not a blob.</exception>
        public byte[] Get()
        {
            using (var content = new QdbBlobBuffer(Handle))
            {
                var error = qdb_api.qdb_blob_get(Handle, Alias, out content.Pointer, out content.Size);
                QdbExceptionThrower.ThrowIfNeeded(error, alias: Alias);
                return content.GetBytes();
            }
        }

        /// <summary>
        /// Atomically gets the content and delete the blob.
        /// </summary>
        /// <returns>The previous content of the blob.</returns>
        /// <exception cref="QdbAliasNotFoundException">The blob is not present in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The database entry is not a blob.</exception>
        public byte[] GetAndRemove()
        {
            using (var content = new QdbBlobBuffer(Handle))
            {
                var error = qdb_api.qdb_blob_get_and_remove(Handle, Alias, out content.Pointer, out content.Size);
                QdbExceptionThrower.ThrowIfNeeded(error, alias: Alias);
                return content.GetBytes();
            }
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
            if (content == null) throw new ArgumentNullException(nameof(content));

            using (var oldContent = new QdbBlobBuffer(Handle))
            {
                var error = qdb_api.qdb_blob_get_and_update(Handle, Alias,
                    content, (UIntPtr)content.LongLength,
                    qdb_time.FromOptionalDateTime(expiryTime),
                    out oldContent.Pointer, out oldContent.Size);

                QdbExceptionThrower.ThrowIfNeeded(error, alias: Alias);
                return oldContent.GetBytes();
            }
        }

        /// <summary>
        /// Sets the content, but fails if the blob already exists.
        /// </summary>
        /// <param name="content">The new content of the blob.</param>
        /// <param name="expiryTime">The expiry time to set.</param>
        /// <exception cref="QdbAliasAlreadyExistsException">The entry already exists.</exception>
        public void Put(byte[] content, DateTime? expiryTime=null)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            var error = qdb_api.qdb_blob_put(Handle, Alias,
                content, (UIntPtr)content.LongLength,
                qdb_time.FromOptionalDateTime(expiryTime));

            QdbExceptionThrower.ThrowIfNeeded(error, alias: Alias);
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
            if (comparand == null) throw new ArgumentNullException(nameof(comparand));

            var error = qdb_api.qdb_blob_remove_if(Handle, Alias, comparand, (UIntPtr)comparand.Length);

            switch (error)
            {
                case qdb_error_t.qdb_e_unmatched_content:
                    return false;

                case qdb_error_t.qdb_e_ok:
                    return true;

                default:
                    throw QdbExceptionFactory.Create(error, alias: Alias);
            }
        }

        /// <summary>
        /// Replaces the content.
        /// </summary>
        /// <param name="content">The new content of the blob.</param>
        /// <param name="expiryTime">The expiry time to set.</param>
        /// <returns><c>true</c> if the blob has been created, or <c>false</c> if it has been replaced.</returns>
        /// <exception cref="QdbAliasNotFoundException">The blob is not present in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The database entry is not a blob.</exception>
        public bool Update(byte[] content, DateTime? expiryTime = null)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            var error = qdb_api.qdb_blob_update(Handle, Alias,
                content, (UIntPtr)content.Length,
                qdb_time.FromOptionalDateTime(expiryTime));
            
            switch (error)
            {
                case qdb_error_t.qdb_e_ok:
                    return false;

                case qdb_error_t.qdb_e_ok_created:
                    return true;

                default:
                    throw QdbExceptionFactory.Create(error, alias: Alias);
            }
        }
    }
}
