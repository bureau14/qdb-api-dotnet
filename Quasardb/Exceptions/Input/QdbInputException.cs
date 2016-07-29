namespace Quasardb.Exceptions
{
    /// <summary>
    /// Exception thrown when the input of a command caused an error.
    /// </summary>
    public class QdbInputException : QdbException
    {
        /// <inheritdoc />
        protected internal QdbInputException(string message) : base(message)
        {
        }
    }
}