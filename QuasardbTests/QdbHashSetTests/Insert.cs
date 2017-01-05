using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbHashSetTests
{
    [TestClass]
    public class Insert
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNull()
        {
            var hashSet = QdbTestCluster.CreateEmptyHashSet();

            hashSet.Insert(null); // <- throws ArgumentNullException
        }

        [TestMethod]
        public void ThrowsIncompatibleType()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var hashSet = QdbTestCluster.CreateEmptyHashSet(alias);
            var content = RandomGenerator.CreateRandomContent();

            QdbTestCluster.CreateBlob(alias);

            try
            {
                hashSet.Insert(content);
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(hashSet.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ReturnTrue_WhenEmpty()
        {
            var hashSet = QdbTestCluster.CreateEmptyHashSet();
            var content = RandomGenerator.CreateRandomContent();

            Assert.IsTrue(hashSet.Insert(content));
        }

        [TestMethod]
        public void ReturnsFalse_AfterInsert()
        {
            var hashSet = QdbTestCluster.CreateEmptyHashSet();
            var content = RandomGenerator.CreateRandomContent();

            hashSet.Insert(content);
            Assert.IsFalse(hashSet.Insert(content));
        }

        [TestMethod]
        public void ReturnsFalse_AfterErase()
        {
            var hashSet = QdbTestCluster.CreateEmptyHashSet();
            var content = RandomGenerator.CreateRandomContent();

            hashSet.Insert(content);
            hashSet.Erase(content);
            Assert.IsTrue(hashSet.Insert(content));
        }
    }
}
