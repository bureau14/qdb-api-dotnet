using System;
using System.Collections;
using System.Collections.Generic;
using Quasardb.ManagedApi;
using Quasardb.NativeApi;

namespace Quasardb.TimeSeries
{
    /// <summary>
    /// A collection of <see cref="QdbDoublePoint"/>.
    /// </summary>
    public sealed class QdbDoublePointCollection : IEnumerable<QdbDoublePoint>
    {
        internal readonly InteropableList<qdb_ts_double_point> Points;

        /// <summary>
        /// Creates an empty collection
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the collection</param>
        public QdbDoublePointCollection(int initialCapacity = 1024)
        {
            Points = new InteropableList<qdb_ts_double_point>(initialCapacity);
        }

        /// <summary>
        /// Creates the collection from an existing one
        /// </summary>
        /// <param name="source">The collection of point to duplicate</param>
        public QdbDoublePointCollection(IEnumerable<QdbDoublePoint> source)
        {
            Points = new InteropableList<qdb_ts_double_point>(Helpers.GetCountOrDefault(source, 1024));
            foreach (var point in source)
                Add(point);
        }

        /// <summary>
        /// Adds a point to the collection
        /// </summary>
        /// <param name="timespamp"></param>
        /// <param name="value"></param>
        public void Add(DateTime timespamp, double value)
        {
            Points.Add(new QdbDoublePoint(timespamp, value).ToNative());
        }

        /// <summary>
        /// Adds a point to the collection
        /// </summary>
        /// <param name="point">The point to add</param>
        public void Add(QdbDoublePoint point)
        {
            Points.Add(point.ToNative());
        }

        /// <summary>
        /// Gets the number of points in the collection
        /// </summary>
        public int Count => (int) Points.Count;

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
        public QdbDoublePoint this[int index] => QdbDoublePoint.FromNative(Points[index]);

        /// <inheritdoc />
        public IEnumerator<QdbDoublePoint> GetEnumerator()
        {
            foreach (var point in Points)
                yield return QdbDoublePoint.FromNative(point);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}