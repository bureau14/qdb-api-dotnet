using System;
using System.Collections;
using System.Collections.Generic;
using Quasardb.Native;

using Point = Quasardb.TimeSeries.QdbTimestampPoint;

namespace Quasardb.TimeSeries
{
    /// <summary>
    /// A collection of point
    /// </summary>
    public sealed class QdbTimestampPointCollection : IEnumerable<Point>
    {
        internal readonly InteropableList<qdb_ts_timestamp_point> Points;

        /// <summary>
        /// Creates an empty collection
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the collection</param>
        public QdbTimestampPointCollection(int initialCapacity = 1024)
        {
            Points = new InteropableList<qdb_ts_timestamp_point>(initialCapacity);
        }

        /// <summary>
        /// Creates the collection from an existing one
        /// </summary>
        /// <param name="source">The collection of point to duplicate</param>
        public QdbTimestampPointCollection(IEnumerable<Point> source)
        {
            Points = new InteropableList<qdb_ts_timestamp_point>(Helpers.GetCountOrDefault(source, 1024));
            foreach (var point in source)
                Add(point);
        }

        /// <summary>
        /// Adds a point to the collection
        /// </summary>
        /// <param name="timestamp">The timestamp of the new point</param>
        /// <param name="value">The value of the new point</param>
        public void Add(DateTime timestamp, DateTime? value)
        {
            Add(new Point(timestamp, value));
        }

        /// <summary>
        /// Adds a point to the collection
        /// </summary>
        /// <param name="point">The point to add</param>
        public void Add(Point point)
        {
            Points.Add(point.ToNative());
        }

        /// <summary>
        /// Gets the number of points in the collection
        /// </summary>
        public int Count => (int) Points.Count;

        /// <summary>
        /// Gets or sets the total number of elements the collection can hold without resizing.
        /// </summary>
        public int Capacity => (int) Points.Capacity;

        /// <summary>
        /// Empties the collection
        /// </summary>
        public void Clear()
        {
            Points.Clear();
        }

        /// <summary>
        /// Gets the point at the specified index.
        /// </summary>
        /// <param name="index">The zero-based position in the collection</param>
        /// <exception cref="ArgumentOutOfRangeException">If index is negative or above Count</exception>
        public Point this[int index] => Points[index].ToManaged();

        /// <inheritdoc />
        public IEnumerator<Point> GetEnumerator()
        {
            foreach (var point in Points)
                yield return point.ToManaged();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}