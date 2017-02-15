using System;

namespace Quasardb
{
    /// <summary>
    /// A time interval [begin, end[
    /// </summary>
    public struct QdbTimeInterval
    {
        /// <summary>
        /// Constructs a time interval [begin, end[
        /// </summary>
        /// <param name="begin">The first date included in the interval</param>
        /// <param name="end">The first date excluded from the interval</param>
        public QdbTimeInterval(DateTime begin, DateTime end)
        {
            Begin = begin;
            End = end;
        }

        /// <summary>
        /// The first date included in the interval
        /// </summary>
        public readonly DateTime Begin;

        /// <summary>
        /// The first date excluded from the interval
        /// </summary>
        public readonly DateTime End;

        /// <summary>
        /// The duration of the interval
        /// </summary>
        public TimeSpan Duration => End - Begin;
    }
}