using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests
{
    [TestClass]
    public class QdbDequeTests
    {
        QdbDeque _deque;
        byte[] _content1, _content2;

        [TestInitialize]
        public void Initialize()
        {
            _deque = QdbTestCluster.CreateEmptyQueue();
            _content1 = RandomGenerator.CreateRandomContent();
            _content2 = RandomGenerator.CreateRandomContent();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbEmptyContainerException))]
        public void Back_Empty()
        {
            _deque.PushBack(RandomGenerator.CreateRandomContent());
            _deque.PopBack();
            _deque.Back();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void Back_NotFound()
        {
            _deque.Back();          
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void Back_Incompatible()
        {
            QdbTestCluster.CreateBlob(_deque.Alias);
            _deque.Back();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbEmptyContainerException))]
        public void Front_Empty()
        {
            _deque.PushFront(RandomGenerator.CreateRandomContent());
            _deque.PopFront();
            _deque.Front();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void Front_NotFound()
        {
            _deque.Front();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void Front_Incompatible()
        {
            QdbTestCluster.CreateBlob(_deque.Alias);
            _deque.Front();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbEmptyContainerException))]
        public void PopBack_Empty()
        {
            _deque.PushBack(RandomGenerator.CreateRandomContent());
            _deque.PopBack();
            _deque.PopBack();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void PopBack_NotFound()
        {
            _deque.PopBack();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void PopBack_Incompatible()
        {
            QdbTestCluster.CreateBlob(_deque.Alias);
            _deque.PopBack();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbEmptyContainerException))]
        public void PopFront_Empty()
        {
            _deque.PushFront(RandomGenerator.CreateRandomContent());
            _deque.PopFront();
            _deque.PopFront();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void PopFront_NotFound()
        {
            _deque.PopFront();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void PopFront_Incompatible()
        {
            QdbTestCluster.CreateBlob(_deque.Alias);
            _deque.PopFront();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PushFront_Null()
        {
            _deque.PushFront(null);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void PushFront_Incompatible()
        {
            QdbTestCluster.CreateBlob(_deque.Alias);
            _deque.PushFront(RandomGenerator.CreateRandomContent());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PushBack_Null()
        {
            _deque.PushBack(null);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void PushBack_Incompatible()
        {
            QdbTestCluster.CreateBlob(_deque.Alias);
            _deque.PushBack(RandomGenerator.CreateRandomContent());
        }

        [TestMethod]
        public void PushBack_PopBack()
        {
            _deque.PushBack(_content1);
            _deque.PushBack(_content2);
            CollectionAssert.AreEqual(_content2, _deque.PopBack());
            CollectionAssert.AreEqual(_content1, _deque.PopBack());
        }

        [TestMethod]
        public void PushFront_PopBack()
        {
            _deque.PushFront(_content1);
            _deque.PushFront(_content2);
            CollectionAssert.AreEqual(_content1, _deque.PopBack());
            CollectionAssert.AreEqual(_content2, _deque.PopBack());
        }

        [TestMethod]
        public void PushFront_Back()
        {
            _deque.PushFront(_content1);
            _deque.PushFront(_content2);
            CollectionAssert.AreEqual(_content1, _deque.Back());
        }

        [TestMethod]
        public void PushFront_Front()
        {
            _deque.PushFront(_content1);
            _deque.PushFront(_content2);
            CollectionAssert.AreEqual(_content2, _deque.Front());
        }

        [TestMethod]
        public void PushFront_PopFront()
        {
            _deque.PushFront(_content1);
            _deque.PushFront(_content2);
            CollectionAssert.AreEqual(_content2, _deque.PopFront());
            CollectionAssert.AreEqual(_content1, _deque.PopFront());
        }
    }
}
