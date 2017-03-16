using System;
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
            Api.TimeSeriesInsert(Alias, points.Points);
        }

        #region Average()

        /// <summary>
        /// Gets the average (ie the mean of all values) of the timeseries
        /// </summary>
        /// <returns>The average value of the timeseries</returns>
        public double Average()
        {
            return Average(QdbTimeInterval.Everything);
        }

        /// <summary>
        /// Gets the average (ie the mean of all values) in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The average value or <c>NaN</c> if there is no point in the interval</returns>
        public double Average(QdbTimeInterval interval)
        {
            return AggregateValue(interval, qdb_ts_aggregation_type.Average);
        }

        /// <summary>
        /// Gets the average (ie the mean of all values) in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The average values (<c>NaN</c> when there is no point in an interval)</returns>
        public IEnumerable<double> Average(IEnumerable<QdbTimeInterval> intervals)
        {
            return AggregateValues(intervals, qdb_ts_aggregation_type.Average);
        }

        #endregion

        #region Count()

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
            var res = AggregateValue(interval, qdb_ts_aggregation_type.Count);
            return double.IsNaN(res) ? 0 : (long) res;
        }
        
        /// <summary>
        /// Gets the number of points in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The number of points in each interval</returns>
        public IEnumerable<long> Count(IEnumerable<QdbTimeInterval> intervals)
        {
            var results = AggregateValues(intervals, qdb_ts_aggregation_type.Count);
            foreach (var res in results)
            {
                yield return (long) res;
            }
        }

        #endregion

        #region First()

        /// <summary>
        /// Gets the first point (ie the one with the oldest timestamp) of the timeseries 
        /// </summary>
        /// <returns>The first point of the time series</returns>
        public Point First()
        {
            return First(QdbTimeInterval.Everything);
        }

        /// <summary>
        /// Gets the first point (ie the one with the oldest timestamp) in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The first point in the interval or <c>null</c> if there is no point in the interval</returns>
        public Point First(QdbTimeInterval interval)
        {
            return AggregatePoint(interval, qdb_ts_aggregation_type.First);
        }

        /// <summary>
        /// Gets the first points (ie the one with the oldest timestamp) of each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The first point in each interval (<c>null</c> when there is no point in an interval)</returns>
        public IEnumerable<Point> First(IEnumerable<QdbTimeInterval> intervals)
        {
            return AggregatePoints(intervals, qdb_ts_aggregation_type.First);
        }

        #endregion

        #region Last()

        /// <summary>
        /// Gets the last point (ie the one with the newest timestamp) of the time series
        /// </summary>
        /// <returns>The last point of the time series</returns>
        public Point Last()
        {
            return Last(QdbTimeInterval.Everything);
        }

        /// <summary>
        /// Gets the last point (ie the one with the newest timestamp) in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The last point in the interval or <c>null</c> if there is no point in the interval</returns>
        public Point Last(QdbTimeInterval interval)
        {
            return AggregatePoint(interval, qdb_ts_aggregation_type.Last);
        }

        /// <summary>
        /// Gets the last points (ie the one with the newest timestamp) in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The last point in each interval (<c>null</c> when there is no point in an interval)</returns>
        public IEnumerable<Point> Last(IEnumerable<QdbTimeInterval> intervals)
        {
            return AggregatePoints(intervals, qdb_ts_aggregation_type.Last);
        }

        #endregion

        #region Max()

        /// <summary>
        /// Gets the max point (ie the one with the highest value) of the timeseries
        /// </summary>
        /// <returns>The max point of the time series</returns>
        public Point Max()
        {
            return Max(QdbTimeInterval.Everything);
        }

        /// <summary>
        /// Gets the max point (ie the one with the highest value) in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The max point of the interval or <c>null</c> if there is no point in the interval</returns>
        public Point Max(QdbTimeInterval interval)
        {
            return AggregatePoint(interval, qdb_ts_aggregation_type.Max);
        }

        /// <summary>
        /// Gets the max point (ie the one with the highest value) in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The max point in each interval (<c>null</c> when there is no point in an interval)</returns>
        public IEnumerable<Point> Max(IEnumerable<QdbTimeInterval> intervals)
        {
            return AggregatePoints(intervals, qdb_ts_aggregation_type.Max);
        }

        #endregion

        #region Min()

        /// <summary>
        /// Gets the min point (ie the one with the lowest value) of the timeseries
        /// </summary>
        /// <returns>The min point of the time series</returns>
        public Point Min()
        {
            return Min(QdbTimeInterval.Everything);
        }

        /// <summary>
        /// Gets the min point (ie the one with the lowest value) in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The min point in the interval or <c>null</c> if there is no point in the interval</returns>
        public Point Min(QdbTimeInterval interval)
        {
            return AggregatePoint(interval, qdb_ts_aggregation_type.Min);
        }

        /// <summary>
        /// Gets the min point (ie the one with the lowest value) in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The min point of each interval (<c>null</c> when there is no point in an interval)</returns>
        public IEnumerable<Point> Min(IEnumerable<QdbTimeInterval> intervals)
        {
            return AggregatePoints(intervals, qdb_ts_aggregation_type.Min);
        }

        #endregion

        #region Points

        /// <summary>
        /// Gets all the points in the time series
        /// </summary>
        /// <returns>All the points in the time series</returns>
        public IEnumerable<Point> Points()
        {
            return Points(QdbTimeInterval.Everything);
        }

        /// <summary>
        /// Gets all the points in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>All the points in the interval</returns>
        public IEnumerable<Point> Points(QdbTimeInterval interval)
        {
            return Points(new [] { interval });
        }

        /// <summary>
        /// Gets all the points in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>All the points in each interval</returns>
        public IEnumerable<Point> Points(IEnumerable<QdbTimeInterval> intervals)
        {
            var ranges = new InteropableList<qdb_ts_range>(GetCountOrDefault(intervals));
            foreach (var interval in intervals)
                ranges.Add(interval.ToNative());
            foreach (var pt in Api.TimeSeriesGetPoints(Alias, ranges))
                yield return Point.FromNative(pt);
        }

        #endregion

        #region Sum()

        /// <summary>
        /// Gets the sum (ie the addition of all values) of the timeseries
        /// </summary>
        /// <returns>The sum of the timeseries</returns>
        public double Sum()
        {
            return Sum(QdbTimeInterval.Everything);
        }

        /// <summary>
        /// Gets the sum (ie the addition of all values) of an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The sum of the interval or <c>NaN</c> if there is no point in the interval</returns>
            public double Sum(QdbTimeInterval interval)
        {
            return AggregateValue(interval, qdb_ts_aggregation_type.Sum);
        }

        /// <summary>
        /// Gets the sum (ie the addition of all values) in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The sum in each interval  (<c>NaN</c> when there is no point in an interval)</returns>
        public IEnumerable<double> Sum(IEnumerable<QdbTimeInterval> intervals)
        {
            return AggregateValues(intervals, qdb_ts_aggregation_type.Sum);
        }

        #endregion

        #region Private methods

        Point AggregatePoint(QdbTimeInterval interval, qdb_ts_aggregation_type mode)
        {
            var aggregations = new InteropableList<qdb_ts_aggregation>(1) { MakeAggregation(interval) };
            Api.TimeSeriesAggregate(Alias, mode, aggregations);
            return MakePoint(aggregations[0]);
        }

        IEnumerable<Point> AggregatePoints(IEnumerable<QdbTimeInterval> intervals, qdb_ts_aggregation_type mode)
        {
            var aggregations = new InteropableList<qdb_ts_aggregation>(GetCountOrDefault(intervals));
            foreach (var interval in intervals)
                aggregations.Add(MakeAggregation(interval));

            Api.TimeSeriesAggregate(Alias, mode, aggregations);

            foreach (var aggregation in aggregations)
                yield return MakePoint(aggregation);
        }

        double AggregateValue(QdbTimeInterval interval, qdb_ts_aggregation_type mode)
        {
            var aggregations = new InteropableList<qdb_ts_aggregation>(1) { MakeAggregation(interval) };
            Api.TimeSeriesAggregate(Alias, mode, aggregations);
            return aggregations[0].result.value;
        }

        IEnumerable<double> AggregateValues(IEnumerable<QdbTimeInterval> intervals, qdb_ts_aggregation_type mode)
        {
            var aggregations = new InteropableList<qdb_ts_aggregation>(GetCountOrDefault(intervals));
            foreach (var interval in intervals)
                aggregations.Add(MakeAggregation(interval));

            Api.TimeSeriesAggregate(Alias, mode, aggregations);

            foreach (var aggregation in aggregations)
                yield return aggregation.result.value;
        }

        static Point MakePoint(qdb_ts_aggregation agg)
        {
            return double.IsNaN(agg.result.value) ? null : Point.FromNative(agg.result);
        }

        static qdb_ts_aggregation MakeAggregation(QdbTimeInterval interval)
        {
            return new qdb_ts_aggregation
            {
                range = interval.ToNative()
            };
        }

        static int GetCountOrDefault<T>(IEnumerable<T> source, int defaultCount = 128)
        {
            return (source as ICollection<T>)?.Count ?? defaultCount;
        }

        #endregion
    }
}