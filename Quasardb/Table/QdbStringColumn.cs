using System;
using System.Collections.Generic;
using Quasardb.Exceptions;
using Quasardb.Native;

using Point = Quasardb.TimeSeries.QdbStringPoint;

namespace Quasardb.TimeSeries
{
    /// <summary>
    /// A column of a table in a quasardb database.
    /// </summary>
    public sealed class QdbStringColumn : QdbColumn
    {
        internal QdbStringColumn(QdbTable series, string name) : base(series, name)
        {
        }

        #region Insert()

        /// <summary>
        /// Inserts one or more points in the table
        /// </summary>
        /// <param name="points">The points to insert</param>
        public void Insert(QdbStringPointCollection points)
        {
            var error = qdb_api.qdb_ts_string_insert(Handle, Series.Alias, Name, points.Points.Buffer, points.Points.Count);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: Series.Alias, column: Name);
        }

        /// <summary>
        /// Inserts one or more points in the table
        /// </summary>
        /// <param name="points">The points to insert</param>
        public void Insert(IEnumerable<Point> points)
        {
            using (var pointCollection = new QdbStringPointCollection(points))
            {
                Insert(pointCollection);
            }
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
        public void Insert(DateTime time, string value)
        {
            Insert(new Point(time, value));
        }

        #endregion

        #region InsertTruncate()

        /// <summary>
        /// Inserts one or more points in the table and erases given
        /// ranges in the same transaction
        /// </summary>
        /// <param name="intervals">The time intervals to erase</param>
        /// <param name="points">The points to insert</param>
        public void InsertTruncate(IEnumerable<QdbTimeInterval> intervals, QdbStringPointCollection points)
        {
            var ranges = new InteropableList<qdb_ts_range>(Helpers.GetCountOrDefault(intervals));
            foreach (var interval in intervals)
                ranges.Add(interval.ToNative());
            var error = qdb_api.qdb_ts_string_insert_truncate(
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
        public void InsertTruncate(QdbTimeInterval interval, QdbStringPointCollection points)
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
            using (var pointCollection = new QdbStringPointCollection(points))
            {
                InsertTruncate(intervals, pointCollection);
            }
        }

        /// <summary>
        /// Inserts one or more points in the table and erases the given
        /// range in the same transaction
        /// </summary>
        /// <param name="interval">The time interval to erase</param>
        /// <param name="points">The points to insert</param>
        public void InsertTruncate(QdbTimeInterval interval, IEnumerable<Point> points)
        {
            using (var pointCollection = new QdbStringPointCollection(points))
            {
                InsertTruncate(new[] { interval }, pointCollection);
            }
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
        public void InsertTruncate(IEnumerable<QdbTimeInterval> intervals, DateTime time, string value)
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
        public void InsertTruncate(QdbTimeInterval interval, DateTime time, string value)
        {
            InsertTruncate(new[] { interval }, new Point(time, value));
        }

        #endregion

        #region Points()

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
            using (var points = new qdb_buffer<qdb_ts_string_point>(Handle))
            {
                var error = qdb_api.qdb_ts_string_get_ranges(Handle, Series.Alias, Name, ranges.Buffer, ranges.Count, out points.Pointer, out points.Size);
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
            var result = _aggregator.StringAggregate(qdb_ts_aggregation_type.First);
            if (result.Count() == 0)
                throw new QdbEmptyColumnException(Series.Alias, Name);
            return result.AsPoint();
        }

        /// <summary>
        /// Gets the first point (ie the one with the oldest timestamp) in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The first point in the interval or <c>null</c> if there is no point in the interval</returns>
        public Point First(QdbTimeInterval interval)
        {
            return _aggregator.StringAggregate(qdb_ts_aggregation_type.First, interval).AsPoint();
        }

        /// <summary>
        /// Gets the first points (ie the one with the oldest timestamp) of each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The first point in each interval (<c>null</c> when there is no point in an interval)</returns>
        public IEnumerable<Point> First(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.StringAggregate(qdb_ts_aggregation_type.First, intervals).AsPoint();
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
            var result = _aggregator.StringAggregate(qdb_ts_aggregation_type.Last);
            if (result.Count() == 0)
                throw new QdbEmptyColumnException(Series.Alias, Name);
            return result.AsPoint();
        }

        /// <summary>
        /// Gets the last point (ie the one with the newest timestamp) in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The last point in the interval or <c>null</c> if there is no point in the interval</returns>
        public Point Last(QdbTimeInterval interval)
        {
            return _aggregator.StringAggregate(qdb_ts_aggregation_type.Last, interval).AsPoint();
        }

        /// <summary>
        /// Gets the last points (ie the one with the newest timestamp) in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The last point in each interval (<c>null</c> when there is no point in an interval)</returns>
        public IEnumerable<Point> Last(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.StringAggregate(qdb_ts_aggregation_type.Last, intervals).AsPoint();
        }

        #endregion

        #region Count()

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
            return _aggregator.StringAggregate(qdb_ts_aggregation_type.Count, interval).Count();
        }

        /// <summary>
        /// Gets the number of points in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The number of points in each interval</returns>
        public IEnumerable<long> Count(IEnumerable<QdbTimeInterval> intervals)
        {
            return _aggregator.StringAggregate(qdb_ts_aggregation_type.Count, intervals).Count();
        }

        #endregion
    }
}