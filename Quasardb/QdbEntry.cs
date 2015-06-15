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

        public void AddTag(string tag)
        {
            var error = qdb_api.qdb_tag(Handle, Alias, tag);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }

        public bool HasTag(string tag)
        {
            var error = qdb_api.qdb_is_tagged(Handle, Alias, tag);
            if (error == qdb_error.qdb_e_tag_not_set) return false;
            QdbExceptionThrower.ThrowIfNeeded(error);
            return true;
        }
    }
}