using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbDequeTests
{
    [TestClass]
    public class PopBack
    {
        [TestMethod]
        [ExpectedException(typeof(QdbEmptyContainerException))]
        public void ThrowsEmptyContainer()
        {
            var deque = QdbTestCluster.CreateEmptyQueue();
            deque.PushBack(RandomGenerator.CreateRandomContent());
            deque.PopBack();

            deque.PopBack();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void ThrowsAliasNotFound()
        {
            var deque = QdbTestCluster.CreateEmptyQueue();

            deque.PopBack();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void ThrowsIncompatibleType()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            QdbTestCluster.CreateBlob(alias);

            var deque = QdbTestCluster.CreateEmptyQueue(alias);
            deque.PopBack();
        }

        [TestMethod]
        public void ReturnsElements_InReverseOrder_AfterPushBack()
        {
            var deque = QdbTestCluster.CreateEmptyQueue();
            var content1 = RandomGenerator.CreateRandomContent();
            var content2 = RandomGenerator.CreateRandomContent();

            deque.PushBack(content1);
            deque.PushBack(content2);

            CollectionAssert.AreEqual(content2, deque.PopBack());
            CollectionAssert.AreEqual(content1, deque.PopBack());
        }

        [TestMethod]
        public void ReturnsElements_InOrder_AfterPushFront()
        {
            var deque = QdbTestCluster.CreateEmptyQueue();
            var content1 = RandomGenerator.CreateRandomContent();
            var content2 = RandomGenerator.CreateRandomContent();

            deque.PushFront(content1);
            deque.PushFront(content2);

            CollectionAssert.AreEqual(content1, deque.PopBack());
            CollectionAssert.AreEqual(content2, deque.PopBack());
        }
    }
}
