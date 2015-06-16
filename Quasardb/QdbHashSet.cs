using System;
using Quasardb.Exceptions;
using Quasardb.Interop;

namespace Quasardb
{
    /// <summary>
    /// An unordered set in the database.
    /// </summary>
    /// /// <remarks>
    /// QdbHashSet can be constructed via <see cref="QdbCluster.HashSet" />.
    /// </remarks>
    public sealed class QdbHashSet : QdbEntry
    {
        internal QdbHashSet(qdb_handle handle, string alias) : base(handle, alias)
        {
        }

        /// <summary>
        /// Add a new value if the set.
        /// </summary>
        /// <param name="content">The value.</param>
        /// <returns>false if the value was already in the set.</returns>
        /// <exception cref="QdbIncompatibleTypeException">The entry in the database is not a hash-set.</exception>
        public QdbHashSet Insert(byte[] content, out bool alreadyExisted)
        {
            if (content == null) throw new ArgumentNullException("content");

            var error = qdb_api.qdb_hset_insert(Handle, Alias, content, (IntPtr) content.LongLength);

            switch (error)
            {
                case qdb_error.qdb_e_element_already_exists:
                    alreadyExisted = true;
                    break;

                case qdb_error.qdb_e_ok:
                    alreadyExisted = false;
                    break;

                default:
                    throw QdbExceptionFactory.Create(error);
            }

            return this;
        }

        public QdbHashSet Insert(byte[] content)
        {
            bool dummy;
            return Insert(content, out dummy);
        }

        
        /// <summary>
        /// Removes a value from the set.
        /// </summary>
        /// <param name="content">The value to remove.</param>
        /// <returns>false if the value was not present.</returns>
        /// <exception cref="QdbIncompatibleTypeException">The entry in the database is not a hash-set.</exception>
        public QdbHashSet Erase(byte[] content, out bool notFound)
        {
            if (content == null) throw new ArgumentNullException("content");

            var error = qdb_api.qdb_hset_erase(Handle, Alias, content, (IntPtr) content.LongLength);

            switch (error)
            {
                case qdb_error.qdb_e_element_not_found:
                    notFound = true;
                    break;

                case qdb_error.qdb_e_ok:
                    notFound = false;
                    break;

                default:
                    throw QdbExceptionFactory.Create(error);
            }

            return this;
        }

        public QdbHashSet Erase(byte[] content)
        {
            bool dummy;
            return Erase(content, out dummy);
        }

        /// <summary>
        /// Check is a value is in the set.
        /// </summary>
        /// <param name="content">The value to remove.</param>
        /// <returns>true if the value is present, false if not.</returns>
        /// <exception cref="QdbIncompatibleTypeException">The entry in the database is not a hash-set.</exception>
        public bool Contains(byte[] content)
        {
            if (content == null) throw new ArgumentNullException("content");

            var error = qdb_api.qdb_hset_contains(Handle, Alias, content, (IntPtr) content.LongLength);
            if (error == qdb_error.qdb_e_element_not_found) return false;
            QdbExceptionThrower.ThrowIfNeeded(error);
            return true;
        }
    }
}
