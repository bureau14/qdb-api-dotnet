using System;
using System.Collections.Generic;
using System.Text;
using Quasardb.Native;

namespace Quasardb.TimeSeries
{
    /// <summary>
    /// Describes a time-series column, required for batch insertion
    /// </summary>
    public class QdbBatchColumnDefinition
    {
        /// <summary>
        /// The name of the table
        /// </summary>
        public string Alias { get; }

        /// <summary>
        /// The name of the column
        /// </summary>
        public string Column { get; }

        /// <summary>
        /// Creates a batch column description with the specified name.
        /// </summary>
        /// <param name="alias">The name of the table</param>
        /// <param name="column">The name of the column</param>
        public QdbBatchColumnDefinition(string alias, string column)
        {
            Alias = alias;
            Column = column;
        }
    }
}
