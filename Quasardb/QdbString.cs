using System;
using System.Text;
using Quasardb.Exceptions;
using Quasardb.Native;

namespace Quasardb
{
    /// <summary>
    /// A UTF-8 encoded string stored in the database.
    /// </summary>
    /// <remarks>
    /// Qdbstring can be constructed via <see cref="QdbCluster.String" />.
    /// </remarks>
    /// <example>
    /// Here is how to put a string in the database:
    /// <code language="c#">
    /// byte[] myData;
    /// var cluster = new QdbCluster("qdb://127.0.0.1:2836");
    /// 
    /// cluster.string("some name").Put(myData);
    /// </code>
    /// <code language="vb">
    /// Dim myData As Byte()
    /// Dim cluster = New QdbCluster("qdb://127.0.0.1:2836")
    /// 
    /// cluster.string("some name").Put(myData)
    /// </code>
    /// </example>
    public sealed class QdbString : QdbExpirableEntry
    {
        internal QdbString(qdb_handle handle, string alias) : base(handle, alias)
        {
        }

        /// <summary>
        /// Atomically compares and replaces the content when it matches.
        /// </summary>
        /// <param name="content">The new content to put in the string.</param>
        /// <param name="comparand">The content to be compared to.</param>
        /// <param name="expiryTime">The expiry time to set if the string's content is replaced.</param>
        /// <returns>The previous content of the string if it didn't match; <c>null</c> if it matched.</returns>
        /// <exception cref="QdbAliasNotFoundException">The string is not present in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The database entry is not a string.</exception>
        public string CompareAndSwap(string content, string comparand, DateTime? expiryTime = null)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (comparand == null) throw new ArgumentNullException(nameof(comparand));

            using (var oldContent = new QdbStringBuffer(Handle))
            {
                var error = qdb_api.qdb_string_compare_and_swap(
                    Handle, Alias,
                    content, (UIntPtr)Encoding.UTF8.GetByteCount(content),
                    comparand, (UIntPtr)Encoding.UTF8.GetByteCount(comparand),
                    qdb_time.FromOptionalDateTime(expiryTime),
                    out oldContent.Pointer, out oldContent.Size);

                switch (error)
                {
                    case qdb_error.qdb_e_ok:
                        return null;
                    case qdb_error.qdb_e_unmatched_content:
                        return oldContent.GetString();
                    default:
                        throw QdbExceptionFactory.Create(error, alias: Alias);
                }
            }
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <returns>The current content of the string.</returns>
        /// <exception cref="QdbAliasNotFoundException">The string is not present in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The database entry is not a string.</exception>
        public string Get()
        {
            using (var content = new QdbStringBuffer(Handle))
            {
                var error = qdb_api.qdb_string_get(Handle, Alias, out content.Pointer, out content.Size);
                QdbExceptionThrower.ThrowIfNeeded(error, alias: Alias);
                return content.GetString();
            }
        }

        /// <summary>
        /// Atomically gets the content and delete the string.
        /// </summary>
        /// <returns>The previous content of the string.</returns>
        /// <exception cref="QdbAliasNotFoundException">The string is not present in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The database entry is not a string.</exception>
        public string GetAndRemove()
        {
            using (var content = new QdbStringBuffer(Handle))
            {
                var error = qdb_api.qdb_string_get_and_remove(Handle, Alias, out content.Pointer, out content.Size);
                QdbExceptionThrower.ThrowIfNeeded(error, alias: Alias);
                return content.GetString();
            }
        }

        /// <summary>
        /// Atomically gets the content and replaces it.
        /// </summary>
        /// <returns>The previous content of the string.</returns>
        /// <param name="content">The new content of the string.</param>
        /// <param name="expiryTime">The new expiry time of the string.</param>
        /// <exception cref="QdbAliasNotFoundException">The string is not present in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The database entry is not a string.</exception>
        public string GetAndUpdate(string content, DateTime? expiryTime = null)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            using (var oldContent = new QdbStringBuffer(Handle))
            {
                var error = qdb_api.qdb_string_get_and_update(
                    Handle, Alias,
                    content, (UIntPtr)Encoding.UTF8.GetByteCount(content),
                    qdb_time.FromOptionalDateTime(expiryTime),
                    out oldContent.Pointer, out oldContent.Size);

                QdbExceptionThrower.ThrowIfNeeded(error, alias: Alias);
                return oldContent.GetString();
            }
        }

        /// <summary>
        /// Sets the content, but fails if the string already exists.
        /// </summary>
        /// <param name="content">The new content of the string.</param>
        /// <param name="expiryTime">The expiry time to set.</param>
        /// <exception cref="QdbAliasAlreadyExistsException">The entry already exists.</exception>
        public void Put(string content, DateTime? expiryTime = null)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            var error = qdb_api.qdb_string_put(
                Handle, Alias,
                content, (UIntPtr)Encoding.UTF8.GetByteCount(content),
                qdb_time.FromOptionalDateTime(expiryTime));

            QdbExceptionThrower.ThrowIfNeeded(error, alias: Alias);
        }

        /// <summary>
        /// Atomically compares the content and deletes the string when it matches.
        /// </summary>
        /// <param name="comparand">The content to compare to.</param>
        /// <returns>true if remove, false if not.</returns>
        /// <exception cref="QdbAliasNotFoundException">The string is not present in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The database entry is not a string.</exception>
        public bool RemoveIf(string comparand)
        {
            if (comparand == null) throw new ArgumentNullException(nameof(comparand));

            var error = qdb_api.qdb_string_remove_if(
                Handle, Alias,
                comparand, (UIntPtr)Encoding.UTF8.GetByteCount(comparand));

            switch (error)
            {
                case qdb_error.qdb_e_unmatched_content:
                    return false;
                case qdb_error.qdb_e_ok:
                    return true;
                default:
                    throw QdbExceptionFactory.Create(error, alias: Alias);
            }
        }

        /// <summary>
        /// Replaces the content.
        /// </summary>
        /// <param name="content">The new content of the string.</param>
        /// <param name="expiryTime">The expiry time to set.</param>
        /// <returns><c>true</c> if the string has been created, or <c>false</c> if it has been replaced.</returns>
        /// <exception cref="QdbAliasNotFoundException">The string is not present in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The database entry is not a string.</exception>
        public bool Update(string content, DateTime? expiryTime = null)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            var error = qdb_api.qdb_string_update(
                Handle, Alias,
                content, (UIntPtr)Encoding.UTF8.GetByteCount(content),
                qdb_time.FromOptionalDateTime(expiryTime));

            switch (error)
            {
                case qdb_error.qdb_e_ok:
                    return false;
                case qdb_error.qdb_e_ok_created:
                    return true;
                default:
                    throw QdbExceptionFactory.Create(error, alias: Alias);
            }
        }
    }
}
