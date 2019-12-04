namespace Quasardb.Exceptions
{
    /// <summary>
    /// Exception thrown when the input of a query caused an error.
    /// </summary>
    public class QdbQueryException : QdbException
    {
        /// <inheritdoc />
        protected internal QdbQueryException(string message) : base(message)
        {
        }
    }
}
