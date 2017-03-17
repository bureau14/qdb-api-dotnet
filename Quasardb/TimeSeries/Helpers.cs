using System.Collections.Generic;

namespace Quasardb.TimeSeries
{
    internal static class Helpers
    {
        public static int GetCountOrDefault<T>(IEnumerable<T> source, int defaultCount = 128)
        {
            return (source as ICollection<T>)?.Count ?? defaultCount;
        }
    }
}