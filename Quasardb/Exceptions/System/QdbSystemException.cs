namespace Quasardb.Exceptions
{
    /// <summary>
    /// Exception thrown when the operating system caused an error.
    /// </summary>
    public class QdbSystemException : QdbException
    {
        /// <inheritdoc />
        protected internal QdbSystemException(string message) : base(message)
        {
        }
    }
}