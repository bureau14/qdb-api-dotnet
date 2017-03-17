using System;
using System.Collections.Generic;
using Quasardb.ManagedApi;
using Quasardb.Native;

namespace Quasardb.TimeSeries
{
    /// <summary>
    /// A column of a time series in a quasardb database.
    /// </summary>
    public sealed class QdbDoubleColumn : QdbColumn
    {
        internal QdbDoubleColumn(QdbTimeSeries series, string name) : base(series, name)
        {
        }

        #region Insert()

        /// <summary>
        /// Inserts one or more points in the time series
        /// </summary>
        /// <param name="points">The points to insert</param>
        public void Insert(QdbDoublePointCollection points)
        {
            Series.Api.TsDoubleInsert(Series.Alias, Name, points.Points);
        }

        /// <summary>
        /// Inserts one or more points in the time series
        /// </summary>
        /// <param name="points">The points to insert</param>
        public void Insert(IEnumerable<QdbPoint<double>> points)
        {
            Insert(new QdbDoublePointCollection(points));
        }

        /// <summary>
        /// Inserts one or more points in the time series
        /// </summary>
        /// <param name="points">The points to insert</param>
        public void Insert(params QdbPoint<double>[] points)
        {
            Insert((IEnumerable<QdbPoint<double>>)points);
        }

        /// <summary>
        /// Inserts one point in the time series
        /// </summary>
        /// <param name="time">The timestamp of the point to insert</param>
        /// <param name="value">The value of the point to insert</param>
        public void Insert(DateTime time, double value)
        {
            Insert(new QdbPoint<double>(time, value));
        }

        #endregion

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
            return _aggregator.Aggregate(interval, qdb_ts_aggregation_type.Average).ToDouble();
        }

        /// <summary>
        /// Gets the average (ie the mean of all values) in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The average values (<c>NaN</c> when there is no point in an interval)</returns>
        public IEnumerable<double> Average(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.Aggregate(intervals, qdb_ts_aggregation_type.Average).ToDouble();
        }

        #endregion

        #region First()

        /// <summary>
        /// Gets the first point (ie the one with the oldest timestamp) of the timeseries 
        /// </summary>
        /// <returns>The first point of the time series</returns>
        public QdbPoint<double> First()
        {
            return First(QdbTimeInterval.Everything);
        }

        /// <summary>
        /// Gets the first point (ie the one with the oldest timestamp) in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The first point in the interval or <c>null</c> if there is no point in the interval</returns>
        public QdbPoint<double> First(QdbTimeInterval interval)
        {
            return _aggregator.Aggregate(interval, qdb_ts_aggregation_type.First).ToDoublePoint();
        }

        /// <summary>
        /// Gets the first points (ie the one with the oldest timestamp) of each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The first point in each interval (<c>null</c> when there is no point in an interval)</returns>
        public IEnumerable<QdbPoint<double>> First(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.Aggregate(intervals, qdb_ts_aggregation_type.First).ToDoublePoint();
        }

        #endregion

        #region Last()

        /// <summary>
        /// Gets the last point (ie the one with the newest timestamp) of the time series
        /// </summary>
        /// <returns>The last point of the time series</returns>
        public QdbPoint<double> Last()
        {
            return Last(QdbTimeInterval.Everything);
        }

        /// <summary>
        /// Gets the last point (ie the one with the newest timestamp) in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The last point in the interval or <c>null</c> if there is no point in the interval</returns>
        public QdbPoint<double> Last(QdbTimeInterval interval)
        {
            return _aggregator.Aggregate(interval, qdb_ts_aggregation_type.Last).ToDoublePoint();
        }

        /// <summary>
        /// Gets the last points (ie the one with the newest timestamp) in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The last point in each interval (<c>null</c> when there is no point in an interval)</returns>
        public IEnumerable<QdbPoint<double>> Last(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.Aggregate(intervals, qdb_ts_aggregation_type.Last).ToDoublePoint();
        }

        #endregion

        #region Max()

        /// <summary>
        /// Gets the max point (ie the one with the highest value) of the timeseries
        /// </summary>
        /// <returns>The max point of the time series</returns>
        public QdbPoint<double> Max()
        {
            return Max(QdbTimeInterval.Everything);
        }

        /// <summary>
        /// Gets the max point (ie the one with the highest value) in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The max point of the interval or <c>null</c> if there is no point in the interval</returns>
        public QdbPoint<double> Max(QdbTimeInterval interval)
        {
            return _aggregator.Aggregate(interval, qdb_ts_aggregation_type.Max).ToDoublePoint();
        }

        /// <summary>
        /// Gets the max point (ie the one with the highest value) in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The max point in each interval (<c>null</c> when there is no point in an interval)</returns>
        public IEnumerable<QdbPoint<double>> Max(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.Aggregate(intervals, qdb_ts_aggregation_type.Max).ToDoublePoint();
        }

        #endregion

        #region Min()

        /// <summary>
        /// Gets the min point (ie the one with the lowest value) of the timeseries
        /// </summary>
        /// <returns>The min point of the time series</returns>
        public QdbPoint<double> Min()
        {
            return Min(QdbTimeInterval.Everything);
        }

        /// <summary>
        /// Gets the min point (ie the one with the lowest value) in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The min point in the interval or <c>null</c> if there is no point in the interval</returns>
        public QdbPoint<double> Min(QdbTimeInterval interval)
        {
            return _aggregator.Aggregate(interval, qdb_ts_aggregation_type.Min).ToDoublePoint();
        }

        /// <summary>
        /// Gets the min point (ie the one with the lowest value) in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The min point of each interval (<c>null</c> when there is no point in an interval)</returns>
        public IEnumerable<QdbPoint<double>> Min(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.Aggregate(intervals, qdb_ts_aggregation_type.Min).ToDoublePoint();
        }

        #endregion

        #region Points

        /// <summary>
        /// Gets all the points in the time series
        /// </summary>
        /// <returns>All the points in the time series</returns>
        public IEnumerable<QdbPoint<double>> Points()
        {
            return Points(QdbTimeInterval.Everything);
        }

        /// <summary>
        /// Gets all the points in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>All the points in the interval</returns>
        public IEnumerable<QdbPoint<double>> Points(QdbTimeInterval interval)
        {
            return Points(new [] { interval });
        }

        /// <summary>
        /// Gets all the points in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>All the points in each interval</returns>
        public IEnumerable<QdbPoint<double>> Points(IEnumerable<QdbTimeInterval> intervals)
        {
            var ranges = new InteropableList<qdb_ts_range>(Helpers.GetCountOrDefault(intervals));
            foreach (var interval in intervals)
                ranges.Add(interval.ToNative());
            foreach (var pt in Series.Api.TimeSeriesGetPoints(Series.Alias, Name, ranges))
                yield return PointConverter.ToManaged(pt);
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
            return _aggregator.Aggregate(interval, qdb_ts_aggregation_type.Sum).ToDouble();
        }

        /// <summary>
        /// Gets the sum (ie the addition of all values) in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The sum in each interval  (<c>NaN</c> when there is no point in an interval)</returns>
        public IEnumerable<double> Sum(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.Aggregate(intervals, qdb_ts_aggregation_type.Sum).ToDouble();
        }

        #endregion
    }
}