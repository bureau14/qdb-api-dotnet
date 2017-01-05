using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbDequeTests
{
    [TestClass]
    public class PushFront
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNull()
        {
            var deque = QdbTestCluster.CreateEmptyQueue();

            deque.PushFront(null);
        }

        [TestMethod]
        public void ThrowsIncompatibleType()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            QdbTestCluster.CreateBlob(alias);

            var deque = QdbTestCluster.CreateEmptyQueue(alias);

            try
            {
                deque.PushFront(RandomGenerator.CreateRandomContent());
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(deque.Alias, e.Alias);
            }
        }
    }
}
