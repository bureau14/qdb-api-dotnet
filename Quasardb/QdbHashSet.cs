using System;
using Quasardb.Exceptions;
using Quasardb.Native;

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
        public bool Insert(byte[] content)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            var error = qdb_api.qdb_hset_insert(Handle, Alias, content, (UIntPtr)content.LongLength);

            switch (error)
            {
                case qdb_error.qdb_e_element_already_exists:
                    return false;

                case qdb_error.qdb_e_ok:
                    return true;

                default:
                    throw QdbExceptionFactory.Create(error, alias: Alias);
            }
        }
        
        /// <summary>
        /// Removes a value from the set.
        /// </summary>
        /// <param name="content">The value to remove.</param>
        /// <returns>false if the value was not present.</returns>
        /// <exception cref="QdbAliasNotFoundException">The hash-set doesn't exist.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The entry in the database is not a hash-set.</exception>
        public bool Erase(byte[] content)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            var error = qdb_api.qdb_hset_erase(Handle, Alias, content, (UIntPtr)content.LongLength);

            switch (error)
            {
                case qdb_error.qdb_e_element_not_found:
                    return false;

                case qdb_error.qdb_e_ok:
                    return true;

                default:
                    throw QdbExceptionFactory.Create(error, alias: Alias);
            }
        }

        /// <summary>
        /// Check is a value is in the set.
        /// </summary>
        /// <param name="content">The value to remove.</param>
        /// <returns>true if the value is present, false if not.</returns>
        /// <exception cref="QdbAliasNotFoundException">The hash-set doesn't exist.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The entry in the database is not a hash-set.</exception>
        public bool Contains(byte[] content)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            var error = qdb_api.qdb_hset_contains(Handle, Alias, content, (UIntPtr)content.LongLength);

            switch (error)
            {
                case qdb_error.qdb_e_element_not_found:
                    return false;

                case qdb_error.qdb_e_ok:
                    return true;

                default:
                    throw QdbExceptionFactory.Create(error, alias: Alias);
            }
        }
    }
}
