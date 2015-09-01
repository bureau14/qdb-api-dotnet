using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests
{
    [TestClass]
    public class QdbClusterTests
    {
        [TestMethod]
        public void CreateAndDispose()
        {
            var cluster = new QdbCluster(DaemonRunner.ClusterUrl);
            cluster.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbInvalidArgumentException))]
        public void ConnectToAnInvalidAddress()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new QdbCluster("clearly this is not a uri");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConnectToNull()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new QdbCluster(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BlobNull()
        {
            QdbTestCluster.Instance.Blob(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IntegerNull()
        {
            QdbTestCluster.Instance.Integer(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void QueueNull()
        {
            QdbTestCluster.Instance.Deque(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HashSetNull()
        {
            QdbTestCluster.Instance.HashSet(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TagNull()
        {
            QdbTestCluster.Instance.Tag(null);
        }
    }
}
