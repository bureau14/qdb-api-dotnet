using System;
using System.Collections.Generic;
using System.Diagnostics;
using Quasardb.Exceptions;
using Quasardb.Native;

using Point = Quasardb.TimeSeries.QdbDoublePoint;

namespace Quasardb.TimeSeries
{
    /// <summary>
    /// A column of a table in a quasardb database.
    /// </summary>
    public sealed class QdbDoubleColumn : QdbColumn
    {
        internal QdbDoubleColumn(QdbTable series, string name) : base(series, name)
        {
        }

        #region Insert()

        /// <summary>
        /// Inserts one or more points in the table
        /// </summary>
        /// <param name="points">The points to insert</param>
        public void Insert(QdbDoublePointCollection points)
        {
            var error = qdb_api.qdb_ts_double_insert(Handle, Series.Alias, Name, points.Points.Buffer, points.Points.Count);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: Series.Alias, column: Name);
        }

        /// <summary>
        /// Inserts one or more points in the table
        /// </summary>
        /// <param name="points">The points to insert</param>
        public void Insert(IEnumerable<Point> points)
        {
            Insert(new QdbDoublePointCollection(points));
        }

        /// <summary>
        /// Inserts one or more points in the table
        /// </summary>
        /// <param name="points">The points to insert</param>
        public void Insert(params Point[] points)
        {
            Insert((IEnumerable<Point>)points);
        }

        /// <summary>
        /// Inserts one point in the table
        /// </summary>
        /// <param name="time">The timestamp of the point to insert</param>
        /// <param name="value">The value of the point to insert</param>
        public void Insert(DateTime time, double value)
        {
            Insert(new Point(time, value));
        }

        #endregion

        #region InsertTruncate

