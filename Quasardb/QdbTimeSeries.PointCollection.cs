using System;
using System.Collections;
using System.Collections.Generic;
using Quasardb.ManagedApi;
using Quasardb.NativeApi;

namespace Quasardb
{
    public partial class QdbTimeSeries
    {
        /// <summary>
        /// A collection of <see cref="Point"/>.
        /// </summary>
        public sealed class PointCollection : IEnumerable<Point>
        {
            internal readonly InteropableList<qdb_ts_double_point> Points;

            /// <summary>
            /// Create a empty collection
            /// </summary>
            /// <param name="initialCapacity">The initial capacity of the collection</param>
            public PointCollection(int initialCapacity = 1024)
            {
                Points = new InteropableList<qdb_ts_double_point>(initialCapacity);
            }

            /// <summary>
            /// Adds a point to the collection
            /// </summary>
            /// <param name="timespamp"></param>
            /// <param name="value"></param>
            public void Add(DateTime timespamp, double value)
            {
                Points.Add(new Point(timespamp, value).ToNative());
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
            public Point this[int index] => Point.FromNative(Points[index]);

            /// <inheritdoc />
            public IEnumerator<Point> GetEnumerator()
            {
                foreach (var point in Points)
                    yield return Point.FromNative(point);
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}