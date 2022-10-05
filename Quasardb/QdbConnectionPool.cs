using System;
using System.Collections.Concurrent;

namespace Quasardb
{
    /// <summary>
    /// A  entry in a quasardb database.
    /// </summary>
    public class QdbConnectionPool
    {
        private readonly BlockingCollection<QdbCluster> _connections;

        /// <summary>
        /// Create a pool for unsecured connections.
        /// </summary>
        public QdbConnectionPool(string uri, int size)
        {
            _connections = new BlockingCollection<QdbCluster>(size);
            for (int i = 0; i < size; i++)
            {
                _connections.Add(new QdbCluster(uri));
            }
        }

        /// <summary>
        /// Create a pool for secured connections.
        /// </summary>
        public QdbConnectionPool(string uri, string clusterPublicKey, string userName, string userPrivateKey, int size)
        {
            _connections = new BlockingCollection<QdbCluster>(size);
            for (int i = 0 ; i < size; i++)
            {
                _connections.Add(new QdbCluster(uri, clusterPublicKey, userName, userPrivateKey));
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