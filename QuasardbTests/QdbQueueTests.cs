using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;

namespace QuasardbTests
{
    [TestClass]
    public class QdbQueueTests
    {
        QdbQueue _queue;
        byte[] _content1, _content2;

        [TestInitialize]
        public void Initialize()
        {
            var cluster = new QdbCluster(DaemonRunner.ClusterUrl);
            var alias = Utils.CreateUniqueAlias();
            _queue = cluster.Queue(alias);
            _content1 = Utils.CreateRandomContent();
            _content2 = Utils.CreateRandomContent();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PushFront_Null()
        {
            _queue.PushFront(null);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void PopBack()
        {
            _queue.PopBack();
        }

        [TestMethod]
        public void PushFront_PopBack()
        {
            _queue.PushFront(_content1);
            _queue.PushFront(_content2);
            var result1 = _queue.PopBack();
            var result2 = _queue.PopBack();

            CollectionAssert.AreEqual(_content1, result1);
            CollectionAssert.AreEqual(_content2, result2);
        }
    }
}