        /// <summary>
        /// Inserts one or more points in the table and erases given
        /// ranges in the same transaction
        /// </summary>
        /// <param name="intervals">The time intervals to erase</param>
        /// <param name="points">The points to insert</param>
        public void InsertTruncate(IEnumerable<QdbTimeInterval> intervals, QdbDoublePointCollection points)
        {
            var ranges = new InteropableList<qdb_ts_range>(Helpers.GetCountOrDefault(intervals));
            foreach (var interval in intervals)
                ranges.Add(interval.ToNative());
            var error = qdb_api.qdb_ts_double_insert_truncate(
                Handle, Series.Alias, Name,
                ranges.Buffer, ranges.Count,
                points.Points.Buffer, points.Points.Count);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: Series.Alias, column: Name);
        }

        /// <summary>
        /// Inserts one or more points in the table and erases the given
        /// range in the same transaction
        /// </summary>
        /// <param name="interval">The time interval to erase</param>
        /// <param name="points">The points to insert</param>
        public void InsertTruncate(QdbTimeInterval interval, QdbDoublePointCollection points)
        {
            InsertTruncate(new[] { interval }, points);
        }

        /// <summary>
        /// Inserts one or more points in the table and erases given
        /// ranges in the same transaction
        /// </summary>
        /// <param name="intervals">The time intervals to erase</param>
        /// <param name="points">The points to insert</param>
        public void InsertTruncate(IEnumerable<QdbTimeInterval> intervals, IEnumerable<Point> points)
        {
            InsertTruncate(intervals, new QdbDoublePointCollection(points));
        }

        /// <summary>
        /// Inserts one or more points in the table and erases the given
        /// range in the same transaction
        /// </summary>
        /// <param name="interval">The time interval to erase</param>
        /// <param name="points">The points to insert</param>
        public void InsertTruncate(QdbTimeInterval interval, IEnumerable<Point> points)
        {
            InsertTruncate(new[] { interval }, new QdbDoublePointCollection(points));
        }

        /// <summary>
        /// Inserts one or more points in the table and erases given
        /// ranges in the same transaction
        /// </summary>
        /// <param name="intervals">The time intervals to erase</param>
        /// <param name="points">The points to insert</param>
        public void InsertTruncate(IEnumerable<QdbTimeInterval> intervals, params Point[] points)
        {
            InsertTruncate(intervals, (IEnumerable<Point>)points);
        }

        /// <summary>
        /// Inserts one or more points in the table and erases the given
        /// range in the same transaction
        /// </summary>
        /// <param name="interval">The time interval to erase</param>
        /// <param name="points">The points to insert</param>
        public void InsertTruncate(QdbTimeInterval interval, params Point[] points)
        {
            InsertTruncate(new[] { interval }, (IEnumerable<Point>)points);
        }

        /// <summary>
        /// Inserts one point in the table and erases given ranges in
        /// the same transaction
        /// </summary>
        /// <param name="intervals">The time intervals to erase</param>
        /// <param name="time">The timestamp of the point to insert</param>
        /// <param name="value">The value of the point to insert</param>
        public void InsertTruncate(IEnumerable<QdbTimeInterval> intervals, DateTime time, double value)
        {
            InsertTruncate(intervals, new Point(time, value));
        }

        /// <summary>
        /// Inserts one point in the table and erases the given range in
        /// the same transaction
        /// </summary>
        /// <param name="interval">The time interval to erase</param>
        /// <param name="time">The timestamp of the point to insert</param>
        /// <param name="value">The value of the point to insert</param>
        public void InsertTruncate(QdbTimeInterval interval, DateTime time, double value)
        {
            InsertTruncate(new[] { interval }, new Point(time, value));
        }

        #endregion

        #region Points

        /// <summary>
        /// Gets all the points in the table
        /// </summary>
        /// <returns>All the points in the table</returns>
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
            return Points(new[] { interval });
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
                var error = qdb_api.qdb_ts_double_get_ranges(Handle, Series.Alias, Name, ranges.Buffer, ranges.Count,
                    out points.Pointer, out points.Size);
                QdbExceptionThrower.ThrowIfNeeded(error, alias: Series.Alias, column: Name);

                foreach (var pt in points)
                    yield return pt.ToManaged();
            }
        }

        #endregion

        #region First()

        /// <summary>
        /// Gets the first point (ie the one with the oldest timestamp) of the table
        /// </summary>
        /// <returns>The first point of the table</returns>
        /// <exception cref="QdbEmptyColumnException">If the column is empty</exception>
        public Point First()
        {
            var point = _aggregator.DoubleAggregate(qdb_ts_aggregation_type.First).AsPoint();
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
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.First, interval).AsPoint();
        }

        /// <summary>
        /// Gets the first points (ie the one with the oldest timestamp) of each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The first point in each interval (<c>null</c> when there is no point in an interval)</returns>
        public IEnumerable<Point> First(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.First, intervals).AsPoint();
        }

        #endregion

        #region Last()

        /// <summary>
        /// Gets the last point (ie the one with the newest timestamp) of the table
        /// </summary>
        /// <returns>The last point of the table</returns>
        /// <exception cref="QdbEmptyColumnException">If the column is empty</exception>
        public Point Last()
        {
            var point = _aggregator.DoubleAggregate(qdb_ts_aggregation_type.Last).AsPoint();
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
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.Last, interval).AsPoint();
        }

        /// <summary>
        /// Gets the last points (ie the one with the newest timestamp) in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The last point in each interval (<c>null</c> when there is no point in an interval)</returns>
        public IEnumerable<Point> Last(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.Last, intervals).AsPoint();
        }

        #endregion

        #region Min()

        /// <summary>
        /// Gets the point with the smallest value of the table
        /// </summary>
        /// <returns>The point with the smallest value of the table</returns>
        /// <exception cref="QdbEmptyColumnException">If the column is empty</exception>
        public Point Min()
        {
            var point = _aggregator.DoubleAggregate(qdb_ts_aggregation_type.Min).AsPoint();
            if (point == null)
                throw new QdbEmptyColumnException(Series.Alias, Name);
            return point;
        }

        /// <summary>
        /// Gets the point with the smallest value in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The point with the smallest value in the interval or <c>null</c> if there is no point in the interval</returns>
        public Point Min(QdbTimeInterval interval)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.Min, interval).AsPoint();
        }

        /// <summary>
        /// Gets the point with the smallest value in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The point with the smallest value of each interval (<c>null</c> when there is no point in an interval)</returns>
        public IEnumerable<Point> Min(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.Min, intervals).AsPoint();
        }

        #endregion

        #region Max()

        /// <summary>
        /// Gets the point with the largest value of the table
        /// </summary>
        /// <returns>The point with the largest value of the table</returns>
        /// <exception cref="QdbEmptyColumnException">If the column is empty</exception>
        public Point Max()
        {
            var point = _aggregator.DoubleAggregate(qdb_ts_aggregation_type.Max).AsPoint();
            if (point == null)
                throw new QdbEmptyColumnException(Series.Alias, Name);
            return point;
        }

        /// <summary>
        /// Gets the point with the largest value in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The point with the largest value of the interval or <c>null</c> if there is no point in the interval</returns>
        public Point Max(QdbTimeInterval interval)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.Max, interval).AsPoint();
        }

        /// <summary>
        /// Gets the point with the largest value in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The point with the largest value in each interval (<c>null</c> when there is no point in an interval)</returns>
        public IEnumerable<Point> Max(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.Max, intervals).AsPoint();
        }

        #endregion

        #region Average()

        /// <summary>
        /// Gets the average (ie the mean of all values) of the table
        /// </summary>
        /// <returns>The average value of the table</returns>
        /// <exception cref="QdbEmptyColumnException">If the column is empty</exception>
        public double Average()
        {
            var result = _aggregator.DoubleAggregate(qdb_ts_aggregation_type.Average);

            if (result.Count() == 0)
                throw new QdbEmptyColumnException(Series.Alias, Name);
            return result.Value();
        }

        /// <summary>
        /// Gets the average (ie the mean of all values) in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The average value or <c>NaN</c> if there is no point in the interval</returns>
        public double Average(QdbTimeInterval interval)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.Average, interval).Value();
        }

        /// <summary>
        /// Gets the average (ie the mean of all values) in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The average values (<c>NaN</c> when there is no point in an interval)</returns>
        public IEnumerable<double> Average(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.Average, intervals).Value();
        }

        #endregion

        #region HarmonicMean()

        /// <summary>
        /// Gets the harmonic mean (ie the mean of all values) of the table
        /// </summary>
        /// <returns>The harmonic mean value of the table</returns>
        /// <exception cref="QdbEmptyColumnException">If the column is empty</exception>
        public double HarmonicMean()
        {
            var result = _aggregator.DoubleAggregate(qdb_ts_aggregation_type.HarmonicMean);

            if (result.Count() == 0)
                throw new QdbEmptyColumnException(Series.Alias, Name);
            return result.Value();
        }

        /// <summary>
        /// Gets the harmonic mean (ie the mean of all values) in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The harmonic mean value or <c>NaN</c> if there is no point in the interval</returns>
        public double HarmonicMean(QdbTimeInterval interval)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.HarmonicMean, interval).Value();
        }

        /// <summary>
        /// Gets the harmonic mean (ie the mean of all values) in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The harmonic mean values (<c>NaN</c> when there is no point in an interval)</returns>
        public IEnumerable<double> HarmonicMean(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.HarmonicMean, intervals).Value();
        }

        #endregion

        #region GeometricMean()

        /// <summary>
        /// Gets the geometric mean (ie the mean of all values) of the table
        /// </summary>
        /// <returns>The geometric mean value of the table</returns>
        /// <exception cref="QdbEmptyColumnException">If the column is empty</exception>
        public double GeometricMean()
        {
            var result = _aggregator.DoubleAggregate(qdb_ts_aggregation_type.GeometricMean);

            if (result.Count() == 0)
                throw new QdbEmptyColumnException(Series.Alias, Name);
            return result.Value();
        }

        /// <summary>
        /// Gets the geometric mean (ie the mean of all values) in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The geometric mean value or <c>NaN</c> if there is no point in the interval</returns>
        public double GeometricMean(QdbTimeInterval interval)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.GeometricMean, interval).Value();
        }

        /// <summary>
        /// Gets the geometric mean (ie the mean of all values) in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The geometric mean values (<c>NaN</c> when there is no point in an interval)</returns>
        public IEnumerable<double> GeometricMean(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.GeometricMean, intervals).Value();
        }

        #endregion

        #region QuadraticMean()

        /// <summary>
        /// Gets the quadratic mean (ie the mean of all values) of the table
        /// </summary>
        /// <returns>The quadratic mean value of the table</returns>
        /// <exception cref="QdbEmptyColumnException">If the column is empty</exception>
        public double QuadraticMean()
        {
            var result = _aggregator.DoubleAggregate(qdb_ts_aggregation_type.QuadraticMean);

            if (result.Count() == 0)
                throw new QdbEmptyColumnException(Series.Alias, Name);
            return result.Value();
        }

        /// <summary>
        /// Gets the quadratic mean (ie the mean of all values) in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The quadratic mean value or <c>NaN</c> if there is no point in the interval</returns>
        public double QuadraticMean(QdbTimeInterval interval)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.QuadraticMean, interval).Value();
        }

        /// <summary>
        /// Gets the quadratic mean (ie the mean of all values) in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The quadratic mean values (<c>NaN</c> when there is no point in an interval)</returns>
        public IEnumerable<double> QuadraticMean(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.QuadraticMean, intervals).Value();
        }

        #endregion

        #region Count

        /// <summary>
        /// Gets the number of points in the table
        /// </summary>
        /// <returns>The number of points in the table</returns>
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
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.Count, interval).Count();
        }

        /// <summary>
        /// Gets the number of points in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The number of points in each interval</returns>
        public IEnumerable<long> Count(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.Count, intervals).Count();
        }

        #endregion

        #region Sum()

        /// <summary>
        /// Gets the sum (ie the addition of all values) of the table
        /// </summary>
        /// <returns>The sum of the table</returns>
        /// <exception cref="QdbEmptyColumnException">If the column is empty</exception>
        public double Sum()
        {
            var result = _aggregator.DoubleAggregate(qdb_ts_aggregation_type.Sum);
            if (result.Count() == 0)
                throw new QdbEmptyColumnException(Series.Alias, Name);
            return result.Value();
        }

        /// <summary>
        /// Gets the sum (ie the addition of all values) of an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The sum of the interval or <c>NaN</c> if there is no point in the interval</returns>
        public double Sum(QdbTimeInterval interval)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.Sum, interval).Value();
        }

        /// <summary>
        /// Gets the sum (ie the addition of all values) in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The sum of each interval or <c>NaN</c> when there is no point in an interval</returns>
        public IEnumerable<double> Sum(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.Sum, intervals).Value();
        }

        #endregion

        #region SumOfSquares()

        /// <summary>
        /// Gets the sum of squares (ie the addition of all squares of values) of the table
        /// </summary>
        /// <returns>The sum of squares of the table</returns>
        /// <exception cref="QdbEmptyColumnException">If the column is empty</exception>
        public double SumOfSquares()
        {
            var result = _aggregator.DoubleAggregate(qdb_ts_aggregation_type.SumOfSquares);
            if (result.Count() == 0)
                throw new QdbEmptyColumnException(Series.Alias, Name);
            return result.Value();
        }

        /// <summary>
        /// Gets the sum of squares (ie the addition of all squares of values) of an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The sum of squares of the interval or <c>NaN</c> if there is no point in the interval</returns>
        public double SumOfSquares(QdbTimeInterval interval)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.SumOfSquares, interval).Value();
        }

        /// <summary>
        /// Gets the sum of squares (ie the addition of all squares of values) in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The sum of squares of each interval or <c>NaN</c> when there is no point in an interval</returns>
        public IEnumerable<double> SumOfSquares(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.SumOfSquares, intervals).Value();
        }

        #endregion

        #region Spread()

        /// <summary>
        /// Gets the spread (ie the difference between the maximum value and the minimum value) of the table
        /// </summary>
        /// <returns>The spread of the table</returns>
        /// <exception cref="QdbEmptyColumnException">If the column is empty</exception>
        public double Spread()
        {
            var result = _aggregator.DoubleAggregate(qdb_ts_aggregation_type.Spread);
            if (result.Count() == 0)
                throw new QdbEmptyColumnException(Series.Alias, Name);
            return result.Value();
        }

        /// <summary>
        /// Gets the spread (ie the difference between the maximum value and the minimum value) of an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The spread of the interval or <c>NaN</c> if there is no point in the interval</returns>
        public double Spread(QdbTimeInterval interval)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.Spread, interval).Value();
        }

        /// <summary>
        /// Gets the spread (ie the difference between the maximum value and the minimum value) in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The spread of each interval or <c>NaN</c> when there is no point in an interval</returns>
        public IEnumerable<double> Spread(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.Spread, intervals).Value();
        }

        #endregion

        #region SampleVariance()

        /// <summary>
        /// Gets the sample variance of the table
        /// </summary>
        /// <returns>The sample variance of the table</returns>
        /// <exception cref="QdbEmptyColumnException">If the column is empty</exception>
        public double SampleVariance()
        {
            var result = _aggregator.DoubleAggregate(qdb_ts_aggregation_type.SampleVariance);
            if (result.Count() == 0)
                throw new QdbEmptyColumnException(Series.Alias, Name);
            return result.Value();
        }

        /// <summary>
        /// Gets the sample variance of an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The sample variance of the interval or <c>NaN</c> if there is no point in the interval</returns>
        public double SampleVariance(QdbTimeInterval interval)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.SampleVariance, interval).Value();
        }

        /// <summary>
        /// Gets the sample variance in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The sample variance of each interval or <c>NaN</c> when there is no point in an interval</returns>
        public IEnumerable<double> SampleVariance(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.SampleVariance, intervals).Value();
        }

        #endregion

        #region SampleStdDev()

        /// <summary>
        /// Gets the sample standard deviation of the table
        /// </summary>
        /// <returns>The sample standard deviation of the table</returns>
        /// <exception cref="QdbEmptyColumnException">If the column is empty</exception>
        public double SampleStdDev()
        {
            var result = _aggregator.DoubleAggregate(qdb_ts_aggregation_type.SampleStdDev);
            if (result.Count() == 0)
                throw new QdbEmptyColumnException(Series.Alias, Name);
            return result.Value();
        }

        /// <summary>
        /// Gets the sample standard deviation of an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The sample standard deviation of the interval or <c>NaN</c> if there is no point in the interval</returns>
        public double SampleStdDev(QdbTimeInterval interval)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.SampleStdDev, interval).Value();
        }

        /// <summary>
        /// Gets the sample standard deviation in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The sample standard deviation of each interval or <c>NaN</c> when there is no point in an interval</returns>
        public IEnumerable<double> SampleStdDev(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.SampleStdDev, intervals).Value();
        }

        #endregion

        #region PopulationVariance()

        /// <summary>
        /// Gets the population variance of the table
        /// </summary>
        /// <returns>The population variance of the table</returns>
        /// <exception cref="QdbEmptyColumnException">If the column is empty</exception>
        public double PopulationVariance()
        {
            var result = _aggregator.DoubleAggregate(qdb_ts_aggregation_type.PopulationVariance);
            if (result.Count() == 0)
                throw new QdbEmptyColumnException(Series.Alias, Name);
            return result.Value();
        }

        /// <summary>
        /// Gets the population variance of an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The population variance of the interval or <c>NaN</c> if there is no point in the interval</returns>
        public double PopulationVariance(QdbTimeInterval interval)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.PopulationVariance, interval).Value();
        }

        /// <summary>
        /// Gets the population variance in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The population variance of each interval or <c>NaN</c> when there is no point in an interval</returns>
        public IEnumerable<double> PopulationVariance(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.PopulationVariance, intervals).Value();
        }

        #endregion

        #region PopulationStdDev()

        /// <summary>
        /// Gets the population standard deviation of the table
        /// </summary>
        /// <returns>The population standard deviation of the table</returns>
        /// <exception cref="QdbEmptyColumnException">If the column is empty</exception>
        public double PopulationStdDev()
        {
            var result = _aggregator.DoubleAggregate(qdb_ts_aggregation_type.PopulationStdDev);
            if (result.Count() == 0)
                throw new QdbEmptyColumnException(Series.Alias, Name);
            return result.Value();
        }

        /// <summary>
        /// Gets the population standard deviation of an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The population standard deviation of the interval or <c>NaN</c> if there is no point in the interval</returns>
        public double PopulationStdDev(QdbTimeInterval interval)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.PopulationStdDev, interval).Value();
        }

        /// <summary>
        /// Gets the population standard deviation in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The population standard deviation of each interval or <c>NaN</c> when there is no point in an interval</returns>
        public IEnumerable<double> PopulationStdDev(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.PopulationStdDev, intervals).Value();
        }

        #endregion

        #region AbsMin()

        /// <summary>
        /// Gets the point with the smallest absolute value of the table
        /// </summary>
        /// <returns>The point with the smallest absolute value of the table</returns>
        /// <exception cref="QdbEmptyColumnException">If the column is empty</exception>
        public Point AbsMin()
        {
            var point = _aggregator.DoubleAggregate(qdb_ts_aggregation_type.AbsMin).AsPoint();
            if (point == null)
                throw new QdbEmptyColumnException(Series.Alias, Name);
            return point;
        }

        /// <summary>
        /// Gets the point with the smallest absolute value in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The point with the smallest absolute value in the interval or <c>null</c> if there is no point in the interval</returns>
        public Point AbsMin(QdbTimeInterval interval)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.AbsMin, interval).AsPoint();
        }

        /// <summary>
        /// Gets the point with the smallest absolute value in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The point with the smallest absolute value of each interval (<c>null</c> when there is no point in an interval)</returns>
        public IEnumerable<Point> AbsMin(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.AbsMin, intervals).AsPoint();
        }

        #endregion

        #region AbsMax()

        /// <summary>
        /// Gets the point with the largest absolute value of the table
        /// </summary>
        /// <returns>The point with the largest absolute value of the table</returns>
        /// <exception cref="QdbEmptyColumnException">If the column is empty</exception>
        public Point AbsMax()
        {
            var point = _aggregator.DoubleAggregate(qdb_ts_aggregation_type.AbsMax).AsPoint();
            if (point == null)
                throw new QdbEmptyColumnException(Series.Alias, Name);
            return point;
        }

        /// <summary>
        /// Gets the point with the largest absolute value in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The point with the largest absolute value of the interval or <c>null</c> if there is no point in the interval</returns>
        public Point AbsMax(QdbTimeInterval interval)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.AbsMax, interval).AsPoint();
        }

        /// <summary>
        /// Gets the point with the largest absolute value in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The point with the largest absolute value in each interval (<c>null</c> when there is no point in an interval)</returns>
        public IEnumerable<Point> AbsMax(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.AbsMax, intervals).AsPoint();
        }

        #endregion

        #region Product()

        /// <summary>
        /// Gets the product of the table
        /// </summary>
        /// <returns>The product of the table</returns>
        /// <exception cref="QdbEmptyColumnException">If the column is empty</exception>
        public double Product()
        {
            var result = _aggregator.DoubleAggregate(qdb_ts_aggregation_type.Product);
            if (result.Count() == 0)
                throw new QdbEmptyColumnException(Series.Alias, Name);
            return result.Value();
        }

        /// <summary>
        /// Gets the product of an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The product of the interval or <c>NaN</c> if there is no point in the interval</returns>
        public double Product(QdbTimeInterval interval)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.Product, interval).Value();
        }

        /// <summary>
        /// Gets the product in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The product of each interval or <c>NaN</c> when there is no point in an interval</returns>
        public IEnumerable<double> Product(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.Product, intervals).Value();
        }

        #endregion

        #region Skewness()

        /// <summary>
        /// Gets the skewness of the table
        /// </summary>
        /// <returns>The skewness of the table</returns>
        /// <exception cref="QdbEmptyColumnException">If the column is empty</exception>
        public double Skewness()
        {
            var result = _aggregator.DoubleAggregate(qdb_ts_aggregation_type.Skewness);
            if (result.Count() == 0)
                throw new QdbEmptyColumnException(Series.Alias, Name);
            return result.Value();
        }

        /// <summary>
        /// Gets the skewness of an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The skewness of the interval or <c>NaN</c> if there is no point in the interval</returns>
        public double Skewness(QdbTimeInterval interval)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.Skewness, interval).Value();
        }

        /// <summary>
        /// Gets the skewness in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The skewness of each interval or <c>NaN</c> when there is no point in an interval</returns>
        public IEnumerable<double> Skewness(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.Skewness, intervals).Value();
        }

        #endregion

        #region Kurtosis()

        /// <summary>
        /// Gets the kurtosis of the table
        /// </summary>
        /// <returns>The kurtosis of the table</returns>
        /// <exception cref="QdbEmptyColumnException">If the column is empty</exception>
        public double Kurtosis()
        {
            var result = _aggregator.DoubleAggregate(qdb_ts_aggregation_type.Kurtosis);
            if (result.Count() == 0)
                throw new QdbEmptyColumnException(Series.Alias, Name);
            return result.Value();
        }

        /// <summary>
        /// Gets the kurtosis of an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The kurtosis of the interval or <c>NaN</c> if there is no point in the interval</returns>
        public double Kurtosis(QdbTimeInterval interval)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.Kurtosis, interval).Value();
        }

        /// <summary>
        /// Gets the kurtosis in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The kurtosis of each interval or <c>NaN</c> when there is no point in an interval</returns>
        public IEnumerable<double> Kurtosis(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.Kurtosis, intervals).Value();
        }

        #endregion

        #region DistinctCount

        /// <summary>
        /// Gets the number of points with distinct values in the table
        /// </summary>
        /// <returns>The number of points with distinct values in the table</returns>
        public long DistinctCount()
        {
            return DistinctCount(QdbTimeInterval.Everything);
        }

        /// <summary>
        /// Gets the number of points with distinct values in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The number of points with distinct values in the interval</returns>
        public long DistinctCount(QdbTimeInterval interval)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.DistinctCount, interval).Count();
        }

        /// <summary>
        /// Gets the number of points with distinct values in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The number of points with distinct values in each interval</returns>
        public IEnumerable<long> DistinctCount(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.DoubleAggregate(qdb_ts_aggregation_type.DistinctCount, intervals).Count();
        }

        #endregion
    }
}