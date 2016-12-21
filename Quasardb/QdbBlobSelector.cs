using System.Text;
using Quasardb.ManagedApi;

namespace Quasardb
{
    /// <summary>
    /// A way to select blobs.
    /// </summary>
    public interface IQdbBlobSelector : IVisitable
    {
    }

    /// <summary>
    /// Selects the blobs containing the given pattern.
    /// </summary>
    public sealed class QdbPatternSelector : IQdbBlobSelector
    {
        private readonly byte[] _pattern;
        private readonly long _maxCount;

        /// <summary>
        /// Creates a selector from the given pattern.
        /// </summary>
        /// <param name="pattern">The pattern to be found in the blobs.</param>
        /// <param name="maxCount">The maximum number of results</param>
        public QdbPatternSelector(string pattern, long maxCount)
        {
            _pattern = Encoding.UTF8.GetBytes(pattern);
            _maxCount = maxCount;
        }

        /// <summary>
        /// Creates a selector from the given pattern.
        /// </summary>
        /// <param name="pattern">The pattern to be found in the blobs.</param>
        /// <param name="maxCount">The maximum number of results</param>
        public QdbPatternSelector(byte[] pattern, long maxCount)
        {
            _pattern = pattern;
            _maxCount = maxCount;
        }

        object IVisitable.Accept(object visitor)
        {
            return ((QdbApi) visitor).BlobScan(_pattern, _maxCount);
        }
    }

    /// <summary>
    /// Selects the blobs whose content match the given regular expression.
    /// </summary>
    public sealed class QdbRegexSelector : IQdbBlobSelector
    {
        private readonly string _pattern;
        private readonly long _maxCount;

        /// <summary>
        /// Creates a selector from the given pattern.
        /// </summary>
        /// <param name="pattern">The pattern to be found in the blobs.</param>
        /// <param name="maxCount">The maximum number of results</param>
        public QdbRegexSelector(string pattern, long maxCount)
        {
            _pattern = pattern;
            _maxCount = maxCount;
        }

        object IVisitable.Accept(object visitor)
        {
            return ((QdbApi)visitor).BlobScanRegex(_pattern, _maxCount);
        }
    }
}
