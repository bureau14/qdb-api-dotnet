namespace Quasardb.Exceptions
{
    /// <summary>
    /// Exception thrown when the remote (ie server) operating system caused an error.
    /// </summary>
    public class QdbRemoteSystemException : QdbSystemException
    {
        /// <inheritdoc />
        protected internal QdbRemoteSystemException(string message) : base(message)
        {
        }
    }
}