using System;
using Quasardb.Exceptions;
using Quasardb.Interop;

namespace Quasardb
{
    public class QdbExpirableEntry : QdbEntry
    {
        static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public QdbExpirableEntry(qdb_handle handle, string alias) : base(handle, alias)
        {
        }

        public void ExpiresAt(DateTime expiryTime)
        {
            var error = qdb_api.qdb_expires_at(_handle, _alias, TranslateExpiryTime(expiryTime));
            QdbExceptionThrower.ThrowIfNeeded(error);
        }

        public DateTime GetExpiryTime()
        {
            long expiryTime;
            var error = qdb_api.qdb_get_expiry_time(_handle, _alias, out expiryTime);
            QdbExceptionThrower.ThrowIfNeeded(error);
            return TranslateExpiryTime(expiryTime);
        }

        protected long TranslateExpiryTime(DateTime expiryTime)
        {
            if (expiryTime == default(DateTime)) return 0;
            return (long)expiryTime.Subtract(Epoch).TotalSeconds;
        }

        protected DateTime TranslateExpiryTime(long expiryTime)
        {
            if (expiryTime == 0) return default(DateTime);
            return Epoch.AddSeconds(expiryTime);
        }
    }
}