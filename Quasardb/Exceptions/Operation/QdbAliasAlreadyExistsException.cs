namespace Quasardb.Exceptions
{
    /// <summary>
    /// Exception thrown when the specified entry already exists in the database.
    /// </summary>
    public sealed class QdbAliasAlreadyExistsException : QdbOperationException
    {
        internal QdbAliasAlreadyExistsException(string alias) : base($"An entry matching the alias \"{alias}\" already exists.", alias)
        {
        }
    }
}