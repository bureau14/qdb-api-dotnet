using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;

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
        public void PushFront_PopBack()
        {
            _queue.PushFront(_content1);
            _queue.PushFront(_content2);
        }
    }
}
