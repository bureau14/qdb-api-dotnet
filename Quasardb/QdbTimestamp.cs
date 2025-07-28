using System;
using Quasardb.Exceptions;
using Quasardb.Native;

namespace Quasardb
{
    /// <summary>
    /// An atomic timestamp entry in quasardb.
    /// </summary>
    /// <remarks>Instances can be obtained via <see cref="QdbCluster.Timestamp"/>.</remarks>

    /// <summary>
    /// An atomic timestamp entry in quasardb.
    /// </summary>
    /// <example>
    /// Here is how to set a timestamp in the database:
    /// <code language="c#">
    /// var cluster = new QdbCluster("qdb://127.0.0.1:2836");
    /// 
    /// cluster.Timestamp("some name").Put(TimeSpan.(...));
    /// </code>
    /// </example>

    public sealed class QdbTimestamp : QdbExpirableEntry
    {
        internal QdbTimestamp(qdb_handle handle, string alias) : base(handle, alias)
        {
        }

        private static qdb_timespec FromTimeSpan(TimeSpan span)
        {
            const long NanosPerTick = 100; // from TimeConverter
            long totalNanos = span.Ticks * NanosPerTick;
            qdb_timespec ts;
            ts.tv_sec = Math.DivRem(totalNanos, 1000000000, out long nsec);
            ts.tv_nsec = nsec;
            return ts;
        }

        /// <summary>
        /// Modifies the value of the timestamp in the database.
        /// </summary>
        /// <param name="addend">The value to add.</param>
        /// <returns>The new value of the timestamp in the database.</returns>
        /// <exception cref="QdbAliasNotFoundException">The timestamp doesn't exist in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The entry in the database is not an timestamp.</exception>
        public DateTime Add(TimeSpan addend)
        {
            var tsAdd = FromTimeSpan(addend);
            qdb_timespec result;
            unsafe
            {
                var error = qdb_api.qdb_timestamp_add(Handle, Alias, &tsAdd, out result);
                QdbExceptionThrower.ThrowIfNeeded(error, alias: Alias);
            }
            return TimeConverter.ToDateTime(result);
        }

        /// <summary>
        /// Gets the value of the timestamp in the database.
        /// </summary>
        /// <returns>The value of the timestamp in the database.</returns>
        /// <exception cref="QdbAliasNotFoundException">The timestamp doesn't exist in the database.</exception>
        /// <exception cref="QdbIncompatibleTypeException">The entry in the database is not an timestamp.</exception>
        public DateTime Get()
        {
            qdb_timespec value;
            var error = qdb_api.qdb_timestamp_get(Handle, Alias, out value);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: Alias);
            return TimeConverter.ToDateTime(value);
        }

        /// <summary>
        /// Sets the value of the timestamp, fails if it already exists.
        /// </summary>
        /// <param name="value">The initial value of the new timestamp.</param>
        /// <param name="expiryTime">The expiry time of the timestamp.</param>
        /// <exception cref="QdbAliasAlreadyExistsException">The entry already exists in the database./</exception>
        public unsafe void Put(DateTime value, DateTime? expiryTime = null)
        {
            var ts = TimeConverter.ToTimespec(value);
            var error = qdb_api.qdb_timestamp_put(Handle, Alias, &ts, qdb_time.FromOptionalDateTime(expiryTime));
            QdbExceptionThrower.ThrowIfNeeded(error, alias: Alias);
        }

        /// <summary>
        /// Sets the value of the timestamp, fails if it already exists.
        /// </summary>
        /// <param name="value">The new value of the timestamp.</param>
        /// <param name="expiryTime">The expiry time of the timestamp.</param>
        /// <returns><c>true</c> if the timestamp has been created, or <c>false</c> if it has been replaced.</returns>
        /// <exception cref="QdbIncompatibleTypeException">The entry in the database is not an timestamp.</exception>
        public unsafe bool Update(DateTime value, DateTime? expiryTime = null)
        {
            var ts = TimeConverter.ToTimespec(value);
            var error = qdb_api.qdb_timestamp_update(Handle, Alias, &ts, qdb_time.FromOptionalDateTime(expiryTime));
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