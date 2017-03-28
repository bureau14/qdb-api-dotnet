using System;
using System.Collections.Generic;
using Quasardb.Exceptions;
using Quasardb.Native;

namespace Quasardb
{
    /// <summary>
    /// An entry in a quasardb database.
    /// </summary>
    public abstract class QdbEntry
    {
        internal QdbEntry(qdb_handle handle, string alias)
        {
            Alias = alias;
            Handle = handle;
        }

        /// <summary>
        /// The alias of the entry in the database.
        /// </summary>
        public string Alias { get; }

        /// <summary>
        /// The Managed API wrapper.
        /// </summary>
        internal qdb_handle Handle { get; }

        /// <summary>
        /// Removes the entry from the database.
        /// </summary>
        /// <returns><c>true</c> if the entry was removed, or <c>false</c> if the entry didn't exist.</returns>
        public virtual bool Remove()
        {
            var error = qdb_api.qdb_remove(Handle, Alias);

            switch (error)
            {
                case qdb_error.qdb_e_ok:
                    return true;
                    
                case qdb_error.qdb_e_alias_not_found:
                    return false;

                default:
                    throw QdbExceptionFactory.Create(error, alias: Alias);
            }
        }

        /// <summary>
        /// Adds a tag to the entry.
        /// </summary>
        /// <param name="tag">The tag to add.</param>
        /// <returns><c>true</c> if the tag was added, <c>false</c> if the entry already had this tag.</returns>
        /// <exception cref="QdbAliasNotFoundException">The entry doesn't exists in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The tag's alias conflicts with another entry.</exception>
        /// <seealso cref="QdbTag"/>
        public bool AttachTag(QdbTag tag)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));

            return AttachTag(Alias, tag.Alias);
        }

        /// <summary>
        /// Adds a tag to the entry.
        /// </summary>
        /// <param name="tag">The alias of the tag.</param>
        /// <returns><c>true</c> if the tag was added, <c>false</c> if the entry already had this tag.</returns>
        /// <exception cref="QdbAliasNotFoundException">The entry doesn't exists in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The tag's alias conflicts with another entry.</exception>
        /// <seealso cref="QdbTag"/>
        public bool AttachTag(string tag)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));

            return AttachTag(Alias, tag);
        }

        internal bool AttachTag(string target, string tag)
        {
            var error = qdb_api.qdb_attach_tag(Handle, target, tag);

            switch (error)
            {
                case qdb_error.qdb_e_tag_already_set:
                    return false;

                case qdb_error.qdb_e_ok:
                    return true;

                case qdb_error.qdb_e_alias_not_found:
                    throw new QdbAliasNotFoundException(target);

                case qdb_error.qdb_e_incompatible_type:
                    throw new QdbIncompatibleTypeException(tag);

                default:
                    throw QdbExceptionFactory.Create(error);
            }
        }

        /// <summary>
        /// Gets the tags of the entry
        /// </summary>
        /// <returns>A collection of tag.</returns>
        /// <exception cref="QdbAliasNotFoundException">The entry doesn't exists in the database.</exception>
        /// <seealso cref="QdbTag"/>
        public IEnumerable<QdbTag> GetTags()
        {
            using (var result = new qdb_buffer<qdb_string>(Handle))
            {
                var error = qdb_api.qdb_get_tags(Handle, Alias, out result.Pointer, out result.Size);
                QdbExceptionThrower.ThrowIfNeeded(error, alias: Alias);

                // ReSharper disable once LoopCanBeConvertedToQuery (compatibility with .NET Framework 2.0)
                foreach (var tag in result)
                    yield return new QdbTag(Handle, tag);
            }
            
        }

        /// <summary>
        /// Checks if the entry has the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns><c>true</c> if the entry has this tag, <c>false</c> otherwise.</returns>
        /// <seealso cref="QdbTag"/>
        public bool HasTag(QdbTag tag)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));

            return HasTag(Alias, tag.Alias);
        }

        /// <summary>
        /// Checks if the entry has the specified tag.
        /// </summary>
        /// <param name="tag">The alias of the tag.</param>
        /// <returns><c>true</c> if the entry has this tag, <c>false</c> otherwise.</returns>
        /// <seealso cref="QdbTag"/>
        public bool HasTag(string tag)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));

            return HasTag(Alias, tag);
        }

        internal bool HasTag(string target, string tag)
        {
            var error = qdb_api.qdb_has_tag(Handle, target, tag);

            switch (error)
            {
                case qdb_error.qdb_e_tag_not_set:
                    return false;

                case qdb_error.qdb_e_ok:
                    return true;

                case qdb_error.qdb_e_alias_not_found:
                    throw new QdbAliasNotFoundException(target);

                case qdb_error.qdb_e_incompatible_type:
                    throw new QdbIncompatibleTypeException(tag);

                default:
                    throw QdbExceptionFactory.Create(error);
            }
        }

        /// <summary>
        /// Removes a tag from the entry.
        /// </summary>
        /// <param name="tag">The label of the tag.</param>
        /// <returns><c>true</c> if the entry had this tag, <c>false</c> if not.</returns>
        /// <exception cref="QdbAliasNotFoundException">The entry doesn't exists in the database.</exception>
        /// <seealso cref="QdbTag"/>
        public bool DetachTag(QdbTag tag)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));

            return DetachTag(Alias, tag.Alias);
        }

        /// <summary>
        /// Removes a tag from the entry.
        /// </summary>
        /// <param name="tag">The label of the tag.</param>
        /// <returns><c>true</c> if the entry had this tag, <c>false</c> if not.</returns>
        /// <exception cref="QdbAliasNotFoundException">The entry doesn't exists in the database.</exception>
        /// <seealso cref="QdbTag"/>
        public bool DetachTag(string tag)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));

            return DetachTag(Alias, tag);
        }

        internal bool DetachTag(string target, string tag)
        {
            var error = qdb_api.qdb_detach_tag(Handle, target, tag);

            switch (error)
            {
                case qdb_error.qdb_e_tag_not_set:
                    return false;

                case qdb_error.qdb_e_ok:
                    return true;

                case qdb_error.qdb_e_alias_not_found:
                    throw new QdbAliasNotFoundException(target);

                case qdb_error.qdb_e_incompatible_type:
                    throw new QdbIncompatibleTypeException(tag);

                default:
                    throw QdbExceptionFactory.Create(error);
            }
        }
    }
}
