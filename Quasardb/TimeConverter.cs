using System;
using Quasardb.NativeApi;

namespace Quasardb
{
    static class TimeConverter
    {
        const long MinSeconds = -62135596800; // 01/01/0001 00:00:00
        const long MaxSeconds = 253402300799; // 31/12/1969 23:59:59
        static DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        const long NanosPerTick = 100;
        const long TicksPerSecond = 1000000000 / NanosPerTick;

        public static qdb_timespec ToTimespec(DateTime dt)
        {
            var ticksSinceEpoch = (dt - _epoch).Ticks;

            qdb_timespec ts;
            ts.tv_sec = Math.DivRem(ticksSinceEpoch, TicksPerSecond,
                out ts.tv_nsec);
            ts.tv_nsec *= NanosPerTick;
            return ts;
        }

        public static DateTime ToDateTime(qdb_timespec ts)
        {
            if (ts.tv_sec <= MinSeconds) return DateTime.MinValue;
            if (ts.tv_sec >= MaxSeconds) return DateTime.MaxValue;
            return _epoch.AddTicks(ts.tv_sec * TicksPerSecond + ts.tv_nsec / NanosPerTick);
        }
    }
}