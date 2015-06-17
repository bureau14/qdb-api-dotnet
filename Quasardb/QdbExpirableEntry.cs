using System;
using Quasardb.Exceptions;
using Quasardb.Interop;

namespace Quasardb
{
    /// <summary>
    /// An entry which can have an expiry time..
    /// </summary>
    public class QdbExpirableEntry : QdbEntry
    {
        internal QdbExpirableEntry(qdb_handle handle, string alias) : base(handle, alias)
        {
        }

        /// <summary>
        /// Sets an absolute expiry time.
        /// </summary>
        /// <param name="expiryTime">The absolute expiry time.</param>
        public void ExpiresAt(DateTime expiryTime)
        {
            var error = qdb_api.qdb_expires_at(Handle, Alias, qdb_time.FromDateTime(expiryTime));
            QdbExceptionThrower.ThrowIfNeeded(error);
        }

        /// <summary>
        /// Sets an relative expiry time.
        /// </summary>
        /// <param name="ttl">The relative expiry time.</param>
        public void ExpiresFromNow(TimeSpan ttl)
        {
            var error = qdb_api.qdb_expires_from_now(Handle, Alias, qdb_time.FromTimeSpan(ttl));
            QdbExceptionThrower.ThrowIfNeeded(error);
        }
        
        /// <summary>
        /// Gets the expiry time.
        /// </summary>
        /// <returns>The expiry time, or null if no expiry is set.</returns>
        public DateTime? GetExpiryTime()
        {
            long expiryTime;
            var error = qdb_api.qdb_get_expiry_time(Handle, Alias, out expiryTime);
            QdbExceptionThrower.ThrowIfNeeded(error);
            return qdb_time.ToOptionalDateTime(expiryTime);
        }
    }
}