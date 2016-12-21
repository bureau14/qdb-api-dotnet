namespace Quasardb
{
    /// <summary>
    /// Internal use only. Please ignore.
    /// </summary>
    public interface IVisitable
    {
        /// <summary>
        /// Accept a visitor.
        /// </summary>
        /// <param name="visitor">A visitor.</param>
        /// <returns>An implementation defined result</returns>
        object Accept(object visitor);
    }
}