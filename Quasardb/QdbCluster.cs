using Microsoft.Extensions.Logging;
using Quasardb.Exceptions;
using Quasardb.Native;
using Quasardb.Query;
using Quasardb.TimeSeries;
using Quasardb.TimeSeries.ExpWriter;
using Quasardb.TimeSeries.Reader;
using Quasardb.TimeSeries.Writer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

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
    public sealed class QdbCluster : SafeHandle, IDisposable
    {
        readonly qdb_handle _handle;
        readonly QdbEntryFactory _factory;
        QdbLogger _logger = null;

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
        /// Connects to a quasardb database.
        /// </summary>
        /// <param name="uri">The URI of the quasardb database.</param>
        /// <param name="parallelismCount">Number of threads. 
        /// Value of 0 means the number of logical processor cores divided by two.</param>
        public QdbCluster(string uri, int parallelismCount) : base(IntPtr.Zero, true)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            _handle = qdb_api.qdb_open_tcp();

            qdb_error error;
            if (parallelismCount != -1)
            {
                error = qdb_api.qdb_option_set_client_max_parallelism(_handle, parallelismCount);
                QdbExceptionThrower.ThrowIfNeeded(error);
            }

            error = qdb_api.qdb_connect(_handle, uri);
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

        /// <summary>
        /// Connects securely to a quasardb database.
        /// </summary>
        /// <param name="uri">The URI of the quasardb database.</param>
        /// <param name="clusterPublicKey">Cluster public key used for database authentication.</param>
        /// <param name="userName">User name used for connection.</param>
        /// <param name="userPrivateKey">User private key used for connection.</param>
        /// <param name="parallelismCount">Number of threads. 
        /// Value of 0 means the number of logical processor cores divided by two.</param>
        public QdbCluster(string uri, string clusterPublicKey, string userName, string userPrivateKey, int parallelismCount) : base(IntPtr.Zero, true)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            _handle = qdb_api.qdb_open_tcp();

            var error = qdb_api.qdb_option_set_cluster_public_key(_handle, clusterPublicKey);
            QdbExceptionThrower.ThrowIfNeeded(error);

            error = qdb_api.qdb_option_set_user_credentials(_handle, userName, userPrivateKey);
            QdbExceptionThrower.ThrowIfNeeded(error);

            if (parallelismCount != -1)
            {
                error = qdb_api.qdb_option_set_client_max_parallelism(_handle, parallelismCount);
                QdbExceptionThrower.ThrowIfNeeded(error);
            }

            error = qdb_api.qdb_connect(_handle, uri);
            QdbExceptionThrower.ThrowIfNeeded(error);

            _factory = new QdbEntryFactory(_handle);
        }

        internal bool _disposed;

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _logger?.Stop();
                    _logger = null;
                }

                base.Dispose(disposing);
                _disposed = true;
                if (disposing)
                    GC.SuppressFinalize(this);
            }
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
        public unsafe string GetLastError()
        {
            qdb_error error;
            qdb_sized_string* message = (qdb_sized_string*)IntPtr.Zero;
            var ec = qdb_api.qdb_get_last_error(_handle, out error, out message);
            if (ec != qdb_error.qdb_e_ok)
            {
                return qdb_api.qdb_error(ec);
            }

            var msg = message->ToString();
            qdb_api.qdb_release(_handle, (IntPtr)message);
            return msg;
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
        /// Sets the maximum incoming buffer size for all network operations of the client.
        /// </summary>
        /// <param name="max_size">The maximum input size in bytes</param>
        public void SetMaxInBufferSize(int max_size)
        {
            var error = qdb_api.qdb_option_set_client_max_in_buf_size(_handle, max_size);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }

        /// <summary>
        /// Forces tidying of cluster memory.
        /// </summary>
        /// <remarks>EXPERIMENTAL. Use at your own risk!</remarks>
        public void ClusterTidyMemory()
        {
            var error = qdb_api.qdb_option_cluster_tidy_memory(_handle);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }

        /// <summary>
        /// Sets the client-side soft memory limit.
        /// </summary>
        /// <remarks>EXPERIMENTAL. Use at your own risk!</remarks>
        /// <param name="limit">Number of bytes.</param>
        public void SetSoftMemoryLimit(long limit)
        {
            var error = qdb_api.qdb_option_set_client_soft_memory_limit(_handle, (ulong)limit);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }

        /// <summary>
        /// Gets client memory information.
        /// </summary>
        /// <returns>The information about the client-side memory.</returns>
        /// <remarks>EXPERIMENTAL. Use at your own risk!</remarks>
        public string GetClientMemoryInfo()
        {
            using (var content = new QdbBlobBuffer(_handle))
            {
                var error = qdb_api.qdb_option_client_get_memory_info(_handle, out content.Pointer, out content.Size);
                QdbExceptionThrower.ThrowIfNeeded(error);
                var bytes = content.GetBytes();
                return Encoding.UTF8.GetString(bytes);
            }
        }

        /// <summary>
        /// Forces tidying of client-side memory.
        /// </summary>
        /// <remarks>EXPERIMENTAL. Use at your own risk!</remarks>
        public void ClientTidyMemory()
        {
            var error = qdb_api.qdb_option_client_tidy_memory(_handle);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }

        /// <summary>
        /// Sets the timezone of the client
        /// by the current handle.
        /// </summary>
        /// <param name="timezone">The timezone to set.</param>
        public void SetTimezone(string timezone)
        {
            var error = qdb_api.qdb_option_set_timezone(_handle, timezone);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }

        /// <summary>
        /// Gets the timezone of the client
        /// by the current handle.
        /// <returns>A timezone name.</returns>
        /// </summary>
        public unsafe string GetTimezone()
        {
            IntPtr timezone = IntPtr.Zero;
            var error = qdb_api.qdb_option_get_timezone(_handle, out timezone);
            QdbExceptionThrower.ThrowIfNeeded(error);

            var msg = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(timezone);
            qdb_api.qdb_release(_handle, timezone);
            return msg;
        }

        /// <summary>
        /// Sets the maximum stabilization waiting time for operations.
        /// </summary>
        /// <param name="wait_ms">The maximum amount of time to wait, in ms.</param>
        public void SetStabilizationMaxWait(int wait_ms)
        {
            var error = qdb_api.qdb_option_set_stabilization_max_wait(_handle, wait_ms);
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
        /// Returns a <see cref="QdbTableExpWriter" /> attached to the specified columns.
        /// </summary>
        /// <param name="tables">The table names you wish to write to</param>
        /// <param name="options">The options used for the push</param>
        /// <returns>A batch table for bulk insertion associated with the table.</returns>
        /// <seealso cref="QdbTableExpWriter"/>
        public QdbTableExpWriter ExpWriter(string[] tables, QdbTableExpWriterOptions options)
        {
            return new QdbTableExpWriter(_handle, tables, options);
        }

        /// <summary>
        /// Returns a <see cref="QdbTableExpWriter" /> attached to the specified columns.
        /// </summary>
        /// <param name="options">The options used for the push</param>
        /// <returns>A batch table for bulk insertion associated with the table.</returns>
        /// <seealso cref="QdbTableExpWriter"/>
        public QdbTableExpWriter ExpWriter(QdbTableExpWriterOptions options)
        {
            return new QdbTableExpWriter(_handle, new string[] {}, options);
        }

        /// <summary>
        /// Returns a bulk reader associated with the specified tables and columns.
        /// </summary>
        public QdbBulkReader BulkReader(string[] columns, QdbBulkReaderTable[] tables)
        {
            return new QdbBulkReader(_handle, columns, tables);
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

        /// <summary>
        /// Run the provided query at repeated interval and sends a table directory with the results into the callback.
        /// </summary>
        /// <remarks>Queries are transactional. The complexity of this function
        /// is dependent on the complexity of the query.</remarks>
        /// <param name="query">The string representing the query to perform.</param>
        /// <param name="mode">The continuous query mode, one of (Full, NewValuesOnly).</param>
        /// <param name="refresh_rate">The resfresh rate of the query, in milliseconds.</param>
        /// <param name="callback">Your callback function, it will be invoked with the result from the query.</param>
        /// <returns>A <see cref="QdbContinuousQuery" /> holding the results of the query.</returns>
        /// <seealso cref="QdbContinuousQuery"/>
        public QdbContinuousQuery ContinuousQuery(string query, QdbContinuousQuery.Mode mode, TimeSpan refresh_rate, Func<QdbQueryResult, int> callback)
        {
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentException($"'{nameof(query)}' cannot be null or empty.", nameof(query));
            }

            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            return new QdbContinuousQuery(_handle, query, mode, refresh_rate, callback);
        }

        /// <summary>
        /// The client will store performance measures from the server.
        /// Disabled by default.
        /// </summary>
        public void EnablePerformanceTraces()
        {
            var error = qdb_api.qdb_perf_enable_client_tracking(_handle);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }

        /// <summary>
        /// The client will stop storing performance measures from the server.
        /// </summary>
        public void DisablePerformanceTraces()
        {
            var error = qdb_api.qdb_perf_disable_client_tracking(_handle);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }

        /// <summary>
        /// Returns all new performance traces since the last call to this function.
        /// </summary>
        public unsafe QdbPerformanceTrace[] GetPerformanceTraces()
        {
            qdb_perf_profile* profiles;
            UIntPtr profile_count;
            var error = qdb_api.qdb_perf_get_profiles(_handle, out profiles, out profile_count);
            QdbExceptionThrower.ThrowIfNeeded(error);

            var traces = QdbPerformanceTrace.CreateTraces(profiles, (int)profile_count);
            qdb_api.qdb_release(_handle, (IntPtr)profiles);
            return traces;
        }

        /// <summary>
        /// Releases all performances traces currently stored by the client.
        /// </summary>
        public void ClearPerformanceTraces()
        {
            var error = qdb_api.qdb_perf_clear_all_profiles(_handle);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }

        /// <summary>
        /// Starts a new logger
        /// </summary>
        public unsafe void StartLog(QdbLoggerBuilder builder)
        {
            if (_logger == null)
            {
                _logger = new QdbLogger(builder);
            }
        }

        /// <summary>
        /// Swaps the current logger with the new configuration builder
        /// </summary>
        public unsafe void SwapLog(QdbLoggerBuilder builder)
        {
            if (_logger != null)
            {
                StopLog();
            }
            StartLog(builder);
        }

        /// <summary>
        /// Stops the currently used logger
        /// </summary>
        public unsafe void StopLog()
        {
            if (_logger != null)
            {
                _logger.Stop();
                _logger = null;

            }
        }
    }
}
