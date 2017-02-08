using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbClusterTests
{
    [TestClass]
    public class Misc
    {
        [TestMethod]
        public void CreateAndDispose()
        {
            var cluster = new QdbCluster(DaemonRunner.ClusterUrl);
            cluster.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbInvalidArgumentException))]
        public void Constructor_ThrowsInvalidArgument()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new QdbCluster("clearly this is not a uri");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ThrowsArgumentNull()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new QdbCluster(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Blob_ThrowsArgumentNull()
        {
            QdbTestCluster.Instance.Blob(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Integer_ThrowsArgumentNull()
        {
            QdbTestCluster.Instance.Integer(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Queue_ThrowsArgumentNull()
        {
            QdbTestCluster.Instance.Deque(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HashSet_ThrowsArgumentNull()
        {
            QdbTestCluster.Instance.HashSet(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Stream_ThrowsArgumentNull()
        {
            QdbTestCluster.Instance.Stream(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Tag_ThrowsArgumentNull()
        {
            QdbTestCluster.Instance.Tag(null);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TimeSeries_ThrowsArgumentNull()
        {
            QdbTestCluster.Instance.TimeSeries(null);
        }
    }
}
