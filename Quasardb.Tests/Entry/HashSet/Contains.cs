using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;

namespace Quasardb.Tests.Entry.HashSet
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
        public void ThrowsIncompatibleType()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var content = RandomGenerator.CreateRandomContent();
            var hashSet = QdbTestCluster.CreateEmptyHashSet(alias);

            QdbTestCluster.CreateBlob(alias);
    
            try
            {
                hashSet.Contains(content);
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(hashSet.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var hashSet = QdbTestCluster.CreateEmptyHashSet();
            var content = RandomGenerator.CreateRandomContent();

            try
            {
                hashSet.Contains(content);
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
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
