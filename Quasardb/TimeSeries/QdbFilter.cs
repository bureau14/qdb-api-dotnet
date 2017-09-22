using System;
using Quasardb.Native;

namespace Quasardb.TimeSeries
{
    /// <summary>
    /// A filter type, for the moment only None is supported
    /// </summary>
    public enum QdbFilterType : int
    {
        /// <summary> No filter </summary>
        None = qdb_ts_filter_type.qdb_ts_filter_none,
        /// <summary> Return only unique values (remove duplicates) </summary>
        Unique = qdb_ts_filter_type.qdb_ts_filter_unique,
        /// <summary> A random selection within the range </summary>
        Sample = qdb_ts_filter_type.qdb_ts_filter_sample,
        /// <summary> Returns values inside the specified min/max range </summary>
        DoubleInsideRange = qdb_ts_filter_type.qdb_ts_filter_double_inside_range,
        /// <summary> Returns values outside the specified min/max range </summary>
        DoubleOutsideRange = qdb_ts_filter_type.qdb_ts_filter_double_outside_range
    };

    /// <summary>
    /// A filter for range based queries
    /// </summary>
    public struct QdbFilter
    {
        /// <summary>
        /// Constructs a filter
        /// </summary>
        /// <param name="type">The filtered type</param>
        public QdbFilter(QdbFilterType type = QdbFilterType.None)
        {
            Type = type;
        }

        /// <summary>
        /// The filter type
        /// </summary>
        public QdbFilterType Type;

        internal qdb_ts_filter ToNative()
        {
            return new qdb_ts_filter
            {
                type = (qdb_ts_filter_type)Type
            };
        }
    }
}