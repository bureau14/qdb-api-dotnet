using System;

// ReSharper disable InconsistentNaming

using qdb_time_t = System.Int64;

namespace Quasardb.Native
{
    internal static class qdb_time
    {
        static readonly DateTime _utcEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        internal static qdb_time_t FromOptionalDateTime(DateTime? expiryTime)
        {
            return expiryTime.HasValue ? FromDateTime(expiryTime.Value) : 0;
        }

        internal static qdb_time_t FromDateTime(DateTime expiryTime)
        {
            return (qdb_time_t) expiryTime.Subtract(_utcEpoch).TotalMilliseconds;
        }

        internal static qdb_time_t FromTimeSpan(TimeSpan delay)
        {
            return (qdb_time_t) delay.TotalMilliseconds;
        }

        internal static DateTime? ToOptionalDateTime(qdb_time_t expiryTime)
        {
            return expiryTime != 0 ? ToDateTime(expiryTime) : (DateTime?) null;
        }

        private static DateTime ToDateTime(qdb_time_t expiryTime)
        {
            return _utcEpoch.AddMilliseconds(expiryTime);
        }
    }
}
