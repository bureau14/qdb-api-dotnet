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
        /// <param name="tag">A <see cref="QdbTag"/>.</param>
        /// <returns><c>true</c> if the tag was added, <c>false</c> if the entry already had this tag.</returns>
        public QdbEntry AddTag(QdbTag tag)
        {
            bool dummy;
            return AddTag(tag, out dummy);
        }
        
        /// <summary>
        /// Adds a tag on the entry.
        /// </summary>
        /// <param name="tag">The label of the tag.</param>
        /// <returns><c>this</c>, allowing to chain operations.</returns>
        public QdbEntry AddTag(string tag)
        {
            bool dummy;
            return AddTag(tag, out dummy);
        }

        /// <summary>
        /// Adds a tag on the entry.
        /// </summary>
        /// <param name="tag">The label of the tag.</param>
        /// <param name="alreadySet"><c>true</c> if the tag was already set, <c>false</c> if not.</param>
        /// <returns><c>this</c>, allowing to chain operations.</returns>
        public QdbEntry AddTag(QdbTag tag, out bool alreadySet)
        {
            return AddTag(tag.Alias, out alreadySet);
        }

        /// <summary>
        /// Adds a tag on the entry.
        /// </summary>
        /// <param name="tag">The label of the tag.</param>
        /// <param name="alreadySet"><c>true</c> if the tag was already set, <c>false</c> if not.</param>
        /// <returns><c>this</c>, allowing to chain operations.</returns>
        public QdbEntry AddTag(string tag, out bool alreadySet)
        {
            var error = qdb_api.qdb_add_tag(Handle, Alias, tag);

            switch (error)
            {
                case qdb_error.qdb_e_tag_already_set:
                    alreadySet = true;
                    break;

                case qdb_error.qdb_e_ok:
                    alreadySet = false;
                    break;

                default:
                    throw QdbExceptionFactory.Create(error);
            }

            return this;
        }

        /// <summary>
        /// Checks if the entry has the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns><c>true</c> if the entry has this tag, <c>false</c> if not.</returns>
        public bool HasTag(QdbTag tag)
        {
            return HasTag(tag.Alias);
        }

        /// <summary>
        /// Checks if the entry has the specified tag.
        /// </summary>
        /// <param name="tag">The label of the tag.</param>
        /// <returns><c>true</c> if the entry has this tag, <c>false</c> if not.</returns>
        public bool HasTag(string tag)
        {
            var error = qdb_api.qdb_has_tag(Handle, Alias, tag);
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
            var error = qdb_api.qdb_remove_tag(Handle, Alias, tag);
            if (error == qdb_error.qdb_e_tag_not_set) return false;
            QdbExceptionThrower.ThrowIfNeeded(error);
            return true;
        }
    }
}