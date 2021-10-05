using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Quasardb.Native;

using Point = Quasardb.TimeSeries.QdbBlobPoint;

namespace Quasardb.TimeSeries
{
    /// <summary>
    /// A collection of point
    /// </summary>
    public sealed class QdbBlobPointCollection : IEnumerable<Point>, IDisposable
    {
        private bool disposed = false;

        internal readonly InteropableList<qdb_ts_blob_point> Points;
        readonly List<GCHandle> _pins;

        /// <summary>
        /// Creates an empty collection
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the collection</param>
        public QdbBlobPointCollection(int initialCapacity = 1024)
        {
            Points = new InteropableList<qdb_ts_blob_point>(initialCapacity);
            _pins = new List<GCHandle>(initialCapacity);
        }

        /// <summary>
        /// Creates the collection from an existing one
        /// </summary>
        /// <param name="source">The collection of point to duplicate</param>
        public QdbBlobPointCollection(IEnumerable<Point> source)
        {
            var initialCapacity = Helpers.GetCountOrDefault(source, 1024);
            _pins = new List<GCHandle>(initialCapacity);
            Points = new InteropableList<qdb_ts_blob_point>(initialCapacity);
            foreach (var point in source)
                Add(point);
        }

        /// <summary>
        /// Adds a point to the collection
        /// </summary>
        /// <param name="timestamp">The timestamp of the new point</param>
        /// <param name="content">The value of the new point</param>
        public void Add(DateTime timestamp, byte[] content)
        {
            Add(new Point(timestamp, content));
        }

        /// <summary>
        /// Adds a point to the collection
        /// </summary>
        /// <param name="point">The point to add</param>
        public void Add(Point point)
        {
            GCHandle? pin;
            Points.Add(point.ToNative(out pin));
            if (pin.HasValue)
                _pins.Add(pin.Value);
        }

        /// <summary>
        /// Gets the number of points in the collection
        /// </summary>
        public int Count => (int)Points.Count;

        /// <summary>
        /// Gets or sets the total number of elements the collection can hold without resizing.
        /// </summary>
        public int Capacity => (int)Points.Capacity;

        /// <summary>
        /// Empties the collection
        /// </summary>
        public void Clear() => Points.Clear();

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

        void Free()
        {
            foreach (var pin in _pins)
                pin.Free();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Check to see if Dispose has already been called.
            if(!this.disposed)
            {
                Free();
                GC.SuppressFinalize(this);
                this.disposed = true;
            }
        }

        /// <inheritdoc />
        ~QdbBlobPointCollection()
        {
            Free();
        }
    }
}