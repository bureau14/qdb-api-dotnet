using System;
using Quasardb.Native;

namespace Quasardb.TimeSeries
{
    /// <summary>
    /// A Time/Value pair.
    /// </summary>
    public class QdbPoint<T> : IEquatable<QdbPoint<T>>
    {
        /// <summary>
        /// The timestamp of the point
        /// </summary>
        public readonly DateTime Time;

        /// <summary>
        /// The value of the point
        /// </summary>
        public readonly T Value;

        /// <summary>
        /// Creates a point with the specified time and value
        /// </summary>
        /// <param name="time">The timestamp of the point</param>
        /// <param name="value">The value of the point</param>
        public QdbPoint(DateTime time, T value)
        {
            Time = time;
            Value = value;
        }

        /// <inheritdoc />
        public bool Equals(QdbPoint<T> other)
        {
            return other != null && Equals(other.Time, Time) && Equals(other.Value, Value);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as QdbPoint<T>);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Time.GetHashCode() ^ Value.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{{{Time}, {Value}}}";
        }
    }
}