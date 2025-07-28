using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quasardb.Tests.Entry.String
{
    [TestClass]
    public class CompareAndSwap
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNull_WhenNewContentIsNull()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content = RandomGenerator.CreateRandomString();

            s.CompareAndSwap(null, content);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNull_WhenNewComparandIsNull()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content = RandomGenerator.CreateRandomString();

            s.CompareAndSwap(content, null);
        }

        [TestMethod]
        public void ReturnsOriginalContent_WhenComparandMismatches()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content1 = RandomGenerator.CreateRandomString();
            var content2 = RandomGenerator.CreateRandomString();

            s.Put(content1);
            var result = s.CompareAndSwap(content2, content2);

            Assert.AreEqual(content1, result);
        }

        [TestMethod]
        public void ReturnsNull_WhenComparandMatches()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content1 = RandomGenerator.CreateRandomString();
            var content2 = RandomGenerator.CreateRandomString();

            s.Put(content1);
            var result = s.CompareAndSwap(content2, content1);

            Assert.IsNull(result);
        }
    }
}
