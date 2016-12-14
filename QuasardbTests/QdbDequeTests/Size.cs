using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbDequeTests
{
    [TestClass]
    public class Size
    {
        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void ThrowsAliasNotFound()
        {
            var deque = QdbTestCluster.CreateEmptyQueue();

            deque.Size(); // <- throws QdbAliasNotFoundException
        }
        
        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void ThrowsIncompatibleType()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var deque = QdbTestCluster.CreateEmptyQueue(alias);

            QdbTestCluster.CreateBlob(alias);
            deque.Size(); // <- throws QdbIncompatibleTypeException
        }
        
        [TestMethod]
        public void ReturnsOne_AfterPushBack()
        {
            var deque = QdbTestCluster.CreateEmptyQueue();
            var content = RandomGenerator.CreateRandomContent();
            
            deque.PushBack(content);
            Assert.AreEqual(1, deque.Size());
        }
    }
}
