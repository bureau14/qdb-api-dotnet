using System;
using System.Collections.Generic;
using Quasardb.ManagedApi;
using Quasardb.Native;

using Point = Quasardb.TimeSeries.QdbPoint<byte[]>;

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

        /// <summary>
        /// Inserts one or more points in the time series
        /// </summary>
        /// <param name="points">The points to insert</param>
        public void Insert(QdbBlobPointCollection points)
        {
            Series.Api.TsBlobInsert(Series.Alias, Name, points.Points);
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
    }
}