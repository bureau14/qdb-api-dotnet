using System;
using Quasardb.ManagedApi;

namespace Quasardb
{
    /// <summary>
    /// A connection to a quasardb database.
    /// </summary>
    public sealed class QdbCluster : IDisposable
    {
        readonly QdbApi _api;

        /// <summary>
        /// Connects to a quasardb database.
        /// </summary>
        /// <param name="uri">The URI of the quasardb database.</param>
        public QdbCluster(string uri)
        {
            _api = new QdbApi();
            _api.Connect(uri);
        }

        /// <summary>
        /// Close the connection to the database.
        /// </summary>
        public void Dispose()
        {
            _api.Dispose();
        }

        /// <summary>
        /// Returns a <see cref="QdbBlob" /> with the specified alias.
        /// </summary>
        /// <remarks>No operation is performed in the database.</remarks>
        /// <param name="alias">The alias of the blob in the database.</param>
        /// <returns>A blob associated with the specified alias.</returns>
        /// <seealso cref="QdbBlob"/>
        public QdbBlob Blob(string alias)
        {
            if (alias == null) throw new ArgumentNullException("alias");
            return new QdbBlob(_api, alias);
        }

        /// <summary>
        /// Returns a <see cref="QdbDeque" /> with the specified alias.
        /// </summary>
        /// <remarks>No operation is performed in the database.</remarks>
        /// <param name="alias">The alias of the queue in the database.</param>
        /// <returns>A queue associated with the specified alias.</returns>
        /// <seealso cref="QdbDeque"/>
        public QdbDeque Deque(string alias)
        {
            if (alias == null) throw new ArgumentNullException("alias");
            return new QdbDeque(_api, alias);
        }

        /// <summary>
        /// Returns a <see cref="QdbHashSet" /> with the specified alias.
        /// </summary>
        /// <remarks>No operation is performed in the database.</remarks>
        /// <param name="alias">The alias of the hash-set in the database.</param>
        /// <returns>A hash-set associated with the specified alias.</returns>
        /// <seealso cref="QdbHashSet"/>
        public QdbHashSet HashSet(string alias)
        {
            if (alias == null) throw new ArgumentNullException("alias");
            return new QdbHashSet(_api, alias);
        }

        /// <summary>
        /// Returns a <see cref="QdbInteger" /> with the specified alias.
        /// </summary>
        /// <remarks>No operation is performed in the database.</remarks>
        /// <param name="alias">The alias of the integer in the database.</param>
        /// <returns>An integer associated with the specified alias.</returns>
        /// <seealso cref="QdbInteger"/>
        public QdbInteger Integer(string alias)
        {
            if (alias == null) throw new ArgumentNullException("alias");
            return new QdbInteger(_api, alias);
        }

        /// <summary>
        /// Returns a <see cref="QdbTag" /> with the specified alias.
        /// </summary>
        /// <remarks>No operation is performed in the database.</remarks>
        /// <param name="alias">The alias of the tag in the database.</param>
        /// <returns>A tag associated with the specified alias.</returns>
        /// <seealso cref="QdbTag"/>
        public QdbTag Tag(string alias)
        {
            if (alias == null) throw new ArgumentNullException("alias");
            return new QdbTag(_api, alias);
        }
    }
}
