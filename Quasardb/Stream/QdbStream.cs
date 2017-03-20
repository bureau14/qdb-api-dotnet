using System.IO;
using Quasardb.Exceptions;
using Quasardb.Native;

namespace Quasardb
{
    /// <summary>
    /// Specifies how to open a stream 
    /// </summary>
    public enum QdbStreamMode
    {
        /// <summary>
        /// Opens an existing stream. A <see cref="QdbAliasNotFoundException"/> is thrown if the file doesn't exist.
        /// </summary>
        Open,

        /// <summary>
        /// Opens the stream if it exists and seeks to the end of the stream, or creates a new stream.
        /// </summary>
        Append
    }

    /// <summary>
    /// A stream in the database.
    /// </summary>
    /// <example>
    /// Here is how to write or read data to a stream:
    /// <code language="c#">
    /// byte[] myData;
    /// var cluster = new QdbCluster("qdb://127.0.0.1:2836");
    /// 
    /// // To write to a stream:
    /// using (var stream = cluster.Stream("some name").Open(QdbStreamMode.Append))
    /// {
    ///     stream.Write(myData, 0, myData.Length);
    /// }
    /// 
    /// // To read from a stream:
    /// using (var stream = cluster.Stream("some name").Open(QdbStreamMode.Open))
    /// {
    ///     stream.Read(myData, 0, myData.Length);
    /// }
    /// </code>
    /// </example>
    public sealed class QdbStream : QdbEntry
    {
        internal QdbStream(qdb_handle handle, string alias) : base(handle, alias)
        {
        }

        /// <summary>
        /// Opens the quasardb stream. 
        /// </summary>
        /// <param name="mode">Specifies how the stream must be opened</param>
        /// <returns>A <see cref="Stream"/> with read, write and seek abilities.</returns>
        /// <exception cref="QdbAliasNotFoundException">The stream doesn't exist in the database.</exception>
        /// <exception cref="QdbResourceLockedException">The stream is locked by another client of the database.</exception>
        public Stream Open(QdbStreamMode mode)
        {
            qdb_stream_handle handle;
            var error = qdb_api.qdb_stream_open(Handle, Alias, (qdb_stream_mode) mode, out handle);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: Alias);
            return new QdbStreamAdapter(handle, mode == QdbStreamMode.Append);
        }
    }
}
