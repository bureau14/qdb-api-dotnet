using System;
using System.Collections.Generic;
using Quasardb.Exceptions;
using Quasardb.Native;

using Point = Quasardb.TimeSeries.QdbBlobPoint;

namespace Quasardb.TimeSeries
{
    /// <summary>
    /// A column of a time series in a quasardb database.
    /// </summary>
    public sealed class QdbBlobColumn : QdbColumn
    {
        internal QdbBlobColumn(QdbTimeSeries series, string name) : base(series, name)
        {
        }

        #region Insert

        /// <summary>
        /// Inserts one or more points in the time series
        /// </summary>
        /// <param name="points">The points to insert</param>
        public void Insert(QdbBlobPointCollection points)
        {
            var error = qdb_api.qdb_ts_blob_insert(Handle, Series.Alias, Name, points.Points.Buffer, points.Points.Count);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: Series.Alias);
        }

        /// <summary>
        /// Inserts one or more points in the time series
        /// </summary>
        /// <param name="points">The points to insert</param>
        public void Insert(IEnumerable<Point> points)
        {
            using (var pointCollection = new QdbBlobPointCollection(points))
            {
                Insert(pointCollection);
            }
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
        public void Insert(DateTime time, byte[] value)
        {
            Insert(new Point(time, value));
        }

        #endregion

        #region Points

        /// <summary>
        /// Gets all the points in the time series
        /// </summary>
        /// <returns>All the points in the time series</returns>
        public IEnumerable<Point> Points()
        {
            return Points(QdbFilteredTimeInterval.Everything);
        }

        /// <summary>
        /// Gets all the points in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>All the points in the interval</returns>
        public IEnumerable<Point> Points(QdbFilteredTimeInterval interval)
        {
            return Points(new[] { interval });
        }

        /// <summary>
        /// Gets all the points in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>All the points in each interval</returns>
        public IEnumerable<Point> Points(IEnumerable<QdbFilteredTimeInterval> intervals)
        {
            var ranges = new InteropableList<qdb_ts_filtered_range>(Helpers.GetCountOrDefault(intervals));
            foreach (var interval in intervals)
                ranges.Add(interval.ToNative());
            using (var points = new qdb_buffer<qdb_ts_blob_point>(Handle))
            {
                var error = qdb_api.qdb_ts_blob_get_ranges(Handle, Series.Alias, Name, ranges.Buffer, ranges.Count, out points.Pointer, out points.Size);
                QdbExceptionThrower.ThrowIfNeeded(error, alias: Series.Alias, column: Name);
                foreach (var pt in points)
                    yield return pt.ToManaged();
            }
        }

        #endregion

        #region Count

        /// <summary>
        /// Gets the number of points in the time series
        /// </summary>
        /// <returns>The number of points in the time series</returns>
        public long Count()
        {
            return Count(QdbFilteredTimeInterval.Everything);
        }

        /// <summary>
        /// Gets the number of points in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The number of points in the interval</returns>
        public long Count(QdbFilteredTimeInterval interval)
        {
            return _aggregator.BlobAggregate(qdb_ts_aggregation_type.Count, interval).ToLong();
        }

        /// <summary>
        /// Gets the number of points in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The number of points in each interval</returns>
        public IEnumerable<long> Count(IEnumerable<QdbFilteredTimeInterval> intervals)
        {
            return _aggregator.BlobAggregate(qdb_ts_aggregation_type.Count, intervals).ToLong();
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
            var point = _aggregator.BlobAggregate(qdb_ts_aggregation_type.First).ToBlobPoint();
            if (point == null)
                throw new QdbEmptyColumnException(Series.Alias, Name);
            return point;
        }

        /// <summary>
        /// Gets the first point (ie the one with the oldest timestamp) in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The first point in the interval or <c>null</c> if there is no point in the interval</returns>
        public Point First(QdbFilteredTimeInterval interval)
        {
            return _aggregator.BlobAggregate(qdb_ts_aggregation_type.First, interval).ToBlobPoint();
        }

        /// <summary>
        /// Gets the first points (ie the one with the oldest timestamp) of each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The first point in each interval (<c>null</c> when there is no point in an interval)</returns>
        public IEnumerable<Point> First(IEnumerable<QdbFilteredTimeInterval> intervals)
        {
            return _aggregator.BlobAggregate(qdb_ts_aggregation_type.First, intervals).ToBlobPoint();
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
            var point = _aggregator.BlobAggregate(qdb_ts_aggregation_type.Last).ToBlobPoint();
            if (point == null)
                throw new QdbEmptyColumnException(Series.Alias, Name);
            return point;
        }

        /// <summary>
        /// Gets the last point (ie the one with the newest timestamp) in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>The last point in the interval or <c>null</c> if there is no point in the interval</returns>
        public Point Last(QdbFilteredTimeInterval interval)
        {
            return _aggregator.BlobAggregate(qdb_ts_aggregation_type.Last, interval).ToBlobPoint();
        }

        /// <summary>
        /// Gets the last points (ie the one with the newest timestamp) in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>The last point in each interval (<c>null</c> when there is no point in an interval)</returns>
        public IEnumerable<Point> Last(IEnumerable<QdbFilteredTimeInterval> intervals)
        {
            return _aggregator.BlobAggregate(qdb_ts_aggregation_type.Last, intervals).ToBlobPoint();
        }

        #endregion
    }
}