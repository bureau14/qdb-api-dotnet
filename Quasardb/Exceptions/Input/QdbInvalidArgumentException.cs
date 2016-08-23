namespace Quasardb.Exceptions
{
    /// <summary>
    /// Exception thrown when an argument passed to a method is incorrect.
    /// </summary>
    public sealed class QdbInvalidArgumentException : QdbInputException
    {
        internal QdbInvalidArgumentException() : base("The argument is invalid.")
        {
        }
    }
}
