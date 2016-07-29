namespace Quasardb.Exceptions
{
    /// <summary>
    /// Exception thrown when reading a value from an empty container.
    /// </summary>
    /// TODO: remove this exception, we should return null instead of throwing, like the APIs for other languages
    public class QdbEmptyContainerException : QdbException
    {
        internal QdbEmptyContainerException() : base("The entry contains an empty container.")
        {
        }
    }
}
