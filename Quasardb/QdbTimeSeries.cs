using System;
using System.Collections;
using System.Collections.Generic;
using Quasardb.ManagedApi;
using Quasardb.NativeApi;

namespace Quasardb
{
    /// <summary>
    /// A time series in a quasardb database.
    /// </summary>
    /// <remarks>
    /// QdbTimeSeries can be constructed via <see cref="QdbCluster.TimeSeries" />.
    /// </remarks>
    public sealed partial class QdbTimeSeries : QdbEntry
    {
        internal QdbTimeSeries(QdbApi api, string alias) : base(api, alias)
        {
        }

        /// <summary>
        /// Inserts one or more points in the time series
        /// </summary>
        /// <param name="points">The points to insert</param>
        public void Insert(PointCollection points)
        {
            Api.TimeSeriesInsert(Alias, points.GetPoints());
        }
        
        /// <summary>
        /// Gets the average (ie the mean of all values) of the timeseries
        /// </summary>
        /// <returns>The average of the timeseries</returns>
        public double Average()
        {
            return AggregateValue(qdb_timespec.MinValue, qdb_timespec.MaxValue, qdb_ts_aggregation_type.Average);
        }

        /// <summary>
        /// Gets the average (ie the mean of all values) of the interval [begin, end[
        /// </summary>
        /// <returns>The average of the interval</returns>
        public double Average(DateTime begin, DateTime end)
        {
            return AggregateValue(TimeConverter.ToTimespec(begin), TimeConverter.ToTimespec(end), qdb_ts_aggregation_type.Average);
        }

        /// <summary>
        /// Gets the number of points in the time series
        /// </summary>
        /// <returns>The number of points in the time series</returns>
        public long Count()
        {
            return (long)AggregateValue(qdb_timespec.MinValue, qdb_timespec.MaxValue, qdb_ts_aggregation_type.Count);
        }

        /// <summary>
        /// Gets the number of points in the interval [begin, end[
        /// </summary>
        /// <returns>The number of points the interval</returns>
        public long Count(DateTime begin, DateTime end)
        {
            var res = AggregateValue(TimeConverter.ToTimespec(begin), TimeConverter.ToTimespec(end), qdb_ts_aggregation_type.Count);
            return double.IsNaN(res) ? 0 : (long) res;
        }

        /// <summary>
        /// Gets the first point (ie the one with the oldest timestamp) of the timeseries 
        /// </summary>
        /// <returns>The first point of the time series</returns>
        public Point First()
        {
            return AggregatePoint(qdb_timespec.MinValue, qdb_timespec.MaxValue, qdb_ts_aggregation_type.First);
        }

        /// <summary>
        /// Gets the first point (ie the one with the oldest timestamp) in the interval [begin, end[
        /// </summary>
        /// <returns>The first point of the interval</returns>
        public Point First(DateTime begin, DateTime end)
        {
            return AggregatePoint(TimeConverter.ToTimespec(begin), TimeConverter.ToTimespec(end), qdb_ts_aggregation_type.First);
        }

        /// <summary>
        /// Gets the last point (ie the one with the newest timestamp) in the time series
        /// </summary>
        /// <returns>The last point of the time series</returns>
        public Point Last()
        {
            return AggregatePoint(qdb_timespec.MinValue, qdb_timespec.MaxValue, qdb_ts_aggregation_type.Last);
        }

        /// <summary>
        /// Gets the last point (ie the one with the newest timestamp) in the interval [begin, end[
        /// </summary>
        /// <returns>The last point of the interval</returns>
        public Point Last(DateTime begin, DateTime end)
        {
            return AggregatePoint(TimeConverter.ToTimespec(begin), TimeConverter.ToTimespec(end), qdb_ts_aggregation_type.Last);
        }

        /// <summary>
        /// Gets the max point (ie the one with the highest value) in the timeseries
        /// </summary>
        /// <returns>The max point of the time series</returns>
        public Point Max()
        {
            return AggregatePoint(qdb_timespec.MinValue, qdb_timespec.MaxValue, qdb_ts_aggregation_type.Max);
        }

        /// <summary>
        /// Gets the max point (ie the one with the highest value) in the interval [begin, end[
        /// </summary>
        /// <returns>The max point of the interval</returns>
        public Point Max(DateTime begin, DateTime end)
        {
            return AggregatePoint(TimeConverter.ToTimespec(begin), TimeConverter.ToTimespec(end), qdb_ts_aggregation_type.Max);
        }

        /// <summary>
        /// Gets the min point (ie the one with the lowest value) in the timeseries
        /// </summary>
        /// <returns>The min point of the time series</returns>
        public Point Min()
        {
            return AggregatePoint(qdb_timespec.MinValue, qdb_timespec.MaxValue, qdb_ts_aggregation_type.Min);
        }

        /// <summary>
        /// Gets the min point (ie the one with the lowest value) in the interval [begin, end[
        /// </summary>
        /// <returns>The min point of the interval</returns>
        public Point Min(DateTime begin, DateTime end)
        {
            return AggregatePoint(TimeConverter.ToTimespec(begin), TimeConverter.ToTimespec(end), qdb_ts_aggregation_type.Min);
        }

        /// <summary>
        /// Gets the sum (ie the addition of all values) of the timeseries
        /// </summary>
        /// <returns>The sum of the timeseries</returns>
        public double Sum()
        {
            return AggregateValue(qdb_timespec.MinValue, qdb_timespec.MaxValue, qdb_ts_aggregation_type.Sum);
        }

        /// <summary>
        /// Gets the sum (ie the addition of all values) of the interval [begin, end[
        /// </summary>
        /// <returns>The sum of the interval</returns>
        public double Sum(DateTime begin, DateTime end)
        {
            return AggregateValue(TimeConverter.ToTimespec(begin), TimeConverter.ToTimespec(end), qdb_ts_aggregation_type.Sum);
        }

        Point AggregatePoint(qdb_timespec begin, qdb_timespec end, qdb_ts_aggregation_type mode)
        {
            var aggregations = new qdb_ts_aggregation[1];
            aggregations[0].begin = begin;
            aggregations[0].end = end;
            Api.TimeSeriesAggregate(Alias, mode, aggregations);
            return MakePoint(aggregations[0]);
        }

        double AggregateValue(qdb_timespec begin, qdb_timespec end, qdb_ts_aggregation_type mode)
        {
            var aggregations = new qdb_ts_aggregation[1];
            aggregations[0].begin = begin;
            aggregations[0].end = end;
            Api.TimeSeriesAggregate(Alias, mode, aggregations);
            return aggregations[0].result_value;
        }

        static Point MakePoint(qdb_ts_aggregation aggregation)
        {
            return double.IsNaN(aggregation.result_value)
                ? null
                : new Point(TimeConverter.ToDateTime(aggregation.result_timestamp), aggregation.result_value);
        }
}
}