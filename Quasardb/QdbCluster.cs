using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
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
        /// Returns a <see cref="QdbBlob" /> attached to the specified alias.
        /// </summary>
        /// <remarks>No operation is performed in the database.</remarks>
        /// <param name="alias">The alias (i.e. key) of the blob in the database.</param>
        /// <returns>A blob associated attached to the specified alias.</returns>
        /// <seealso cref="QdbBlob"/>
        public QdbBlob Blob(string alias)
        {
            if (alias == null) throw new ArgumentNullException(nameof(alias));
            return new QdbBlob(_api, alias);
        }

        /// <summary>
        /// Returns a collection of <see cref="QdbBlob" />  matching the given criteria.
        /// </summary>
        /// <param name="selector">The criteria to filter the blobs.</param>
        /// <returns>A collection of blob matching the criteria.</returns>
        public IEnumerable<QdbBlob> Blobs(IQdbBlobSelector selector)
        {
            var aliases = (IEnumerable<string>)selector.Accept(_api);

            foreach (var alias in aliases)
                yield return new QdbBlob(_api, alias);
        }

        /// <summary>
        /// Returns a <see cref="QdbDeque" /> attached to the specified alias.
        /// </summary>
        /// <remarks>No operation is performed in the database.</remarks>
        /// <param name="alias">The alias (i.e. key) of the queue in the database.</param>
        /// <returns>A queue associated attached to the specified alias.</returns>
        /// <seealso cref="QdbDeque"/>
        public QdbDeque Deque(string alias)
        {
            if (alias == null) throw new ArgumentNullException(nameof(alias));
            return new QdbDeque(_api, alias);
        }

        /// <summary>
        /// Returns a collection of <see cref="QdbEntry" /> matching the given criteria.
        /// </summary>
        /// <returns>A collection of entry.</returns>
        public IEnumerable<QdbEntry> Entries(IQdbEntrySelector selector)
        {
            var factory = new QdbEntryFactory(_api);
            var aliases = (IEnumerable<string>) selector.Accept(_api);

            foreach (var alias in aliases)
                yield return factory.Create(alias);
        }
        
        /// <summary>
        /// Returns a <see cref="QdbHashSet" /> attached to the specified alias.
        /// </summary>
        /// <remarks>No operation is performed in the database.</remarks>
        /// <param name="alias">The alias (i.e. key) of the hash-set in the database.</param>
        /// <returns>A hash-set associated attached to the specified alias.</returns>
        /// <seealso cref="QdbHashSet"/>
        public QdbHashSet HashSet(string alias)
        {
            if (alias == null) throw new ArgumentNullException(nameof(alias));
            return new QdbHashSet(_api, alias);
        }

        /// <summary>
        /// Returns a <see cref="QdbInteger" /> attached to the specified alias.
        /// </summary>
        /// <remarks>No operation is performed in the database.</remarks>
        /// <param name="alias">The alias (i.e. key) of the integer in the database.</param>
        /// <returns>An integer associated attached to the specified alias.</returns>
        /// <seealso cref="QdbInteger"/>
        public QdbInteger Integer(string alias)
        {
            if (alias == null) throw new ArgumentNullException(nameof(alias));
            return new QdbInteger(_api, alias);
        }

        /// <summary>
        /// Returns a <see cref="QdbStream" /> attached to the specified alias.
        /// </summary>
        /// <param name="alias">The alias (i.e. key) of the stream in the database</param>
        /// <returns>A stream attached to the specified alias.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public QdbStream Stream(string alias)
        {
            if (alias == null) throw new ArgumentNullException(nameof(alias));
            return new QdbStream(_api, alias);
        }

        /// <summary>
        /// Returns a <see cref="QdbTag" /> attached to the specified alias.
        /// </summary>
        /// <remarks>No operation is performed in the database.</remarks>
        /// <param name="alias">The alias (i.e. key) of the tag in the database.</param>
        /// <returns>A tag associated attached to the specified alias.</returns>
        /// <seealso cref="QdbTag"/>
        public QdbTag Tag(string alias)
        {
            if (alias == null) throw new ArgumentNullException(nameof(alias));
            return new QdbTag(_api, alias);
        }

        /// <summary>
        /// Executes the operations contained in the batch.
        /// </summary>
        /// <param name="batch">The collection of operation to execute.</param>
        public void RunBatch(QdbBatch batch)
        {
            _api.RunBatch(batch.Operations);
        }
    }
}
