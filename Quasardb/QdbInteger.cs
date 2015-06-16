using Quasardb.Exceptions;
using Quasardb.Interop;

namespace Quasardb
{
    /// <summary>
    /// An atomic integer in the database.
    /// </summary>
    public sealed class QdbInteger : QdbExpirableEntry
    {
        internal QdbInteger(qdb_handle handle, string alias) : base(handle, alias)
        {
        }
        
        /// <summary>
        /// Modifies the value of the integer in the database.
        /// </summary>
        /// <param name="addend">The value to add.</param>
        /// <returns>The new value of the integer in the database.</returns>
        /// <exception cref="QdbIncompatibleTypeException">The entry in the database is not an integer.</exception>
        public long Add(long addend)
        {
            long result;
            var error = qdb_api.qdb_int_add(Handle, Alias, addend, out result);
            QdbExceptionThrower.ThrowIfNeeded(error);
            return result;
        }

        /// <summary>
        /// Gets the value of the integer in the database.
        /// </summary>
        /// <returns>The value of the integer in the database.</returns>
        /// <exception cref="QdbIncompatibleTypeException">The entry in the database is not an integer.</exception>
        public long Get()
        {
            long value;
            var error = qdb_api.qdb_int_get(Handle, Alias, out value);
            QdbExceptionThrower.ThrowIfNeeded(error);
            return value;
        }

        /// <summary>
        /// Sets the value of the integer, fails if it already exists.
        /// </summary>
        /// <param name="value">The initial value of the new integer.</param>
        /// <exception cref="QdbAliasAlreadyExistsException">The entry already exists in the database./</exception>
        public QdbInteger Put(long value)
        {
            var error = qdb_api.qdb_int_put(Handle, Alias, value);
            QdbExceptionThrower.ThrowIfNeeded(error);
            return this;
        }

        /// <summary>
        /// Sets the value of the integer, fails if it already exists.
        /// </summary>
        /// <param name="value">The new value of the integer.</param>
        /// <exception cref="QdbIncompatibleTypeException">The entry in the database is not an integer.</exception>
        public QdbInteger Update(long value)
        {
            var error = qdb_api.qdb_int_update(Handle, Alias, value);
            QdbExceptionThrower.ThrowIfNeeded(error);
            return this;
        }
    }
}
