using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbDequeTests
{
    [TestClass]
    public class GetAt
    {
        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void ThrowsAliasNotFound()
        {
            var deque = QdbTestCluster.CreateEmptyQueue();

            deque.GetAt(0); // <- throws QdbAliasNotFoundException
        }
        
        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void ThrowsIncompatibleType()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var deque = QdbTestCluster.CreateEmptyQueue(alias);

            QdbTestCluster.CreateBlob(alias);
            deque.GetAt(0); // <- throws QdbIncompatibleTypeException
        }
        
        [TestMethod]
        public void GivenPositiveIndex_ReturnsElementInOrder()
        {
            var deque = QdbTestCluster.CreateEmptyQueue();
            var content0 = RandomGenerator.CreateRandomContent();
            var content1 = RandomGenerator.CreateRandomContent();
            
            deque.PushBack(content0);
            deque.PushBack(content1);
            var result0 = deque[0];
            var result1 = deque[1];

            CollectionAssert.AreEqual(content0, result0);
            CollectionAssert.AreEqual(content1, result1);
        }

        [TestMethod]
        public void GivenNegativeIndex_ReturnsElementInOrder()
        {
            var deque = QdbTestCluster.CreateEmptyQueue();
            var content0 = RandomGenerator.CreateRandomContent();
            var content1 = RandomGenerator.CreateRandomContent();

            deque.PushBack(content0);
            deque.PushBack(content1);
            var result0 = deque[-1];
            var result1 = deque[-2];

            CollectionAssert.AreEqual(content1, result0);
            CollectionAssert.AreEqual(content0, result1);
        }
    }
}
