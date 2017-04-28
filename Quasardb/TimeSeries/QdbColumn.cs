using System.Collections.Generic;
using Quasardb.Exceptions;
using Quasardb.Native;

namespace Quasardb.TimeSeries
{
    /// <summary>
    /// A column of a time series
    /// </summary>
    public abstract class QdbColumn
    {
        internal readonly QdbColumnAggregator _aggregator;

        internal QdbColumn(QdbTimeSeries series, string name)
        {
            Series = series;
            Name = name;
            _aggregator = new QdbColumnAggregator(this);
        }

        /// <summary>
        /// The parent of the column
        /// </summary>
        public QdbTimeSeries Series { get; }

        /// <summary>
        /// The name of the column
        /// </summary>
        public string Name { get; }

        internal qdb_handle Handle => Series.Handle;
    }

    class QdbUnknownColumn : QdbColumn
    {
        public readonly int Type;

        public QdbUnknownColumn(QdbTimeSeries series, string name, qdb_ts_column_type type) : base(series, name)
        {
            Type = (int) type;
        }
    }
}