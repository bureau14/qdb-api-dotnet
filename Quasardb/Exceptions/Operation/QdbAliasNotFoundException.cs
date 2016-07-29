namespace Quasardb.Exceptions
{
    /// <summary>
    /// Exception thrown when an entry cannot be found in the database.
    /// </summary>
    public sealed class QdbAliasNotFoundException : QdbOperationException
    {
        internal QdbAliasNotFoundException() : base("An entry matching the provided alias cannot be found.")
        {
        }
    }
}