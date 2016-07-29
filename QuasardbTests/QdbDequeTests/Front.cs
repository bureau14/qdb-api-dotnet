using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbDequeTests
{
    [TestClass]
    public class Front
    {
        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void ThrowsAliasNotFound()
        {
            var deque = QdbTestCluster.CreateEmptyQueue();

            deque.Front();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void ThrowsIncompatibleType()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            QdbTestCluster.CreateBlob(alias);

            var deque = QdbTestCluster.CreateEmptyQueue(alias);
            deque.Front();
        }

        [TestMethod]
        public void ReturnsNull_WhenEmpty()
        {
            var deque = QdbTestCluster.CreateEmptyQueue();
            deque.PushFront(RandomGenerator.CreateRandomContent());
            deque.PopFront();

            Assert.IsNull(deque.Front());
        }

        [TestMethod]
        public void ReturnsLastElement_AfterPushFront()
        {
            var deque = QdbTestCluster.CreateEmptyQueue();
            var content1 = RandomGenerator.CreateRandomContent();
            var content2 = RandomGenerator.CreateRandomContent();

            deque.PushFront(content1);
            deque.PushFront(content2);

            CollectionAssert.AreEqual(content2, deque.Front());
        }

        [TestMethod]
        public void ReturnsFirstElement_AfterPushBack()
        {
            var deque = QdbTestCluster.CreateEmptyQueue();
            var content1 = RandomGenerator.CreateRandomContent();
            var content2 = RandomGenerator.CreateRandomContent();

            deque.PushBack(content1);
            deque.PushBack(content2);

            CollectionAssert.AreEqual(content1, deque.Front());
        }
    }
}
