﻿using Quasardb.Exceptions;
using Quasardb.Native;

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
    public sealed class QdbPrefixSelector : IQdbEntrySelector
    {
        readonly string _prefix;
        readonly long _maxCount;

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
            var handle = (qdb_handle)visitor;
            var result = new qdb_buffer<qdb_string>(handle);

            var error = qdb_api.qdb_prefix_get(handle, _prefix, _maxCount, out result.Pointer, out result.Size);

            switch (error)
            {
                case qdb_error.qdb_e_ok:
                case qdb_error.qdb_e_alias_not_found:
                    return result;

                default:
                    throw QdbExceptionFactory.Create(error);
            }
        }
    }

    /// <summary>
    /// Selects entries by the end (ie suffix) of their alias.
    /// </summary>
    public sealed class QdbSuffixSelector : IQdbEntrySelector
    {
        readonly string _suffix;
        readonly long _maxCount;

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
            var handle = (qdb_handle)visitor;
            var result = new qdb_buffer<qdb_string>(handle);

            var error = qdb_api.qdb_suffix_get(handle, _suffix, _maxCount, out result.Pointer, out result.Size);

            switch (error)
            {
                case qdb_error.qdb_e_ok:
                case qdb_error.qdb_e_alias_not_found:
                    return result;

                default:
                    throw QdbExceptionFactory.Create(error);
            }
        }
    }

    /// <summary>
    /// Selects entries attached to a tag.
    /// </summary>
    public sealed class QdbTagSelector : IQdbEntrySelector
    {
        readonly string _tag;

        /// <summary>
        /// Creates a selector for the specified tag.
        /// </summary>
        /// <param name="tag">The tag to which entries are attached.</param>
        public QdbTagSelector(string tag)
        {
            _tag = tag;
        }

        object IVisitable.Accept(object visitor)
        {
            var handle = (qdb_handle) visitor;
            var result = new qdb_buffer<qdb_string>(handle);
            var error = qdb_api.qdb_get_tagged(handle, _tag, out result.Pointer, out result.Size);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: _tag);
            return result;
        }
    }
}
