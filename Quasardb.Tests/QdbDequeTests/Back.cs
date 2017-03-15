using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.Tests.Helpers;

namespace Quasardb.Tests.QdbDequeTests
{
    [TestClass]
    public class Back
    {
        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var deque = QdbTestCluster.CreateEmptyQueue();
            
            try
            {
                deque.Back();
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
                deque.Back();
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

            Assert.IsNull(deque.Back());
        }

        [TestMethod]
        public void ReturnsFirstElement_AfterPushFront()
        {
            var deque = QdbTestCluster.CreateEmptyQueue();
            var content1 = RandomGenerator.CreateRandomContent();
            var content2 = RandomGenerator.CreateRandomContent();

            deque.PushFront(content1);
            deque.PushFront(content2);

            CollectionAssert.AreEqual(content1, deque.Back());
        }

        [TestMethod]
        public void ReturnsLastElement_AfterPushBack()
        {
            var deque = QdbTestCluster.CreateEmptyQueue();
            var content1 = RandomGenerator.CreateRandomContent();
            var content2 = RandomGenerator.CreateRandomContent();

            deque.PushBack(content1);
            deque.PushBack(content2);

            CollectionAssert.AreEqual(content2, deque.Back());
        }
    }
}
