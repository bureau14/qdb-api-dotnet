using System;
using Quasardb.Native;

namespace Quasardb.TimeSeries
{
    /// <summary>
    /// A Time/Value pair.
    /// </summary>
    public sealed class QdbDoublePoint : IEquatable<QdbDoublePoint>
    {
        /// <summary>
        /// The timestamp of the point
        /// </summary>
        public readonly DateTime Time;

        /// <summary>
        /// The value of the point
        /// </summary>
        public readonly double Value;

        /// <summary>
        /// Creates a point with the specified time and value
        /// </summary>
        /// <param name="time">The timestamp of the point</param>
        /// <param name="value">The value of the point</param>
        public QdbDoublePoint(DateTime time, double value)
        {
            Time = time;
            Value = value;
        }

        /// <inheritdoc />
        public bool Equals(QdbDoublePoint other)
        {
            return other != null && other.Time == Time && other.Value == Value;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as QdbDoublePoint);
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

        internal qdb_ts_double_point ToNative()
        {
            return new qdb_ts_double_point
            {
                timestamp = TimeConverter.ToTimespec(Time),
                value = Value
            };
        }

        internal static QdbDoublePoint FromNative(qdb_ts_double_point pt)
        {
            return new QdbDoublePoint(TimeConverter.ToDateTime(pt.timestamp), pt.value);
        }
    }
}