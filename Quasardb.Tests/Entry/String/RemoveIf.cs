using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;

namespace Quasardb.Tests.Entry.String
{
    [TestClass]
    public class RemoveIf
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNull()
        {
            var s = QdbTestCluster.CreateEmptyString();

            s.RemoveIf(null);
        }

        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content = RandomGenerator.CreateRandomString();

            try
            {
                s.RemoveIf(content);
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(s.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ReturnsTrue_WhenComparandMatches()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content = RandomGenerator.CreateRandomString();

            s.Put(content);
            var result = s.RemoveIf(content);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ReturnsFalse_WhenComparandMismatches()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content1 = RandomGenerator.CreateRandomString();
            var content2 = RandomGenerator.CreateRandomString();

            s.Put(content1);
            var result = s.RemoveIf(content2);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ThrowsAliasNotFound_WhenCalledTwice()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content = RandomGenerator.CreateRandomString();

            s.Put(content);
            s.RemoveIf(content);

            try
            {
                s.RemoveIf(content);
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(s.Alias, e.Alias);
            }
        }
    }
}
