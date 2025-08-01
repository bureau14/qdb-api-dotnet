﻿using System;
using Quasardb.Native;

namespace Quasardb
{
    static class TimeConverter
    {
        const long MinSeconds = -62135596800; // 01/01/0001 00:00:00
        const long MaxSeconds = 253402300799; // 31/12/9999 23:59:59
        static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        const long NanosPerTick = 100;
        const long TicksPerSecond = 1000000000 / NanosPerTick;

        internal static readonly qdb_timespec NullTimespec = new qdb_timespec
        {
            tv_sec = long.MinValue,
            tv_nsec = long.MinValue
        };

        public static bool IsNull(qdb_timespec ts)
        {
            return ts.tv_sec == long.MinValue && ts.tv_nsec == long.MinValue;
        }

        public static qdb_timespec ToTimespec(DateTime dt)
        {
            var ticksSinceEpoch = (dt - _epoch).Ticks;

            qdb_timespec ts;
            ts.tv_sec = Math.DivRem(ticksSinceEpoch, TicksPerSecond,
                out ts.tv_nsec);
            ts.tv_nsec *= NanosPerTick;
            return ts;
        }

        public static qdb_timespec ToTimespec(DateTime? dt)
        {
            if (dt == null)
                return NullTimespec;
            return ToTimespec((DateTime)dt);
        }

        public static DateTime ToDateTime(qdb_timespec ts)
        {
            if (ts.tv_sec <= MinSeconds) return DateTime.MinValue;
            if (ts.tv_sec >= MaxSeconds) return DateTime.MaxValue;
            return _epoch.AddTicks(ts.tv_sec * TicksPerSecond + ts.tv_nsec / NanosPerTick);
        }
    }
}