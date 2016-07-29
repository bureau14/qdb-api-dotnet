namespace Quasardb.Exceptions
{
    /// <summary>
    /// Exception thrown when an operation caused an error.
    /// </summary>
    public class QdbOperationException : QdbException
    {
        /// <inheritdoc />
        protected internal QdbOperationException(string message) : base(message)
        {
        }
    }
}