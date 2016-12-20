using System;
using Quasardb.Exceptions;
using Quasardb.ManagedApi;

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
        internal QdbHashSet(QdbApi api, string alias) : base(api, alias)
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

            return Api.HashSetInsert(Alias, content);
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

            return Api.HashSetErase(Alias, content);
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

            return Api.HashSetContains(Alias, content);
        }
    }
}
