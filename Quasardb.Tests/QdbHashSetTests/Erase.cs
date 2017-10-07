using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.Tests.Helpers;

namespace Quasardb.Tests.QdbHashSetTests
{
    [TestClass]
    public class Erase
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNull()
        {
            var hashSet = QdbTestCluster.CreateEmptyHashSet();
            hashSet.Erase(null); // <- throws ArgumentNullException
        }

        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var hashSet = QdbTestCluster.CreateEmptyHashSet();
            var content = RandomGenerator.CreateRandomContent();

            try
            {
                hashSet.Erase(content);
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(hashSet.Alias, e.Alias);
            }
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
                hashSet.Erase(content);
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(hashSet.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ReturnsTrue_AfterInsert()
        {
            var hashSet = QdbTestCluster.CreateEmptyHashSet();
            var content = RandomGenerator.CreateRandomContent();

            hashSet.Insert(content);
            Assert.IsTrue(hashSet.Erase(content));
        }

        [TestMethod]
        public void ReturnsFalse_AfterErase()
        {
            var hashSet = QdbTestCluster.CreateEmptyHashSet();
            var content = RandomGenerator.CreateRandomContent();

            hashSet.Insert(content);
            hashSet.Erase(content);
            Assert.IsFalse(hashSet.Erase(content));
        }
    }
}
