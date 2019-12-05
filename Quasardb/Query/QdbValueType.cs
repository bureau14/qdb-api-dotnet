using System;
using Quasardb.Exceptions;

// ReSharper disable InconsistentNaming

namespace Quasardb.Query
{
    /// <summary>
    /// Specifies the type of a value.
    /// </summary>
    public enum QdbValueType
    {
        /// <summary>
        /// A result of type <see cref="QdbNone" />.
        /// </summary>
        None = -1,

        /// <summary>
        /// A result of type double />.
        /// </summary>
        Double = 0,

        /// <summary>
        /// A result of type byte[] />.
        /// </summary>
        Blob = 1,

        /// <summary>
        /// A result of type long />.
        /// </summary>
        Int64 = 2,

        /// <summary>
        /// A result of type <see cref="DateTime" />.
        /// </summary>
        Timestamp = 3,

        /// <summary>
        /// A result of type long that represents a count.
        /// </summary>
        Count = 4
    }
}
