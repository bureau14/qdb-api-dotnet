using Quasardb.Exceptions;
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
        /// <param name="tag">The label of the tag.</param>
        /// <returns><c>true</c> if the tag was added, <c>false</c> if the entry already had this tag.</returns>
        public bool AddTag(string tag)
        {
            var error = qdb_api.qdb_tag(Handle, Alias, tag);
            if (error == qdb_error.qdb_e_tag_already_set) return false;
            QdbExceptionThrower.ThrowIfNeeded(error);
            return true;
        }

        /// <summary>
        /// Adds a tag on the entry.
        /// </summary>
        /// <param name="tag">A <see cref="QdbTag"/>.</param>
        /// <returns><c>true</c> if the tag was added, <c>false</c> if the entry already had this tag.</returns>
        public bool AddTag(QdbTag tag)
        {
            return AddTag(tag.Alias);
        }

        /// <summary>
        /// Checks if the entry has the specified tag.
        /// </summary>
        /// <param name="tag">The label of the tag.</param>
        /// <returns><c>true</c> if the entry has this tag, <c>false</c> if not.</returns>
        public bool HasTag(string tag)
        {
            var error = qdb_api.qdb_is_tagged(Handle, Alias, tag);
            if (error == qdb_error.qdb_e_tag_not_set) return false;
            QdbExceptionThrower.ThrowIfNeeded(error);
            return true;
        }

        /// <summary>
        /// Removes a tag on the entry.
        /// </summary>
        /// <param name="tag">The label of the tag.</param>
        /// <returns><c>true</c> if the entry had this tag, <c>false</c> if not.</returns>
        public bool RemoveTag(string tag)
        {
            var error = qdb_api.qdb_untag(Handle, Alias, tag);
            if (error == qdb_error.qdb_e_tag_not_set) return false;
            QdbExceptionThrower.ThrowIfNeeded(error);
            return true;
        }
    }
}