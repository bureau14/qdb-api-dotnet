namespace Quasardb.Exceptions
{
    /// <summary>
    /// Exception thrown when the specified entry has a type incompatible for this operation.
    /// </summary>
    public sealed class QdbIncompatibleTypeException : QdbOperationException
    {
        internal QdbIncompatibleTypeException(string alias) : base($"The alias \"{alias}\" has a type incompatible for this operation.", alias)
        {
        }
    }
}
