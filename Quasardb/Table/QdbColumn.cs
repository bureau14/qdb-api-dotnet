using System;
using System.Collections.Generic;
using Quasardb.Exceptions;
using Quasardb.Native;

namespace Quasardb.TimeSeries
{
    /// <summary>
    /// A column of a table
    /// </summary>
    public abstract class QdbColumn
    {
        internal readonly QdbColumnAggregator _aggregator;

        internal QdbColumn(QdbTable series, string name)
        {
            Series = series;
            Name = name;
            _aggregator = new QdbColumnAggregator(this);
        }

        internal QdbColumn(QdbTable series, string name, string symtable)
        {
            Series = series;
            Name = name;
            Symtable = symtable;
            _aggregator = new QdbColumnAggregator(this);
        }

        /// <summary>
        /// The parent of the column
        /// </summary>
        public QdbTable Series { get; }

        /// <summary>
        /// The name of the column
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The symtable name of the column
        /// </summary>
        public string Symtable { get; }

        internal qdb_handle Handle => Series.Handle;

        #region Timestamps

        /// <summary>
        /// Gets all the timestamps in the table
        /// </summary>
        /// <returns>All the timestamps in the table</returns>
        public IEnumerable<DateTime> Timestamps()
        {
            return Timestamps(QdbTimeInterval.Everything);
        }

        /// <summary>
        /// Gets all the timestamps in an interval
        /// </summary>
        /// <param name="interval">The time interval to scan</param>
        /// <returns>All the timestamps in the interval</returns>
        public IEnumerable<DateTime> Timestamps(QdbTimeInterval interval)
        {
            return Timestamps(new[] { interval });
        }

        /// <summary>
        /// Gets all the timestamps in each interval
        /// </summary>
        /// <param name="intervals">The time intervals to scan</param>
        /// <returns>All the timestamps in each interval</returns>
        public IEnumerable<DateTime> Timestamps(IEnumerable<QdbTimeInterval> intervals)
        {
            var ranges = new InteropableList<qdb_ts_range>(Helpers.GetCountOrDefault(intervals));
            foreach (var interval in intervals)
                ranges.Add(interval.ToNative());
            using (var timestamps = new qdb_buffer<qdb_timespec>(Handle))
            {
                var error = qdb_api.qdb_ts_get_timestamps(Handle, Series.Alias, Name, ranges.Buffer, ranges.Count,
                    out timestamps.Pointer, out timestamps.Size);
                QdbExceptionThrower.ThrowIfNeeded(error, alias: Series.Alias, column: Name);

                foreach (var pt in timestamps)
                    yield return TimeConverter.ToDateTime(pt);
            }
        }

        #endregion

        #region Erase

        /// <summary>
        /// Erases all points in the specified ranges (left inclusive)
        /// </summary>
        /// <param name="intervals">The time intervals to erase</param>
        /// <returns>The number of erased points</returns>
        public long Erase(IEnumerable<QdbTimeInterval> intervals)
        {
            var ranges = new InteropableList<qdb_ts_range>(Helpers.GetCountOrDefault(intervals));
            foreach (var interval in intervals)
                ranges.Add(interval.ToNative());
            var error = qdb_api.qdb_ts_erase_ranges(
                Handle, Series.Alias, Name,
                ranges.Buffer, ranges.Count,
                out ulong erasedCount);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: Series.Alias, column: Name);
            return (long)erasedCount;
        }

        /// <summary>
        /// Erases all points in the specified range (left inclusive)
        /// </summary>
        /// <param name="interval">The time interval to erase</param>
        /// <returns>The number of erased points</returns>
        public long Erase(QdbTimeInterval interval)
        {
            return Erase(new[] { interval });
        }

        #endregion
    }

    class QdbUnknownColumn : QdbColumn
    {
        public readonly int Type;

        public QdbUnknownColumn(QdbTable series, string name, qdb_ts_column_type type) : base(series, name)
        {
            Type = (int)type;
        }
    }
}