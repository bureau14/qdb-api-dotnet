using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbBlobTests
{
    [TestClass]
    public class Remove
    {
        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void ThrowsAliasNotFound_OnNewAlias()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();

            blob.Remove();
        }

        [TestMethod]
        public void NoError_AfterPut()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content = RandomGenerator.CreateRandomContent();

            blob.Put(content);
            blob.Remove();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void ThrowsAliasNotFound_WhenCalledTwice()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content = RandomGenerator.CreateRandomContent();

            blob.Put(content);
            blob.Remove();
            blob.Remove();
        }
    }
}
