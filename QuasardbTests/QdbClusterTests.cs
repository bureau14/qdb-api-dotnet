using System;
using System.Linq;
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
        public void StreamNull()
        {
            QdbTestCluster.Instance.Stream(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TagNull()
        {
            QdbTestCluster.Instance.Tag(null);
        }

        [TestMethod]
        public void SearchByPrefix()
        {
            var prefix = RandomGenerator.CreateUniqueAlias();
            var aliases = new [] {prefix + "AAAA", prefix + "BBBB"};
            foreach (var alias in aliases)
                QdbTestCluster.CreateBlob(alias);

            var results = QdbTestCluster.Instance.Search(prefix, 10, QdbSearchMode.Prefix);

            CollectionAssert.AreEqual(aliases, results.Select(x => x.Alias).ToArray());
        }

        [TestMethod]
        public void SearchBySuffix()
        {
            var suffix = RandomGenerator.CreateUniqueAlias();
            var aliases = new[] { "AAAA" + suffix, "BBBB" + suffix };
            foreach (var alias in aliases)
                QdbTestCluster.CreateBlob(alias);

            var results = QdbTestCluster.Instance.Search(suffix, 10, QdbSearchMode.Suffix);

            CollectionAssert.AreEqual(aliases, results.Select(x => x.Alias).ToArray());
        }

        [TestMethod]
        public void SearchTag()
        {
            var tag = RandomGenerator.CreateUniqueAlias();
            var prefix = RandomGenerator.CreateUniqueAlias();
            var aliases = new[] { prefix + "AAAA", prefix + "BBBB" };
            foreach (var alias in aliases)
                QdbTestCluster.CreateBlob(alias).AttachTag(tag);

            var results = QdbTestCluster.Instance.Search(tag, 10, QdbSearchMode.Tag);

            CollectionAssert.AreEqual(aliases, results.Select(x => x.Alias).ToArray());
        }
    }
}
