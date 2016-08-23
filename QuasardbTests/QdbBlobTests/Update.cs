using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbBlobTests
{
    [TestClass]
    public class Update
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNull()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();

            blob.Update(null);
        }

        [TestMethod]
        public void ReturnsTrue_WithNewAlias()
        {
            var content = RandomGenerator.CreateRandomContent();
            var blob = QdbTestCluster.CreateEmptyBlob();

            var created = blob.Update(content);

            Assert.IsTrue(created);
        }

        [TestMethod]
        public void ReturnsFalse_WhenCalledTwice()
        {
            var content = RandomGenerator.CreateRandomContent();
            var blob = QdbTestCluster.CreateEmptyBlob();

            blob.Update(content);
            var created = blob.Update(content);

            Assert.IsFalse(created);
        }
    }
}
