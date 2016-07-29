namespace Quasardb.Exceptions
{
    /// <summary>
    /// Exception thrown when the local (ie client) operating system caused an error.
    /// </summary>
    public class QdbLocalSystemException : QdbSystemException
    {
        /// <inheritdoc />
        protected internal QdbLocalSystemException(string message) : base(message)
        {
        }
    }
}