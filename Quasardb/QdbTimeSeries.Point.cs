using System;
using Quasardb.NativeApi;

namespace Quasardb
{
    public partial class QdbTimeSeries
    {
        /// <summary>
        /// A Time/Value pair.
        /// </summary>
        public sealed class Point : IEquatable<Point>
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
            public Point(DateTime time, double value)
            {
                Time = time;
                Value = value;
            }

            /// <inheritdoc />
            public bool Equals(Point other)
            {
                return other != null && other.Time == Time && other.Value == Value;
            }

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                return Equals(obj as Point);
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

            internal static Point FromNative(qdb_ts_double_point pt)
            {
                return new Point(TimeConverter.ToDateTime(pt.timestamp), pt.value);
            }
        }
    }
}