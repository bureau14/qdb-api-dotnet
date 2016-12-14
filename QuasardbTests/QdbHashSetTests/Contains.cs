using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbHashSetTests
{
    [TestClass]
    public class Contains
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNull()
        {
           var hashSet = QdbTestCluster.CreateEmptyHashSet();

           hashSet.Contains(null); // <- throws ArgumentNullException 
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void ThrowsIncompatibleType()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var content = RandomGenerator.CreateRandomContent();
            var hashSet = QdbTestCluster.CreateEmptyHashSet(alias);

            QdbTestCluster.CreateBlob(alias);
            hashSet.Contains(content); // <- throws QdbIncompatibleTypeException
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void ThrowsAliasNotFound()
        {
            var hashSet = QdbTestCluster.CreateEmptyHashSet();
            var content = RandomGenerator.CreateRandomContent();

            hashSet.Contains(content); // <- throws QdbAliasNotFoundException
        }


        [TestMethod]
        public void ReturnsTrue_AfterInsert()
        {
            var hashSet = QdbTestCluster.CreateEmptyHashSet();
            var content = RandomGenerator.CreateRandomContent();

            hashSet.Insert(content);
            Assert.IsTrue(hashSet.Contains(content));
        }

        [TestMethod]
        public void ReturnsFalse_AfterErase()
        {
            var hashSet = QdbTestCluster.CreateEmptyHashSet();
            var content = RandomGenerator.CreateRandomContent();

            hashSet.Insert(content);
            hashSet.Erase(content);
            Assert.IsFalse(hashSet.Contains(content));
        }
    }
}
