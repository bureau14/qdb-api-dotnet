namespace Quasardb.Exceptions
{
    /// <summary>
    /// Exception thrown when the specified entry has a type incompatible for this operation.
    /// </summary>
    public class QdbIncompatibleTypeException : QdbException
    {
        internal QdbIncompatibleTypeException() : base("The alias has a type incompatible for this operation.")
        {
        }
    }
}
