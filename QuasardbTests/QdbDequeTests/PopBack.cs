using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbDequeTests
{
    [TestClass]
    public class PopBack
    {
        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var deque = QdbTestCluster.CreateEmptyQueue();
            
            try
            {
                deque.PopBack();
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(deque.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ThrowsIncompatibleType()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            QdbTestCluster.CreateBlob(alias);

            var deque = QdbTestCluster.CreateEmptyQueue(alias);
            
            try
            {
                deque.PopBack();
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(deque.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ReturnsNull_WhenEmpty()
        {
            var deque = QdbTestCluster.CreateEmptyQueue();
            deque.PushBack(RandomGenerator.CreateRandomContent());
            deque.PopBack();

            Assert.IsNull(deque.PopBack());
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
