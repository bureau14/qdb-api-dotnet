using System;
using Quasardb.Exceptions;
using Quasardb.Interop;

namespace Quasardb
{
    /// <summary>
    /// A blob (unstructured data) in a quasardb database.
    /// </summary>
    /// <remarks>
    /// QdbBlob can be constructed via <see cref="QdbCluster.Blob" />.
    /// </remarks>
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
        /// <returns>The previous content of the blob.</returns>
        /// <exception cref="QdbAliasNotFoundException">The blob is not present in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The database entry is not a blob.</exception>
        public byte[] CompareAndSwap(byte[] content, byte[] comparand, DateTime? expiryTime = null)
        {
            if (content == null) throw new ArgumentNullException("content");
            if (comparand == null) throw new ArgumentNullException("comparand");

            using (var oldContent = new qdb_buffer(Handle))
            {
                var error = qdb_api.qdb_compare_and_swap(Handle, Alias,
                    content, (IntPtr)content.LongLength,
                    comparand, (IntPtr)comparand.LongLength,
                    qdb_time.FromOptionalDateTime(expiryTime),
                    out oldContent.Pointer, out oldContent.Size);

                QdbExceptionThrower.ThrowIfNeeded(error);

                return oldContent.GetBytes();
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
            using (var content = new qdb_buffer(Handle))
            {
                var error = qdb_api.qdb_get(Handle, Alias, out content.Pointer, out content.Size);
                QdbExceptionThrower.ThrowIfNeeded(error);
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
            using (var content = new qdb_buffer(Handle))
            {
                var error = qdb_api.qdb_get_and_remove(Handle, Alias, out content.Pointer, out content.Size);
                QdbExceptionThrower.ThrowIfNeeded(error);
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
            if (content == null) throw new ArgumentNullException("content");

            using (var oldContent = new qdb_buffer(Handle))
            {
                var error = qdb_api.qdb_get_and_update(Handle, Alias,
                    content, (IntPtr) content.LongLength,
                    qdb_time.FromOptionalDateTime(expiryTime),
                    out oldContent.Pointer, out oldContent.Size);

                QdbExceptionThrower.ThrowIfNeeded(error);
                return oldContent.GetBytes();
            }
        }

        /// <summary>
        /// Sets the content, but fails if the blob already exists.
        /// </summary>
        /// <param name="content">The new content of the blob.</param>
        /// <param name="expiryTime">The expiry time to set.</param>
        /// <exception cref="QdbAliasAlreadyExistsException">The entry already exists.</exception>
        /// <returns><c>this</c>, allowing to chain other commands</returns>
        public QdbBlob Put(byte[] content, DateTime? expiryTime=null)
        {
            if (content == null) throw new ArgumentNullException("content");

            var error = qdb_api.qdb_put(Handle, Alias, 
                content, (IntPtr)content.LongLength, 
                qdb_time.FromOptionalDateTime(expiryTime));

            QdbExceptionThrower.ThrowIfNeeded(error);
            return this;
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

            var error = qdb_api.qdb_remove_if(Handle, Alias, comparand, (IntPtr)comparand.Length);
            if (error == qdb_error.qdb_e_unmatched_content) return false;
            QdbExceptionThrower.ThrowIfNeeded(error);
            return true;
        }

        /// <summary>
        /// Replaces the content.
        /// </summary>
        /// <param name="content">The new content of the blob.</param>
        /// <param name="expiryTime">The expiry time to set.</param>
        /// <returns>true if remove, false if not.</returns>
        /// <exception cref="QdbAliasNotFoundException">The blob is not present in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The database entry is not a blob.</exception>
        /// <returns><c>this</c>, allowing to chain other commands</returns>
        public QdbBlob Update(byte[] content, DateTime? expiryTime = null)
        {
            var error = qdb_api.qdb_update(Handle, Alias, 
                content, (IntPtr) content.Length, 
                qdb_time.FromOptionalDateTime(expiryTime));

            QdbExceptionThrower.ThrowIfNeeded(error);

            return this;
        }
    }
}
