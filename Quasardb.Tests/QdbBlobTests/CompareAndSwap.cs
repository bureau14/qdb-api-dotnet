using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Tests.Helpers;

namespace Quasardb.Tests.QdbBlobTests
{
    [TestClass]
    public class CompareAndSwap
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNull_WhenNewContentIsNull()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content = RandomGenerator.CreateRandomContent();

            blob.CompareAndSwap(null, content);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNull_WhenNewComparandIsNull()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content = RandomGenerator.CreateRandomContent();

            blob.CompareAndSwap(content, null);
        }

        [TestMethod]
        public void ReturnsOriginalContent_WhenComparandMismatches()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content1 = RandomGenerator.CreateRandomContent();
            var content2 = RandomGenerator.CreateRandomContent();

            blob.Put(content1);
            var result = blob.CompareAndSwap(content2, content2);

            CollectionAssert.AreEqual(content1, result);
        }

        [TestMethod]
        public void ReturnsNull_WhenComparandMatches()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content1 = RandomGenerator.CreateRandomContent();
            var content2 = RandomGenerator.CreateRandomContent();

            blob.Put(content1);
            var result = blob.CompareAndSwap(content2, content1);

            Assert.IsNull(result);
        }
    }
}
