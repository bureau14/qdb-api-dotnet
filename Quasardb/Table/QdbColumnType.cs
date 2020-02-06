using System;
using System.Collections.Generic;
using System.Text;
using Quasardb.Native;

namespace Quasardb.TimeSeries
{
    /// <summary>
    /// Specifies the type of a column.
    /// </summary>
    public enum QdbColumnType
    {
        /// <summary>
        /// A value of type double />.
        /// </summary>
        Double = 0,

        /// <summary>
        /// A value of type byte[] />.
        /// </summary>
        Blob = 1,

        /// <summary>
        /// A value of type long />.
        /// </summary>
        Int64 = 2,

        /// <summary>
        /// A value of type <see cref="DateTime" />.
        /// </summary>
        Timestamp = 3,

        /// <summary>
        /// A value of type string.
        /// </summary>
        String = 4,
    }
}
