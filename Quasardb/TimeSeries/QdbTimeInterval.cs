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

        internal qdb_ts_range ToNative()
        {
            return new qdb_ts_range
            {
                begin = TimeConverter.ToTimespec(Begin),
                end = TimeConverter.ToTimespec(End)
            };
        }
    }
}