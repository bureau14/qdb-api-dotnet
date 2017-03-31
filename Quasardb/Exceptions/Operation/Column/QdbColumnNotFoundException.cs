namespace Quasardb.Exceptions
{
    /// <summary>
    /// Exception thrown when an entry cannot be found in the database.
    /// </summary>
    public sealed class QdbColumnNotFoundException : QdbColumnException
    {
        internal QdbColumnNotFoundException(string alias, string column) : base($"The time-series \"{alias}\" has no column \"{column}\".", alias, column)
        {
        }
    }
}