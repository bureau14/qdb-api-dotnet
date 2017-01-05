namespace Quasardb.Exceptions
{
    /// <summary>
    /// Exception thrown when an operation caused an error.
    /// </summary>
    public class QdbOperationException : QdbException
    {
        /// <inheritdoc />
        protected internal QdbOperationException(string message, string alias) : base(message)
        {
            Alias = alias;
            base.Data["Alias"] = alias;
        }

        /// <summary>
        /// Gets the alias of the entry that caused the error.
        /// </summary>
        public string Alias { get; private set; }
    }
}