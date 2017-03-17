using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;

namespace Quasardb.Tests.Entry.Deque
{
    [TestClass]
    public class Size
    {
        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var deque = QdbTestCluster.CreateEmptyQueue();
            
            try
            {
                deque.Size();
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
            var deque = QdbTestCluster.CreateEmptyQueue(alias);

            QdbTestCluster.CreateBlob(alias);

            try
            {
                deque.Size();
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(deque.Alias, e.Alias);
            }
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
