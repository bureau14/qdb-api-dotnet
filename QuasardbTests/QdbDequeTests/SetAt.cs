using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbDequeTests
{
    [TestClass]
    public class SetAt
    {

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNull()
        {
            var deque = QdbTestCluster.CreateEmptyQueue();

            deque[0] = null; // <- throws ArgumentNullException
        }

        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var deque = QdbTestCluster.CreateEmptyQueue();
            var content = RandomGenerator.CreateRandomContent();

            try
            {
                deque[0] = content;
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
            var content = RandomGenerator.CreateRandomContent();

            QdbTestCluster.CreateBlob(alias);

            try
            {
                deque.SetAt(0, content);
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(deque.Alias, e.Alias);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(QdbOutOfBoundsException))]
        public void ThrowsOutOfRange()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var deque = QdbTestCluster.CreateEmptyQueue(alias);
            var content = RandomGenerator.CreateRandomContent();

            deque.PushBack(content);
            deque.SetAt(1, content); // <- throws QdbOutOfBoundsException
        }

        [TestMethod]
        public void ReplacesExistingElement()
        {
            var deque = QdbTestCluster.CreateEmptyQueue();
            var originalContent = RandomGenerator.CreateRandomContent();
            var newContent = RandomGenerator.CreateRandomContent();

            deque.PushFront(originalContent);
            deque[0] = newContent;
            var actualContent = deque.PopFront();

            CollectionAssert.AreEqual(newContent, actualContent);
        }
    }
}
