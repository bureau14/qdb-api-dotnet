using System;
using Quasardb.NativeApi;

namespace Quasardb
{
    static class TimeConverter
    {
        const long NanosPerTick = 100;
        const long TicksPerSecond = 1000000000 / NanosPerTick;

        public static qdb_timespec ToTimespec(DateTime dt)
        {
            qdb_timespec ts;
            ts.tv_sec = Math.DivRem(dt.ToFileTimeUtc(), TicksPerSecond,
                out ts.tv_nsec);
            ts.tv_nsec *= NanosPerTick;
            return ts;
        }

        public static DateTime ToDateTime(qdb_timespec ts)
        {
            return DateTime.FromFileTimeUtc(ts.tv_sec * TicksPerSecond + ts.tv_nsec / NanosPerTick);
        }
    }
}