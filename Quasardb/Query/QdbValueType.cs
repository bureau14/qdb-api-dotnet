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
        /// A null result />.
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
        Count = 4,

        /// <summary>
        /// A result of type String />.
        /// </summary>
        String = 5,

        /// <summary>
        /// A result of type Double Array />.
        /// </summary>
        DoubleArray = 6,

        /// <summary>
        /// A result of type Int64 Array />.
        /// </summary>
        Int64Array = 7,

        /// <summary>
        /// A result of type Blob Array />.
        /// </summary>
        BlobArray = 8,

        /// <summary>
        /// A result of type Timestamp Array />.
        /// </summary>
        TimestampArray = 9,

        /// <summary>
        /// A result of type String Array />.
        /// </summary>
        StringArray = 10,
    }
}
