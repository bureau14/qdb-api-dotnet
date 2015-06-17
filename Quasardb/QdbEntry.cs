using System.Collections.Generic;
using Quasardb.Exceptions;
using Quasardb.Internals;
using Quasardb.Interop;

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
        public string Alias { get; private set; }

        /// <summary>
        /// The C API handle.
        /// </summary>
        internal qdb_handle Handle { get; private set; }

        /// <summary>
        /// Deletes the entry.
        /// </summary>
        /// <exception cref="QdbAliasNotFoundException">The entry doesn't exists in the database.</exception>
        public void Remove()
        {
            var error = qdb_api.qdb_remove(Handle, Alias);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }

        /// <summary>
        /// Adds a tag on the entry.
        /// </summary>
        /// <param name="tag">The tag to add.</param>
        /// <returns><c>true</c> if the tag was added, <c>false</c> if the entry already had this tag.</returns>
        public bool AddTag(QdbTag tag)
        {
            return QdbTagHelper.AddTag(Handle, Alias, tag.Alias);
        }

        /// <summary>
        /// Adds a tag on the entry.
        /// </summary>
        /// <param name="tag">The alias of the tag.</param>
        /// <returns><c>true</c> if the tag was added, <c>false</c> if the entry already had this tag.</returns>
        public bool AddTag(string tag)
        {
            return QdbTagHelper.AddTag(Handle, Alias, tag);
        }

        /// <summary>
        /// Checks if the entry has the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns><c>true</c> if the entry has this tag, <c>false</c> if not.</returns>
        public bool HasTag(QdbTag tag)
        {
            return QdbTagHelper.HasTag(Handle, Alias, tag.Alias);
        }

        /// <summary>
        /// Checks if the entry has the specified tag.
        /// </summary>
        /// <param name="tag">The alias of the tag.</param>
        /// <returns><c>true</c> if the entry has this tag, <c>false</c> if not.</returns>
        public bool HasTag(string tag)
        {
            return QdbTagHelper.HasTag(Handle, Alias, tag);
        }

        /// <summary>
        /// Removes a tag on the entry.
        /// </summary>
        /// <param name="tag">The label of the tag.</param>
        /// <returns><c>true</c> if the entry had this tag, <c>false</c> if not.</returns>
        public bool RemoveTag(QdbTag tag)
        {
            return QdbTagHelper.RemoveTag(Handle, Alias, tag.Alias);
        }

        /// <summary>
        /// Removes a tag on the entry.
        /// </summary>
        /// <param name="tag">The label of the tag.</param>
        /// <returns><c>true</c> if the entry had this tag, <c>false</c> if not.</returns>
        public bool RemoveTag(string tag)
        {
            return QdbTagHelper.RemoveTag(Handle, Alias, tag);
        }
        
        /// <summary>
        /// Gets the tags of the entry
        /// </summary>
        /// <returns>A collection of <see cref="QdbTag"/>.</returns>
        public IEnumerable<QdbTag> GetTags()
        {
            var tags = new QdbTagCollection(Handle);

            var error = qdb_api.qdb_get_tags(Handle, Alias, out tags.Pointer, out tags.Size);
            QdbExceptionThrower.ThrowIfNeeded(error);

            return tags;
        }
    }
}