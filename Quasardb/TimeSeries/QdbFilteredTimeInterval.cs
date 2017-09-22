using System;
using Quasardb.Native;

namespace Quasardb.TimeSeries
{
    /// <summary>
    /// A filtered time interval [begin, end[
    /// </summary>
    public struct QdbFilteredTimeInterval
    {
        /// <summary>
        /// Constructs a time interval [begin, end[
        /// </summary>
        /// <param name="begin">The first date included in the interval</param>
        /// <param name="end">The first date excluded from the interval</param>
        /// <param name="filter">The filter</param>
        public QdbFilteredTimeInterval(DateTime begin, DateTime end, QdbFilter filter = new QdbFilter())
        {
            Interval = new QdbTimeInterval(begin, end);
            Filter = filter;
        }

        /// <summary>
        /// The actual interval
        /// </summary>
        public readonly QdbTimeInterval Interval;

        /// <summary>
        /// The filter to apply on the interval
        /// </summary>
        public readonly QdbFilter Filter;

        /// <summary>
        /// A instance of QdbFilteredTimeInterval that include everything
        /// </summary>
        public static readonly QdbFilteredTimeInterval Everything = new QdbFilteredTimeInterval(DateTime.MinValue, DateTime.MaxValue, new QdbFilter());

        internal qdb_ts_filtered_range ToNative()
        {
            return new qdb_ts_filtered_range
            {
                range = Interval.ToNative(), filter = Filter.ToNative()
            };
        }
    }
}