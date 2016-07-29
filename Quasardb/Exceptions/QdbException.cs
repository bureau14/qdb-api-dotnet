using System;

namespace Quasardb.Exceptions
{
    /// <summary>
    /// Base class of all quasardb exceptions
    /// </summary>
    public class QdbException : Exception
    {
        /// <summary>
        /// Creates an exception with the specified message.
        /// </summary>
        protected internal QdbException(string message) : base(message)
        {
        }
    }
}
