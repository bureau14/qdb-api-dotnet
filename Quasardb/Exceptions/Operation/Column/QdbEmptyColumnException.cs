namespace Quasardb.Exceptions
{
    /// <summary>
    /// Exception thrown when an entry cannot be found in the database.
    /// </summary>
    public sealed class QdbEmptyColumnException : QdbColumnException
    {
        internal QdbEmptyColumnException(string alias, string column) : base($"The column \"{column}\" is empty.", alias, column)
        {
        }
    }
}