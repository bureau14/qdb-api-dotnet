using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbBlobTests
{
    [TestClass]
    public class RemoveIf
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNull()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();

            blob.RemoveIf(null);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void ThrowsAliasNotFound()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content = RandomGenerator.CreateRandomContent();

            blob.RemoveIf(content);
        }

        [TestMethod]
        public void ReturnsTrue_WhenComparandMatches()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content = RandomGenerator.CreateRandomContent();

            blob.Put(content);
            var result = blob.RemoveIf(content);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ReturnsFalse_WhenComparandMismatches()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content1 = RandomGenerator.CreateRandomContent();
            var content2 = RandomGenerator.CreateRandomContent();

            blob.Put(content1);
            var result = blob.RemoveIf(content2);

            Assert.IsFalse(result);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void ThrowsAliasNotFound_WhenCalledTwice()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content = RandomGenerator.CreateRandomContent();

            blob.Put(content);
            blob.RemoveIf(content);
            blob.RemoveIf(content);
        }
    }
}
