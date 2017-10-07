using System;
using System.Collections.Generic;
using Quasardb.Exceptions;
using Quasardb.Native;
using Quasardb.TimeSeries;

namespace Quasardb
{
    /// <summary>
    /// A connection to a quasardb database.
    /// </summary>
    public sealed class QdbCluster : IDisposable
    {
        readonly qdb_handle _api;
        readonly QdbEntryFactory _factory;

        /// <summary>
        /// Connects to a quasardb database.
        /// </summary>
        /// <param name="uri">The URI of the quasardb database.</param>
        public QdbCluster(string uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            _api = qdb_api.qdb_open_tcp();

            var error = qdb_api.qdb_connect(_api, uri);
            QdbExceptionThrower.ThrowIfNeeded(error);
            _factory = new QdbEntryFactory(_api);
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
        /// <returns>A blob associated to the specified alias.</returns>
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
            using (var aliases = (QdbStringCollection)selector.Accept(_api))
            {
                foreach (var alias in aliases)
                    yield return new QdbBlob(_api, alias);
            }
        }

        /// <summary>
        /// Returns a <see cref="QdbDeque" /> attached to the specified alias.
        /// </summary>
        /// <remarks>No operation is performed in the database.</remarks>
        /// <param name="alias">The alias (i.e. key) of the queue in the database.</param>
        /// <returns>A queue associated to the specified alias.</returns>
        /// <seealso cref="QdbDeque"/>
        public QdbDeque Deque(string alias)
        {
            if (alias == null) throw new ArgumentNullException(nameof(alias));
            return new QdbDeque(_api, alias);
        }

        /// <summary>
        /// Returns a <see cref="QdbEntry" /> attached to the specified alias.
        /// The actual type of the return value depends on the type of the entry in the database.
        /// </summary>
        /// <param name="alias"></param>
        /// <returns>An entry associated to the specified alias.</returns>
        /// <exception cref="QdbAliasNotFoundException">If the entry doesn't exists.</exception>
        public QdbEntry Entry(string alias)
        {
            if (alias == null) throw new ArgumentNullException(nameof(alias));
            return _factory.Create(alias);
        }

        /// <summary>
        /// Returns a collection of <see cref="QdbEntry" /> matching the given criteria.
        /// </summary>
        /// <param name="selector">The criteria to filter the entries</param>
        /// <returns>A collection of entry.</returns>
        public IEnumerable<QdbEntry> Entries(IQdbEntrySelector selector)
        {
            using (var aliases = (QdbStringCollection)selector.Accept(_api))
            {
                foreach (var alias in aliases)
                    yield return _factory.Create(alias);
            }
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
            var managedOps = batch.Operations;
            if (managedOps.Count == 0) return;

            var nativeOps = new qdb_operation[managedOps.Count];

            var err = qdb_api.qdb_init_operations(nativeOps, (UIntPtr) nativeOps.Length);
            QdbExceptionThrower.ThrowIfNeeded(err);
            
            for (var i = 0; i < managedOps.Count; i++)
            {
                managedOps[i].MarshalTo(ref nativeOps[i]);
            }
            qdb_api.qdb_run_batch(_api, nativeOps,  (UIntPtr) nativeOps.Length);
            for (var i = 0; i < managedOps.Count; i++)
            {
                managedOps[i].UnmarshalFrom(ref nativeOps[i]);
            }

            err = qdb_api.qdb_free_operations(_api, nativeOps, (UIntPtr) nativeOps.Length);
            QdbExceptionThrower.ThrowIfNeeded(err);
        }

    }
}
