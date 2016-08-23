using System;
using Quasardb.Exceptions;
using Quasardb.ManagedApi;

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
        internal QdbInteger(QdbApi api, string alias) : base(api, alias)
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
            return Api.IntegerAdd(Alias, addend);
        }

        /// <summary>
        /// Gets the value of the integer in the database.
        /// </summary>
        /// <returns>The value of the integer in the database.</returns>
        /// <exception cref="QdbAliasNotFoundException">The integer doesn't exist in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The entry in the database is not an integer.</exception>
        public long Get()
        {
            return Api.IntegerGet(Alias);
        }

        /// <summary>
        /// Sets the value of the integer, fails if it already exists.
        /// </summary>
        /// <param name="value">The initial value of the new integer.</param>
        /// <param name="expiryTime">The expiry time of the integer.</param>
        /// <exception cref="QdbAliasAlreadyExistsException">The entry already exists in the database./</exception>
        public void Put(long value, DateTime? expiryTime = null)
        {
            Api.IntegerPut(Alias, value, expiryTime);
        }

        /// <summary>
        /// Sets the value of the integer, fails if it already exists.
        /// </summary>
        /// <param name="value">The new value of the integer.</param>
        /// <param name="expiryTime">The expiry time of the integer.</param>
        /// <exception cref="QdbIncompatibleTypeException">The entry in the database is not an integer.</exception>
        public bool Update(long value, DateTime? expiryTime = null)
        {
            return Api.IntegerUpdate(Alias, value, expiryTime);
        }
    }
}
