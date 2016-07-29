namespace Quasardb.Exceptions
{
    /// <summary>
    /// Exception thrown when the connection to the database caused an error.
    /// </summary>
    public class QdbConnectionException : QdbException
    {
        /// <inheritdoc />
        protected internal QdbConnectionException(string message) : base(message)
        {
        }
    }
}