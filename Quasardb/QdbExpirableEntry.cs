using System;
using Quasardb.Exceptions;
using Quasardb.ManagedApi;

namespace Quasardb
{
    /// <summary>
    /// An entry which can have an expiry time..
    /// </summary>
    public class QdbExpirableEntry : QdbEntry
    {
        internal QdbExpirableEntry(QdbApi api, string alias) : base(api, alias)
        {
        }

        /// <summary>
        /// Sets an absolute expiry time.
        /// </summary>
        /// <param name="expiryTime">The absolute expiry time.</param>
        /// <exception cref="QdbAliasNotFoundException">The entry doesn't exists in the database.</exception>
        public void ExpiresAt(DateTime expiryTime)
        {
            Api.ExpiresAt(Alias, expiryTime);
        }

        /// <summary>
        /// Sets an relative expiry time.
        /// </summary>
        /// <param name="ttl">The relative expiry time.</param>
        /// <exception cref="QdbAliasNotFoundException">The entry doesn't exists in the database.</exception>
        public void ExpiresFromNow(TimeSpan ttl)
        {
            Api.ExpiresFromNow(Alias, ttl);
        }

        /// <summary>
        /// Gets the expiry time.
        /// </summary>
        /// <returns>The expiry time, or null if no expiry is set.</returns>
        /// <exception cref="QdbAliasNotFoundException">The entry doesn't exists in the database.</exception>
        public DateTime? GetExpiryTime()
        {
            return Api.GetExpiryTime(Alias);
        }
    }
}