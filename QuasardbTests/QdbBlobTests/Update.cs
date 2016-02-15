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
        public void NoError_WithNewAlias()
        {
            var content = RandomGenerator.CreateRandomContent();
            var blob = QdbTestCluster.CreateEmptyBlob();

            blob.Update(content);
        }

        [TestMethod]
        public void NoError_WhenCalledTwice()
        {
            var content = RandomGenerator.CreateRandomContent();
            var blob = QdbTestCluster.CreateEmptyBlob();

            blob.Update(content);
            blob.Update(content);
        }
    }
}
