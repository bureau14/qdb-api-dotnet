using System;
using Quasardb.Exceptions;
using Quasardb.Interop;

namespace Quasardb
{
    public class QdbExpirableEntry : QdbEntry
    {
        static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public QdbExpirableEntry(qdb_handle handle, string alias) : base(handle, alias)
        {
        }

        public void ExpiresAt(DateTime expiryTime)
        {
            var error = qdb_api.qdb_expires_at(m_handle, m_alias, TranslateExpiryTime(expiryTime));
            QdbExceptionThrower.ThrowIfNeeded(error);
        }

        public void ExpiresFromNow(TimeSpan ttl)
        {
            var error = qdb_api.qdb_expires_from_now(m_handle, m_alias, TranslateExpiryTime(ttl));
            QdbExceptionThrower.ThrowIfNeeded(error);
        }

        public DateTime GetExpiryTime()
        {
            long expiryTime;
            var error = qdb_api.qdb_get_expiry_time(m_handle, m_alias, out expiryTime);
            QdbExceptionThrower.ThrowIfNeeded(error);
            return TranslateExpiryTime(expiryTime);
        }

        protected long TranslateExpiryTime(DateTime expiryTime)
        {
            if (expiryTime == default(DateTime)) return 0;
            return (long)expiryTime.Subtract(_epoch).TotalSeconds;
        }

        protected long TranslateExpiryTime(TimeSpan delay)
        {
            return (long)delay.TotalSeconds;
        }

        protected DateTime TranslateExpiryTime(long expiryTime)
        {
            if (expiryTime == 0) return default(DateTime);
            return _epoch.AddSeconds(expiryTime);
        }
    }
}