namespace Quasardb.Exceptions
{
    /// <summary>
    /// Exception thrown when the given index in out of bounds.
    /// </summary>
    public sealed class QdbOutOfBoundsException : QdbInputException
    {
        internal QdbOutOfBoundsException() : base("The given index was out of bounds.")
        {
        }
    }
}
