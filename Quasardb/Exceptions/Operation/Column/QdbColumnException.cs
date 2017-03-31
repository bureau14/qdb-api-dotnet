namespace Quasardb.Exceptions
{
    /// <summary>
    /// Exception throw when an error concerns a time-series column.
    /// </summary>
    public class QdbColumnException : QdbOperationException
    {
        internal QdbColumnException(string message, string alias, string column) : base(message, alias)
        {
            Column = column;
            base.Data["Column"] = column;
        }

        /// <summary>
        /// The name of the column.
        /// </summary>
        public string Column { get; }
    }
}