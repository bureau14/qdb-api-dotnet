using System;
using Quasardb.Native;

namespace Quasardb.TimeSeries
{
    /// <summary>
    /// A time interval [begin, end[
    /// </summary>
    public struct QdbTimeInterval
    {
        /// <summary>
        /// Constructs a time interval [begin, end[
        /// </summary>
        /// <param name="begin">The first date included in the interval</param>
        /// <param name="end">The first date excluded from the interval</param>
        public QdbTimeInterval(DateTime begin, DateTime end)
        {
            Begin = begin;
            End = end;
        }

        /// <summary>
        /// The first date included in the interval
        /// </summary>
        public readonly DateTime Begin;

        /// <summary>
        /// The first date excluded from the interval
        /// </summary>
        public readonly DateTime End;

        /// <summary>
        /// The duration of the interval
        /// </summary>
        public TimeSpan Duration => End - Begin;

        /// <summary>
        /// A instance of QdbTimeInterval that include everything
        /// </summary>
        public static readonly QdbTimeInterval Everything = new QdbTimeInterval(DateTime.MinValue, DateTime.MaxValue);

        /// <summary>
        /// A instance of QdbTimeInterval that include nothing
        /// </summary>
        public static readonly QdbTimeInterval Nothing = new QdbTimeInterval(DateTime.MinValue, DateTime.MinValue);

        internal qdb_ts_range ToNative()
        {
            return new qdb_ts_range
            {
                begin = Begin == DateTime.MinValue
                    ? qdb_timespec.MinValue
                    : Begin == DateTime.MaxValue
                        ? qdb_timespec.MaxValue
                        : TimeConverter.ToTimespec(Begin),
                end = End == DateTime.MinValue
                    ? qdb_timespec.MinValue
                    : End == DateTime.MaxValue
                        ? qdb_timespec.MaxValue
                        : TimeConverter.ToTimespec(End)
            };
        }
    }
}
