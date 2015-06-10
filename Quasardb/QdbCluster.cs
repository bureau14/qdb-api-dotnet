using System;
using Quasardb.Exceptions;
using Quasardb.Interop;

namespace Quasardb
{
    /// <summary>
    /// A connection to a quasardb database.
    /// </summary>
    public sealed class QdbCluster : IDisposable
    {
        readonly qdb_handle _handle;

        /// <summary>
        /// Connects to a quasardb database.
        /// </summary>
        /// <param name="uri">The URI of the quasardb database.</param>
        public QdbCluster(string uri)
        {
            if (uri == null) throw new ArgumentNullException("uri");

            _handle = qdb_api.qdb_open_tcp();

            var error = qdb_api.qdb_connect(_handle, uri);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }

        /// <summary>
        /// Close the connection.
        /// </summary>
        public void Dispose()
        {
            _handle.Dispose();
        }

        /// <summary>
        /// Return a <see cref="QdbBlob" /> with the specified alias.
        /// </summary>
        /// <remarks>No operation is performed in the database.</remarks>
        /// <param name="alias">The alias of the blob in the database.</param>
        /// <returns>A <see cref="QdbBlob" />.</returns>
        public QdbBlob Blob(string alias)
        {
            if (alias == null) throw new ArgumentNullException("alias");
            return new QdbBlob(_handle, alias);
        }

        /// <summary>
        /// Return a <see cref="QdbInteger" /> with the specified alias.
        /// </summary>
        /// <remarks>No operation is performed in the database.</remarks>
        /// <param name="alias">The alias of the integer in the database.</param>
        /// <returns>A <see cref="QdbInteger" />.</returns>
        public QdbInteger Integer(string alias)
        {
            if (alias == null) throw new ArgumentNullException("alias");
            return new QdbInteger(_handle, alias);
        }

        /// <summary>
        /// Return a <see cref="QdbQueue" /> with the specified alias.
        /// </summary>
        /// <remarks>No operation is performed in the database.</remarks>
        /// <param name="alias">The alias of the queue in the database.</param>
        /// <returns>A <see cref="QdbQueue" />.</returns>
        public QdbQueue Queue(string alias)
        {
            if (alias == null) throw new ArgumentNullException("alias");
            return new QdbQueue(_handle, alias);
        }

        /// <summary>
        /// Return a <see cref="QdbHashSet" /> with the specified alias.
        /// </summary>
        /// <remarks>No operation is performed in the database.</remarks>
        /// <param name="alias">The alias of the hash-set in the database.</param>
        /// <returns>A <see cref="HashSet" />.</returns>
        public QdbHashSet HashSet(string alias)
        {
            if (alias == null) throw new ArgumentNullException("alias");
            return new QdbHashSet(_handle, alias);
        }
    }
}
