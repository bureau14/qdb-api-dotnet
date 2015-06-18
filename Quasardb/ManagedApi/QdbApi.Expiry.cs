using System;
using Quasardb.NativeApi;

namespace Quasardb.ManagedApi
{
    partial class QdbApi
    {
        public void ExpiresAt(string alias, DateTime expiryTime)
        {
            var error = qdb_api.qdb_expires_at(_handle, alias, qdb_time.FromDateTime(expiryTime));
            QdbExceptionThrower.ThrowIfNeeded(error);
        }

        public void ExpiresFromNow(string alias, TimeSpan ttl)
        {
            var error = qdb_api.qdb_expires_from_now(_handle, alias, qdb_time.FromTimeSpan(ttl));
            QdbExceptionThrower.ThrowIfNeeded(error);
        }

        public DateTime? GetExpiryTime(string alias)
        {
            long expiryTime;
            var error = qdb_api.qdb_get_expiry_time(_handle, alias, out expiryTime);
            QdbExceptionThrower.ThrowIfNeeded(error);
            return qdb_time.ToOptionalDateTime(expiryTime);
        }
    }
}
