using System;

namespace Quasardb.TimeSeries
{
    /// <summary>
    /// A Time/Value pair.
    /// </summary>
    public abstract class QdbPoint<T> : IEquatable<QdbPoint<T>>
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
        protected QdbPoint(DateTime time, T value)
        {
            Time = time;
            Value = value;
        }

        /// <inheritdoc />
        public virtual bool Equals(QdbPoint<T> other)
        {
            return other != null && TimeEquals(other.Time) && ValueEquals(other.Value);
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

        bool TimeEquals(DateTime other)
        {
            return Equals(other, Time);
        }

        internal virtual bool ValueEquals(T other)
        {
            return Equals(other, Value);
        }
    }

    /// <summary>
    /// A Time/Value pair, where value is a double-precision floating point
    /// </summary>
    public class QdbDoublePoint : QdbPoint<double>
    {
        /// <summary>
        /// Creates a point.
        /// </summary>
        /// <param name="time">The point's time</param>
        /// <param name="value">The point's value</param>
        public QdbDoublePoint(DateTime time, double value) : base(time, value)
        {
        }
    }

    /// <summary>
    /// A Time/Value pair, where value is a blob
    /// </summary>
    public class QdbBlobPoint : QdbPoint<byte[]>
    {
        /// <summary>
        /// Creates a point.
        /// </summary>
        /// <param name="time">The point's time</param>
        /// <param name="value">The point's value</param>
        public QdbBlobPoint(DateTime time, byte[] value) : base(time, value)
        {
        }

        internal override bool ValueEquals(byte[] other)
        {
            // TODO: replace by SequenceEquals once we drop Framework 2.0
            if (Value.Length != other.Length) return false;
            for (var i = 0; i < Value.Length; i++)
            {
                if (Value[i] != other[i])
                    return false;
            }
            return true;
        }
    }
}