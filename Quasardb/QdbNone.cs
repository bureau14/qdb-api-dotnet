// ReSharper disable InconsistentNaming

namespace Quasardb
{
    /// <summary>
    /// A unit type that represents no value.
    /// </summary>
    public class QdbNone
    {
        /// <summary>
        /// The only instance of this type.
        /// </summary>
        public static readonly QdbNone Instance = new QdbNone();

        private QdbNone() { }
    }
}
