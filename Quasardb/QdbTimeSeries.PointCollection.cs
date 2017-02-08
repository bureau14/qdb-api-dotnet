using System;
using System.Collections;
using System.Collections.Generic;
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
            qdb_ts_double_point[] _points;
            int _length;

            /// <summary>
            /// Create a empty collection
            /// </summary>
            /// <param name="initialCapacity">The initial capacity of the collection</param>
            public PointCollection(int initialCapacity = 1024)
            {
                _points = new qdb_ts_double_point[initialCapacity];
                _length = 0;
            }

            /// <summary>
            /// Adds a point to the collection
            /// </summary>
            /// <param name="timespamp"></param>
            /// <param name="value"></param>
            public void Add(DateTime timespamp, double value)
            {
                if (IsFull) IncreaseCapacity();

                _points[_length].timestamp = TimeConverter.ToTimespec(timespamp);
                _points[_length].value = value;
                _length++;
            }

            /// <summary>
            /// Gets the number of points in the collection
            /// </summary>
            public int Count => _length;

            /// <summary>
            /// Empties the collection
            /// </summary>
            public void Clear()
            {
                _length = 0;
            }

            /// <summary>
            /// Gets the point at the specified index.
            /// </summary>
            /// <param name="index">The zero-based position in the collection</param>
            /// <exception cref="ArgumentOutOfRangeException">If index is negative or above Count</exception>
            public Point this[int index]
            {
                get
                {
                    if (index<0 || index>=_length) throw new ArgumentOutOfRangeException(nameof(index));
                    return PointConverter.ToManaged(_points[index]);
                }
            }

            /// <inheritdoc />
            public IEnumerator<Point> GetEnumerator()
            {
                foreach (var point in _points)
                {
                    yield return PointConverter.ToManaged(point);
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            internal qdb_ts_double_point[] GetPoints()
            {
                Array.Resize(ref _points, _length);
                return _points;
            }

            void IncreaseCapacity()
            {
                Array.Resize(ref _points, _points.Length * 2);
            }

            bool IsFull => _points.Length == _length;
        }
    }
}