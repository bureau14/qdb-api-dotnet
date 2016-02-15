using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbBlobTests
{
    [TestClass]
    public class GetAndRemove
    {
        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void ThrowsAliasNotFound()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();

            blob.GetAndRemove();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void ThrowsAliasNotFound_WhenCalledTwice()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();

            blob.GetAndRemove();
            blob.GetAndRemove();
        }
    }
}
