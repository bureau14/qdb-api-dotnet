using System;
using System.Collections.Concurrent;

namespace Quasardb
{
    /// <summary>
    /// A configuration builder to define the properties of the connection.
    /// </summary>
    public class QdbConnectionFactory
    {
        string _uri;
        string _cluster_public_key;
        string _username;
        string _user_private_key;
        bool _is_secured = false;
        int _parallelism_count = -1;
        int _max_in_buffer_size = -1;
        QdbCompression _level = QdbCompression.None;

        /// <summary>
        /// Initialize a factory to create connections.
        /// </summary>
        public QdbConnectionFactory(string uri)
        {
            _uri = uri;
        }

        /// <summary>
        /// Adds security parameters.
        /// </summary>
        public QdbConnectionFactory WithSecurity(string clusterPublicKey, string userName, string userPrivateKey)
        {
            _is_secured = true;
            _cluster_public_key = clusterPublicKey;
            _username = userName;
            _user_private_key = userPrivateKey;

            return this;
        }

        /// <summary>
        /// Sets the number of thread used per connection.
        /// </summary>
        public QdbConnectionFactory WithParallelism(int count)
        {
            _parallelism_count = count;

            return this;
        }

        /// <summary>
        /// Sets the number of thread used per connection.
        /// </summary>
        public QdbConnectionFactory WithMaxInBufferSize(int max_size)
        {
            _max_in_buffer_size = max_size;

            return this;
        }

        /// <summary>
        /// Sets the compression level.
        /// </summary>
        public QdbConnectionFactory WithCompression(QdbCompression level)
        {
            _level = level;

            return this;
        }

        internal QdbCluster Create()
        {
            var c = _is_secured ? new QdbCluster(_uri, _cluster_public_key, _username, _user_private_key, _parallelism_count) : new QdbCluster(_uri, _parallelism_count);
            if (_max_in_buffer_size != -1)
            {
                c.SetMaxInBufferSize(_max_in_buffer_size);
            }
            c.SetCompression(_level);

            return c;
        }
    }

    /// <summary>
    /// A  entry in a quasardb database.
    /// </summary>
    public class QdbConnectionPool
    {
        private readonly BlockingCollection<QdbCluster> _connections;

        /// <summary>
        /// Create a pool using the connection factory.
        /// </summary>
        public QdbConnectionPool(QdbConnectionFactory factory, int size)
        {
            _connections = new BlockingCollection<QdbCluster>(size);
            for (int i = 0; i < size; i++)
            {
                _connections.Add(factory.Create());
            }
        }

        /// <summary>
        /// Get the maximum number of connections.
        /// </summary>
        public int Size() { return _connections.Count; }

        /// <summary>
        /// Get one connection from the pool.
        /// </summary>
        public QdbCluster Get()
        {
            return _connections.Take();
        }

        /// <summary>
        /// Tries to get one connection from the pool within the provided time span.
        /// </summary>
        public bool TryGet(out QdbCluster conn, TimeSpan span)
        {
            return _connections.TryTake(out conn, span);
        }

        /// <summary>
        /// Return one connection to the pool.
        /// </summary>
        public void Return(QdbCluster conn) => _connections.Add(conn);
    }
}
