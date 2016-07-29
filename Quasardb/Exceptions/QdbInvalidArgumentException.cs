namespace Quasardb.Exceptions
{
    /// <summary>
    /// Exception thrown when an argument passed to a method is incorrect.
    /// </summary>
    public class QdbInvalidArgumentException : QdbException
    {
        internal QdbInvalidArgumentException() : base("The argument is invalid.")
        {
        }
    }
}
