using System;
using System.Collections.Generic;
using Quasardb.Exceptions;
using Quasardb.Native;
using Quasardb.TimeSeries;
using Quasardb.Query;
using Quasardb.TimeSeries.Writer;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace Quasardb
{
    /// <summary>
    /// Specifies compression mode.
    /// </summary>
    public enum QdbCompression
    {
        /// <summary>
        /// Disable compression.
        /// </summary>
        None,

        /// <summary>
        /// Maximum compression speed, potentially minimum compression ratio (default).
        /// </summary>
        Fast,

        /// <summary>
        /// Maximum compression ratio, potentially minimum compression speed.
        /// </summary>
        Best
    }

    /// <summary>
    /// A connection to a quasardb database.
    /// </summary>
    public sealed class QdbCluster : SafeHandle
    {
        readonly qdb_handle _handle;
        readonly QdbEntryFactory _factory;

        /// <summary>
        /// Connects to a quasardb database.
        /// </summary>
        /// <param name="uri">The URI of the quasardb database.</param>
        public QdbCluster(string uri) : base(IntPtr.Zero, true)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            _handle = qdb_api.qdb_open_tcp();

            var error = qdb_api.qdb_connect(_handle, uri);
            QdbExceptionThrower.ThrowIfNeeded(error);
            _factory = new QdbEntryFactory(_handle);
        }

        /// <summary>
        /// Connects securely to a quasardb database.
        /// </summary>
        /// <param name="uri">The URI of the quasardb database.</param>
        /// <param name="clusterPublicKey">Cluster public key used for database authentication.</param>
        /// <param name="userName">User name used for connection.</param>
        /// <param name="userPrivateKey">User private key used for connection.</param>
        public QdbCluster(string uri, string clusterPublicKey, string userName, string userPrivateKey) : base(IntPtr.Zero, true)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            _handle = qdb_api.qdb_open_tcp();

            var error = qdb_api.qdb_option_set_cluster_public_key(_handle, clusterPublicKey);
            QdbExceptionThrower.ThrowIfNeeded(error);

            error = qdb_api.qdb_option_set_user_credentials(_handle, userName, userPrivateKey);
            QdbExceptionThrower.ThrowIfNeeded(error);

            error = qdb_api.qdb_connect(_handle, uri);
            QdbExceptionThrower.ThrowIfNeeded(error);

            _factory = new QdbEntryFactory(_handle);
        }

        /// <inheritdoc />
        ~QdbCluster()
        {
            this.Dispose();
        }

        /// <summary>
        /// Close the connection to the database.
        /// </summary>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle()
        {
            _handle.Dispose();
            return true;
        }

        /// <inheritdoc />
        public override bool IsInvalid
        {
            get { return _handle == null || _handle.IsInvalid; }
        }

        /// <summary>
        /// Gets the last API error description.
        /// <returns>A message describing the error occurred during the last operation.</returns>
        /// </summary>
        public string GetLastError()
        {
            qdb_error error;
            qdb_sized_string message;
            qdb_api.qdb_get_last_error(_handle, out error, out message);
            return message.ToString();
        }

        /// <summary>
        /// Set the compression level.
        /// </summary>
        /// <param name="level">The level of compression to be used for the current handle to cluster.</param>
        public void SetCompression(QdbCompression level)
        {
            var error = qdb_api.qdb_option_set_compression(_handle, (qdb_compression)level);
            QdbExceptionThrower.ThrowIfNeeded(error);
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
            return new QdbBlob(_handle, alias);
        }

        /// <summary>
        /// Returns a collection of <see cref="QdbBlob" />  matching the given criteria.
        /// </summary>
        /// <param name="selector">The criteria to filter the blobs.</param>
        /// <returns>A collection of blob matching the criteria.</returns>
        public IEnumerable<QdbBlob> Blobs(IQdbBlobSelector selector)
        {
            using (var aliases = (qdb_buffer<qdb_string>)selector.Accept(_handle))
            {
                foreach (var alias in aliases)
                    yield return new QdbBlob(_handle, alias);
            }
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
            using (var aliases = (qdb_buffer<qdb_string>)selector.Accept(_handle))
            {
                foreach (var alias in aliases)
                    yield return _factory.Create(alias);
            }
        }

        /// <summary>
        /// Returns a collection of <see cref="String" /> matching the given criteria.
        /// </summary>
        /// <param name="selector">The criteria to filter the entries</param>
        /// <returns>A collection of entry.</returns>
        public IEnumerable<String> Keys(IQdbEntrySelector selector)
        {
            using (var aliases = (qdb_buffer<qdb_string>)selector.Accept(_handle))
            {
                foreach (var alias in aliases)
                    yield return alias;
            }
        }

        /// <summary>
        /// Returns a <see cref="QdbInteger" /> attached to the specified alias.
        /// </summary>
        /// <remarks>No operation is performed in the database.</remarks>
        /// <param name="alias">The alias (i.e. key) of the integer in the database.</param>
        /// <returns>An integer associated to the specified alias.</returns>
        /// <seealso cref="QdbInteger"/>
        public QdbInteger Integer(string alias)
        {
            if (alias == null) throw new ArgumentNullException(nameof(alias));
            return new QdbInteger(_handle, alias);
        }

        /// <summary>
        /// Returns a <see cref="QdbTag" /> attached to the specified alias.
        /// </summary>
        /// <remarks>No operation is performed in the database.</remarks>
        /// <param name="alias">The alias (i.e. key) of the tag in the database.</param>
        /// <returns>A tag associated to the specified alias.</returns>
        /// <seealso cref="QdbTag"/>
        public QdbTag Tag(string alias)
        {
            if (alias == null) throw new ArgumentNullException(nameof(alias));
            return new QdbTag(_handle, alias);
        }

        /// <summary>
        /// Executes the operations contained in the batch.
        /// </summary>
        /// <param name="batch">The collection of operation to execute.</param>
        public void RunBatch(QdbBatch batch)
        {
            if (batch.Operations.Count == 0) return;

            using (var nativeBatch = new QdbNativeBatch(_handle, batch.Operations))
            {
                nativeBatch.Run();
            }
        }

        /// <summary>
        /// Returns a <see cref="QdbTable" /> attached to the specified alias.
        /// </summary>
        /// <remarks>No operation is performed in the database.</remarks>
        /// <param name="alias">The alias (i.e. key) of the table in the database.</param>
        /// <returns>A table associated to the specified alias.</returns>
        /// <seealso cref="QdbTable"/>
        public QdbTable Table(string alias)
        {
            if (alias == null) throw new ArgumentNullException(nameof(alias));
            return new QdbTable(_handle, alias);
        }

        /// <summary>
        /// Returns a <see cref="QdbTimeSeries" /> attached to the specified alias.
        /// </summary>
        /// <remarks>No operation is performed in the database.</remarks>
        /// <param name="alias">The alias (i.e. key) of the time series in the database.</param>
        /// <returns>A time series associated to the specified alias.</returns>
        /// <seealso cref="Table"/>
        [Obsolete("Use QdbCluster.Table(string) instead")]
        public QdbTimeSeries TimeSeries(string alias)
        {
            if (alias == null) throw new ArgumentNullException(nameof(alias));
            return new QdbTimeSeries(_handle, alias);
        }

        /// <summary>
        /// Returns a <see cref="QdbTableWriter" /> attached to the specified columns.
        /// </summary>
        /// <param name="columnDefinitions">The description of the columns</param>
        /// <exception cref="QdbInvalidArgumentException">If columns list is empty.</exception>
        /// <returns>A batch table for bulk insertion associated with the specified columns.</returns>
        /// <seealso cref="QdbTableWriter"/>
        public QdbTableWriter Writer(params QdbBatchColumnDefinition[] columnDefinitions)
        {
            return Writer((IEnumerable<QdbBatchColumnDefinition>)columnDefinitions);
        }

        /// <summary>
        /// Returns a <see cref="QdbTableWriter" /> attached to the specified columns.
        /// </summary>
        /// <param name="columnDefinitions">The description of the columns</param>
        /// <exception cref="QdbInvalidArgumentException">If columns list is empty.</exception>
        /// <returns>A batch table for bulk insertion associated with the specified columns.</returns>
        /// <seealso cref="QdbTableWriter"/>
        public QdbTableWriter Writer(IEnumerable<QdbBatchColumnDefinition> columnDefinitions)
        {
            return new QdbTableWriter(_handle, columnDefinitions);
        }

        /// <summary>
        /// Run the provided query and creates a table directory with the results.
        /// </summary>
        /// <remarks>Queries are transactional. The complexity of this function
        /// is dependent on the complexity of the query.</remarks>
        /// <param name="query">The string representing the query to perform.</param>
        /// <returns>A <see cref="QdbQueryResult" /> holding the results of the query.</returns>
        /// <seealso cref="QdbQueryResult"/>
        public QdbQueryResult Query(string query)
        {
            return new QdbQueryResult(_handle, query);
        }
    }
}
