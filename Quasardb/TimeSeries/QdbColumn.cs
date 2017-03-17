using System.Collections.Generic;
using Quasardb.Native;

namespace Quasardb.TimeSeries
{
    /// <summary>
    /// A column of a time series
    /// </summary>
    public class QdbColumn
    {
        internal readonly QdbColumnAggregator _aggregator;

        internal QdbColumn(QdbTimeSeries series, string name)
        {
            Series = series;
            Name = name;
            _aggregator = new QdbColumnAggregator(this);
        }

        /// <summary>
        /// The parent of the column
        /// </summary>
        public QdbTimeSeries Series { get; }

        /// <summary>
        /// The name of the column
        /// </summary>
        public string Name { get; }

        #region Count

        /// <summary>
        /// Gets the number of points in the time series
        /// </summary>
        /// <returns>The number of points in the time series</returns>
        public long Count()
        {
            return Count(QdbTimeInterval.Everything);
        }

        /// <summary>
        /// Gets the number of points in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The number of points in the interval</returns>
        public long Count(QdbTimeInterval interval)
        {
            return _aggregator.Aggregate(interval, qdb_ts_aggregation_type.Count).ToLong();
        }

        /// <summary>
        /// Gets the number of points in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The number of points in each interval</returns>
        public IEnumerable<long> Count(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.Aggregate(intervals, qdb_ts_aggregation_type.Count).ToLong();
        }

        #endregion
    }
}