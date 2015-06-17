using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

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
            _queue = QdbTestCluster.CreateEmptyQueue();
            _content1 = RandomGenerator.CreateRandomContent();
            _content2 = RandomGenerator.CreateRandomContent();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PushFront_Null()
        {
            _queue.PushFront(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PushBack_Null()
        {
            _queue.PushBack(null);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void PopBack()
        {
            _queue.PopBack();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void PopFront()
        {
            _queue.PopFront();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void Back()
        {
            _queue.Back();          
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void Front()
        {
            _queue.Front();
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

        [TestMethod]
        public void PushFront_Back()
        {
            _queue.PushFront(_content1);
            _queue.PushFront(_content2);
            var result = _queue.Back();

            CollectionAssert.AreEqual(_content1, result);
        }

        [TestMethod]
        public void PushFront_Front()
        {
            _queue.PushFront(_content1);
            _queue.PushFront(_content2);
            var result = _queue.Front();

            CollectionAssert.AreEqual(_content2, result);
        }

        [TestMethod]
        public void PushBack_PopBack()
        {
            _queue.PushBack(_content1);
            _queue.PushBack(_content2);
            var result1 = _queue.PopBack();
            var result2 = _queue.PopBack();

            CollectionAssert.AreEqual(_content2, result1);
            CollectionAssert.AreEqual(_content1, result2);
        }

        [TestMethod]
        public void PushFront_PopFront()
        {
            _queue.PushFront(_content1);
            _queue.PushFront(_content2);
            var result1 = _queue.PopFront();
            var result2 = _queue.PopFront();

            CollectionAssert.AreEqual(_content2, result1);
            CollectionAssert.AreEqual(_content1, result2);
        }
    }
}
