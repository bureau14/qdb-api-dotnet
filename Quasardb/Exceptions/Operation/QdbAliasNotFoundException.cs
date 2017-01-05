namespace Quasardb.Exceptions
{
    /// <summary>
    /// Exception thrown when an entry cannot be found in the database.
    /// </summary>
    public sealed class QdbAliasNotFoundException : QdbOperationException
    {
        internal QdbAliasNotFoundException(string alias) : base($"An entry matching the alias \"{alias}\" cannot be found.", alias)
        {
        }
    }
}