namespace Quasardb.Exceptions
{
    /// <summary>
    /// Exception thrown when an error is detected in the quasardb protocol.
    /// </summary>
    public class QdbProtocolException : QdbException
    {
        /// <inheritdoc />
        protected internal QdbProtocolException(string message) : base(message)
        {
        }
    }
}