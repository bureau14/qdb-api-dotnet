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
        [ExpectedException(typeof(QdbEmptyContainerException))]
        public void Back_Empty()
        {
            _queue.PushBack(RandomGenerator.CreateRandomContent());
            _queue.PopBack();
            _queue.Back();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void Back_NotFound()
        {
            _queue.Back();          
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void Back_Incompatible()
        {
            QdbTestCluster.CreateBlob(_queue.Alias);
            _queue.Back();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbEmptyContainerException))]
        public void Front_Empty()
        {
            _queue.PushFront(RandomGenerator.CreateRandomContent());
            _queue.PopFront();
            _queue.Front();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void Front_NotFound()
        {
            _queue.Front();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void Front_Incompatible()
        {
            QdbTestCluster.CreateBlob(_queue.Alias);
            _queue.Front();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbEmptyContainerException))]
        public void PopBack_Empty()
        {
            _queue.PushBack(RandomGenerator.CreateRandomContent());
            _queue.PopBack();
            _queue.PopBack();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void PopBack_NotFound()
        {
            _queue.PopBack();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void PopBack_Incompatible()
        {
            QdbTestCluster.CreateBlob(_queue.Alias);
            _queue.PopBack();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbEmptyContainerException))]
        public void PopFront_Empty()
        {
            _queue.PushFront(RandomGenerator.CreateRandomContent());
            _queue.PopFront();
            _queue.PopFront();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void PopFront_NotFound()
        {
            _queue.PopFront();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void PopFront_Incompatible()
        {
            QdbTestCluster.CreateBlob(_queue.Alias);
            _queue.PopFront();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PushFront_Null()
        {
            _queue.PushFront(null);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void PushFront_Incompatible()
        {
            QdbTestCluster.CreateBlob(_queue.Alias);
            _queue.PushFront(RandomGenerator.CreateRandomContent());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PushBack_Null()
        {
            _queue.PushBack(null);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void PushBack_Incompatible()
        {
            QdbTestCluster.CreateBlob(_queue.Alias);
            _queue.PushBack(RandomGenerator.CreateRandomContent());
        }

        [TestMethod]
        public void PushBack_PopBack()
        {
            _queue.PushBack(_content1);
            _queue.PushBack(_content2);
            CollectionAssert.AreEqual(_content2, _queue.PopBack());
            CollectionAssert.AreEqual(_content1, _queue.PopBack());
        }

        [TestMethod]
        public void PushFront_PopBack()
        {
            _queue.PushFront(_content1);
            _queue.PushFront(_content2);
            CollectionAssert.AreEqual(_content1, _queue.PopBack());
            CollectionAssert.AreEqual(_content2, _queue.PopBack());
        }

        [TestMethod]
        public void PushFront_Back()
        {
            _queue.PushFront(_content1);
            _queue.PushFront(_content2);
            CollectionAssert.AreEqual(_content1, _queue.Back());
        }

        [TestMethod]
        public void PushFront_Front()
        {
            _queue.PushFront(_content1);
            _queue.PushFront(_content2);
            CollectionAssert.AreEqual(_content2, _queue.Front());
        }

        [TestMethod]
        public void PushFront_PopFront()
        {
            _queue.PushFront(_content1);
            _queue.PushFront(_content2);
            CollectionAssert.AreEqual(_content2, _queue.PopFront());
            CollectionAssert.AreEqual(_content1, _queue.PopFront());
        }
    }
}
