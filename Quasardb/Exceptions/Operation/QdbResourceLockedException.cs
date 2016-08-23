namespace Quasardb.Exceptions
{
    /// <summary>
    /// Exception thrown when an operation cannot be performed because the entry is locked.
    /// </summary>
    public sealed class QdbResourceLockedException : QdbOperationException
    {
        internal QdbResourceLockedException() : base("The entry is currently locked by another client.")
        {
        }
    }
}
