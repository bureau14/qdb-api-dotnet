using System.Collections.Generic;
using Quasardb.Exceptions;
using Quasardb.Native;

namespace Quasardb.TimeSeries
{
    /// <summary>
    /// A column of a time series
    /// </summary>
    public class QdbColumn
    {
        internal readonly QdbColumnAggregator _aggregator;
        internal readonly string _alias; // TODO: remove once columns are supported in qdb_api.dll

        internal QdbColumn(QdbTimeSeries series, string name)
        {
            Series = series;
            Name = name;
            _alias = series.Alias + "." + name;
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

        /// <summary>
        /// Removes the column from the timeseries.
        /// </summary>
        /// <returns><c>true</c> if the entry was removed, or <c>false</c> if the entry didn't exist.</returns>
        public virtual bool Remove()
        {
            var error = qdb_api.qdb_remove(Handle, _alias);

            switch (error)
            {
                case qdb_error_t.qdb_e_ok:
                    return true;

                case qdb_error_t.qdb_e_alias_not_found:
                    return false;

                default:
                    throw QdbExceptionFactory.Create(error, alias: Series.Alias);
            }
        }

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