using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbBlobTests
{
    [TestClass]
    public class Remove
    {
        [TestMethod]
        public void ReturnsFalse_WhenAliasDoesntExist()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();

            var result = blob.Remove();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ReturnsTrue_WhenAliasExists()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content = RandomGenerator.CreateRandomContent();

            blob.Put(content);
            var result = blob.Remove();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ReturnsFalse_WhenCalledTwice()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content = RandomGenerator.CreateRandomContent();

            blob.Put(content);
            blob.Remove();
            var result = blob.Remove();

            Assert.IsFalse(result);
        }
    }
}
