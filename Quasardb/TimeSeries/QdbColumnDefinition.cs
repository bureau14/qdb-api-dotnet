using System;
using System.Collections.Generic;
using System.Text;
using Quasardb.Native;

namespace Quasardb.TimeSeries
{
    /// <summary>
    /// Describes a time-series column, required to create the column
    /// </summary>
    public abstract class QdbColumnDefinition
    {
        /// <summary>
        /// The name of the new column
        /// </summary>
        public string Name { get; }

        internal qdb_ts_column_type Type { get; }

        internal QdbColumnDefinition(string name, qdb_ts_column_type type)
        {
            Name = name;
            Type = type;
        }
    }

    /// <summary>
    /// Describes a time-series column that contains blobs
    /// </summary>
    public class QdbBlobColumnDefinition : QdbColumnDefinition
    {
        /// <summary>
        /// Creates a column description with the specified name.
        /// </summary>
        /// <param name="name">The name of the column</param>
        public QdbBlobColumnDefinition(string name) : base(name, qdb_ts_column_type.qdb_ts_column_blob)
        {
        }
    }

    /// <summary>
    /// Describes a time-series column that contains double precision floating point values.
    /// </summary>
    public class QdbDoubleColumnDefinition : QdbColumnDefinition
    {
        /// <summary>
        /// Creates a column description with the specified name.
        /// </summary>
        /// <param name="name">The name of the column</param>
        public QdbDoubleColumnDefinition(string name) : base(name, qdb_ts_column_type.qdb_ts_column_double)
        {
        }
    }
}
