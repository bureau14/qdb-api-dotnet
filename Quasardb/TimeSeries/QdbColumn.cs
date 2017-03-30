using System.Collections.Generic;
using Quasardb.Exceptions;
using Quasardb.Native;

namespace Quasardb.TimeSeries
{
    /// <summary>
    /// A column of a time series
    /// </summary>
    public abstract class QdbColumn
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

        internal qdb_handle Handle => Series.Handle;

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
            return _aggregator.Aggregate(qdb_ts_aggregation_type.Count, interval).ToLong();
        }

        /// <summary>
        /// Gets the number of points in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The number of points in each interval</returns>
        public IEnumerable<long> Count(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.Aggregate(qdb_ts_aggregation_type.Count, intervals).ToLong();
        }

        #endregion
    }

    class QdbUnknownColumn : QdbColumn
    {
        public readonly int Type;

        public QdbUnknownColumn(QdbTimeSeries series, string name, qdb_ts_column_type type) : base(series, name)
        {
            Type = (int) type;
        }
    }
}