using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbHashSetTests
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
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void ThrowsAliasNotFound()
        {
            var hashSet = QdbTestCluster.CreateEmptyHashSet();
            var content = RandomGenerator.CreateRandomContent();

            hashSet.Erase(content); // <- QdbAliasNotFoundException
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void ThrowsIncompatibleType()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var hashSet = QdbTestCluster.CreateEmptyHashSet(alias);
            var content = RandomGenerator.CreateRandomContent();

            QdbTestCluster.CreateBlob(alias);
            hashSet.Erase(content); // <- QdbIncompatibleTypeException
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
