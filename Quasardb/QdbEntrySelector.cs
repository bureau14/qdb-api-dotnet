using System;
using System.Collections.Generic;
using System.Text;
using Quasardb.ManagedApi;

namespace Quasardb
{
    /// <summary>
    /// A way to select entries.
    /// </summary>
    /// <seealso cref="QdbCluster.Entries"/>
    public interface IQdbEntrySelector : IVisitable
    {
    }

    /// <summary>
    /// Selects entries by the beginning (ie prefix) of their alias.
    /// </summary>
    public class QdbPrefixSelector : IQdbEntrySelector
    {
        private readonly string _prefix;
        private readonly long _maxCount;

        /// <summary>
        /// Creates a selector with the specified prefix.
        /// </summary>
        /// <param name="prefix">The prefix that the aliases need to match.</param>
        /// <param name="maxCount">The maximum number of results.</param>
        public QdbPrefixSelector(string prefix, long maxCount)
        {
            _prefix = prefix;
            _maxCount = maxCount;
        }

        object IVisitable.Accept(object visitor)
        {
            return ((QdbApi)visitor).PrefixGet(_prefix, _maxCount);
        }
    }

    /// <summary>
    /// Selects entries by the end (ie suffix) of their alias.
    /// </summary>
    public struct QdbSuffixSelector : IQdbEntrySelector
    {
        private readonly string _suffix;
        private readonly long _maxCount;

        /// <summary>
        /// Creates a selector with the specified suffix.
        /// </summary>
        /// <param name="suffix">The suffix that the aliases need to match.</param>
        /// <param name="maxCount">The maximum number of results.</param>
        public QdbSuffixSelector(string suffix, long maxCount)
        {
            _suffix = suffix;
            _maxCount = maxCount;
        }

        object IVisitable.Accept(object visitor)
        {
            return ((QdbApi)visitor).SuffixGet(_suffix, _maxCount);
        }
    }

    /// <summary>
    /// Selects entries attached to a tag.
    /// </summary>
    public struct QdbTagSelector : IQdbEntrySelector
    {
        private readonly string _tag;

        /// <summary>
        /// Creates a selector for the specified tag.
        /// </summary>
        /// <param name="tag"></param>
        public QdbTagSelector(string tag)
        {
            _tag = tag;
        }

        object IVisitable.Accept(object visitor)
        {
            return ((QdbApi)visitor).GetTagged(_tag);
        }
    }
}
