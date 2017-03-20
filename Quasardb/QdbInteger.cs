using System;
using Quasardb.Exceptions;
using Quasardb.Native;

namespace Quasardb
{
    /// <summary>
    /// An atomic integer in the database.
    /// </summary>
    /// <example>
    /// Here is how to set an integer blob in the database:
    /// <code language="c#">
    /// var cluster = new QdbCluster("qdb://127.0.0.1:2836");
    /// 
    /// cluster.Integer("some name").Put(42);
    /// </code>
    /// <code language="vb">
    /// Dim cluster = New QdbCluster("qdb://127.0.0.1:2836")
    /// 
    /// cluster.Integer("some name").Put(42)
    /// </code>
    /// </example>
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
        /// <exception cref="QdbAliasNotFoundException">The integer doesn't exist in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The entry in the database is not an integer.</exception>
        public long Add(long addend)
        {
            long result;
            var error = qdb_api.qdb_int_add(Handle, Alias, addend, out result);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: Alias);
            return result;
        }

        /// <summary>
        /// Gets the value of the integer in the database.
        /// </summary>
        /// <returns>The value of the integer in the database.</returns>
        /// <exception cref="QdbAliasNotFoundException">The integer doesn't exist in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The entry in the database is not an integer.</exception>
        public long Get()
        {
            long value;
            var error = qdb_api.qdb_int_get(Handle, Alias, out value);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: Alias);
            return value;
        }

        /// <summary>
        /// Sets the value of the integer, fails if it already exists.
        /// </summary>
        /// <param name="value">The initial value of the new integer.</param>
        /// <param name="expiryTime">The expiry time of the integer.</param>
        /// <exception cref="QdbAliasAlreadyExistsException">The entry already exists in the database./</exception>
        public void Put(long value, DateTime? expiryTime = null)
        {
            var error = qdb_api.qdb_int_put(Handle, Alias, value, qdb_time.FromOptionalDateTime(expiryTime));
            QdbExceptionThrower.ThrowIfNeeded(error, alias: Alias);
        }

        /// <summary>
        /// Sets the value of the integer, fails if it already exists.
        /// </summary>
        /// <param name="value">The new value of the integer.</param>
        /// <param name="expiryTime">The expiry time of the integer.</param>
        /// <returns><c>true</c> if the integer has been created, or <c>false</c> if it has been replaced.</returns>
        /// <exception cref="QdbIncompatibleTypeException">The entry in the database is not an integer.</exception>
        public bool Update(long value, DateTime? expiryTime = null)
        {
            var error = qdb_api.qdb_int_update(Handle, Alias, value, qdb_time.FromOptionalDateTime(expiryTime));

            switch (error)
            {
                case qdb_error_t.qdb_e_ok:
                    return false;

                case qdb_error_t.qdb_e_ok_created:
                    return true;

                default:
                    throw QdbExceptionFactory.Create(error, alias: Alias);
            }
        }
    }
}
