using System;
using Quasardb.Exceptions;
using Quasardb.Native;

namespace Quasardb
{
    /// <summary>
    /// An atomic double in the database.
    /// </summary>
    /// <example>
    /// Here is how to set a double blob in the database:
    /// <code language="c#">
    /// var cluster = new QdbCluster("qdb://127.0.0.1:2836");
    /// 
    /// cluster.Double("some name").Put(42.0);
    /// </code>
    /// </example>
    public sealed class QdbDouble : QdbExpirableEntry
    {
        internal QdbDouble(qdb_handle handle, string alias) : base(handle, alias)
        {
        }

        /// <summary>
        /// Modifies the value of the double in the database.
        /// </summary>
        /// <param name="addend">The value to add.</param>
        /// <returns>The new value of the double in the database.</returns>
        /// <exception cref="QdbAliasNotFoundException">The double doesn't exist in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The entry in the database is not a double.</exception>
        public double Add(double addend)
        {
            double result;
            var error = qdb_api.qdb_double_add(Handle, Alias, addend, out result);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: Alias);
            return result;
        }

        /// <summary>
        /// Gets the value of the double in the database.
        /// </summary>
        /// <returns>The value of the double in the database.</returns>
        /// <exception cref="QdbAliasNotFoundException">The double doesn't exist in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The entry in the database is not a double.</exception>
        public double Get()
        {
            double value;
            var error = qdb_api.qdb_double_get(Handle, Alias, out value);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: Alias);
            return value;
        }

        /// <summary>
        /// Sets the value of the double, fails if it already exists.
        /// </summary>
        /// <param name="value">The initial value of the new double.</param>
        /// <param name="expiryTime">The expiry time of the double.</param>
        /// <exception cref="QdbAliasAlreadyExistsException">The entry already exists in the database.</exception>
        public void Put(double value, DateTime? expiryTime = null)
        {
            var error = qdb_api.qdb_double_put(Handle, Alias, value, qdb_time.FromOptionalDateTime(expiryTime));
            QdbExceptionThrower.ThrowIfNeeded(error, alias: Alias);
        }

        /// <summary>
        /// Sets the value of the double, fails if it already exists.
        /// </summary>
        /// <param name="value">The new value of the double.</param>
        /// <param name="expiryTime">The expiry time of the double.</param>
        /// <returns><c>true</c> if the double has been created, or <c>false</c> if it has been replaced.</returns>
        /// <exception cref="QdbIncompatibleTypeException">The entry in the database is not a double.</exception>
        public bool Update(double value, DateTime? expiryTime = null)
        {
            var error = qdb_api.qdb_double_update(Handle, Alias, value, qdb_time.FromOptionalDateTime(expiryTime));

            switch (error)
            {
                case qdb_error.qdb_e_ok:
                    return false;

                case qdb_error.qdb_e_ok_created:
                    return true;

                default:
                    throw QdbExceptionFactory.Create(error, alias: Alias);
            }
        }
    }
}
