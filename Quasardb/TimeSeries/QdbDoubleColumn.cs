using System;
using System.Collections.Generic;
using System.Diagnostics;
using Quasardb.Exceptions;
using Quasardb.Native;

using Point = Quasardb.TimeSeries.QdbDoublePoint;

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
            var error = qdb_api.qdb_ts_double_insert(Handle, Series.Alias, Name, points.Points.Buffer, points.Points.Count);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: Series.Alias);
        }

        /// <summary>
        /// Inserts one or more points in the time series
        /// </summary>
        /// <param name="points">The points to insert</param>
        public void Insert(IEnumerable<Point> points)
        {
            Insert(new QdbDoublePointCollection(points));
        }

        /// <summary>
        /// Inserts one or more points in the time series
        /// </summary>
        /// <param name="points">The points to insert</param>
        public void Insert(params Point[] points)
        {
            Insert((IEnumerable<Point>)points);
        }

        /// <summary>
        /// Inserts one point in the time series
        /// </summary>
        /// <param name="time">The timestamp of the point to insert</param>
        /// <param name="value">The value of the point to insert</param>
        public void Insert(DateTime time, double value)
        {
            Insert(new Point(time, value));
        }

        #endregion

        #region Average()

        /// <summary>
        /// Gets the average (ie the mean of all values) of the timeseries
        /// </summary>
        /// <returns>The average value of the timeseries</returns>
        /// <exception cref="QdbEmptyColumnException">If the column is empty</exception>
        public double Average()
        {
            var result = _aggregator.Aggregate(qdb_ts_aggregation_type.Average).ToDouble();
            if (double.IsNaN(result))
                throw new QdbEmptyColumnException(Series.Alias, Name);
            return result;
        }

        /// <summary>
        /// Gets the average (ie the mean of all values) in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The average value or <c>NaN</c> if there is no point in the interval</returns>
        public double Average(QdbTimeInterval interval)
        {
            return _aggregator.Aggregate(qdb_ts_aggregation_type.Average, interval).ToDouble();
        }

        /// <summary>
        /// Gets the average (ie the mean of all values) in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The average values (<c>NaN</c> when there is no point in an interval)</returns>
        public IEnumerable<double> Average(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.Aggregate(qdb_ts_aggregation_type.Average, intervals).ToDouble();
        }

        #endregion

        #region First()

        /// <summary>
        /// Gets the first point (ie the one with the oldest timestamp) of the timeseries 
        /// </summary>
        /// <returns>The first point of the time series</returns>
        /// <exception cref="QdbEmptyColumnException">If the column is empty</exception>
        public Point First()
        {
            var point = _aggregator.Aggregate(qdb_ts_aggregation_type.First).ToDoublePoint();
            if (point == null)
                throw new QdbEmptyColumnException(Series.Alias, Name);
            return point;
        }

        /// <summary>
        /// Gets the first point (ie the one with the oldest timestamp) in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The first point in the interval or <c>null</c> if there is no point in the interval</returns>
        public Point First(QdbTimeInterval interval)
        {
            return _aggregator.Aggregate(qdb_ts_aggregation_type.First, interval).ToDoublePoint();
        }

        /// <summary>
        /// Gets the first points (ie the one with the oldest timestamp) of each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The first point in each interval (<c>null</c> when there is no point in an interval)</returns>
        public IEnumerable<Point> First(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.Aggregate(qdb_ts_aggregation_type.First, intervals).ToDoublePoint();
        }

        #endregion

        #region Last()

        /// <summary>
        /// Gets the last point (ie the one with the newest timestamp) of the time series
        /// </summary>
        /// <returns>The last point of the time series</returns>
        /// <exception cref="QdbEmptyColumnException">If the column is empty</exception>
        public Point Last()
        {
            var point = _aggregator.Aggregate(qdb_ts_aggregation_type.Last).ToDoublePoint();
            if (point == null)
                throw new QdbEmptyColumnException(Series.Alias, Name);
            return point;
        }

        /// <summary>
        /// Gets the last point (ie the one with the newest timestamp) in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The last point in the interval or <c>null</c> if there is no point in the interval</returns>
        public Point Last(QdbTimeInterval interval)
        {
            return _aggregator.Aggregate(qdb_ts_aggregation_type.Last, interval).ToDoublePoint();
        }

        /// <summary>
        /// Gets the last points (ie the one with the newest timestamp) in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The last point in each interval (<c>null</c> when there is no point in an interval)</returns>
        public IEnumerable<Point> Last(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.Aggregate(qdb_ts_aggregation_type.Last, intervals).ToDoublePoint();
        }

        #endregion

        #region Max()

        /// <summary>
        /// Gets the max point (ie the one with the highest value) of the timeseries
        /// </summary>
        /// <returns>The max point of the time series</returns>
        /// <exception cref="QdbEmptyColumnException">If the column is empty</exception>
        public Point Max()
        {
            var point = _aggregator.Aggregate(qdb_ts_aggregation_type.Max).ToDoublePoint();
            if (point == null)
                throw new QdbEmptyColumnException(Series.Alias, Name);
            return point;
        }

        /// <summary>
        /// Gets the max point (ie the one with the highest value) in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The max point of the interval or <c>null</c> if there is no point in the interval</returns>
        public Point Max(QdbTimeInterval interval)
        {
            return _aggregator.Aggregate(qdb_ts_aggregation_type.Max, interval).ToDoublePoint();
        }

        /// <summary>
        /// Gets the max point (ie the one with the highest value) in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The max point in each interval (<c>null</c> when there is no point in an interval)</returns>
        public IEnumerable<Point> Max(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.Aggregate(qdb_ts_aggregation_type.Max, intervals).ToDoublePoint();
        }

        #endregion

        #region Min()

        /// <summary>
        /// Gets the min point (ie the one with the lowest value) of the timeseries
        /// </summary>
        /// <returns>The min point of the time series</returns>
        /// <exception cref="QdbEmptyColumnException">If the column is empty</exception>
        public Point Min()
        {
            var point = _aggregator.Aggregate(qdb_ts_aggregation_type.Min).ToDoublePoint();
            if (point == null)
                throw new QdbEmptyColumnException(Series.Alias, Name);
            return point;
        }

        /// <summary>
        /// Gets the min point (ie the one with the lowest value) in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The min point in the interval or <c>null</c> if there is no point in the interval</returns>
        public Point Min(QdbTimeInterval interval)
        {
            return _aggregator.Aggregate(qdb_ts_aggregation_type.Min, interval).ToDoublePoint();
        }

        /// <summary>
        /// Gets the min point (ie the one with the lowest value) in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The min point of each interval (<c>null</c> when there is no point in an interval)</returns>
        public IEnumerable<Point> Min(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.Aggregate(qdb_ts_aggregation_type.Min, intervals).ToDoublePoint();
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
            var ranges = new InteropableList<qdb_ts_range>(Helpers.GetCountOrDefault(intervals));
            foreach (var interval in intervals)
                ranges.Add(interval.ToNative());
            using (var points = new qdb_buffer<qdb_ts_double_point>(Handle))
            {
                var error = qdb_api.qdb_ts_double_get_range(Handle, Series.Alias, Name, ranges.Buffer, ranges.Count,
                    out points.Pointer, out points.Size);
                QdbExceptionThrower.ThrowIfNeeded(error, alias: Series.Alias, column: Name);

                foreach (var pt in points)
                    yield return pt.ToManaged();
            }
        }

        #endregion

        #region Sum()

        /// <summary>
        /// Gets the sum (ie the addition of all values) of the timeseries
        /// </summary>
        /// <returns>The sum of the timeseries</returns>
        public double Sum()
        {
            return _aggregator.Aggregate(qdb_ts_aggregation_type.Sum).ToDouble();
        }

        /// <summary>
        /// Gets the sum (ie the addition of all values) of an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The sum of the interval or <c>NaN</c> if there is no point in the interval</returns>
        public double Sum(QdbTimeInterval interval)
        {
            return _aggregator.Aggregate(qdb_ts_aggregation_type.Sum, interval).ToDouble();
        }

        /// <summary>
        /// Gets the sum (ie the addition of all values) in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The sum in each interval  (<c>NaN</c> when there is no point in an interval)</returns>
        public IEnumerable<double> Sum(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.Aggregate(qdb_ts_aggregation_type.Sum, intervals).ToDouble();
        }

        #endregion
    }
}