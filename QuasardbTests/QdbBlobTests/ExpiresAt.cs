using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbBlobTests
{
    [TestClass]
    public class ExpiresAt
    {
        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void ThrowsAliasNotFound_OnNewAlias()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var expiry = new DateTime(3000, 12, 25);

            blob.ExpiresAt(expiry);
        }

        [TestMethod]
        public void NoError_AfterPut()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content = RandomGenerator.CreateRandomContent();
            var expiry = new DateTime(3000, 12, 25);

            blob.Put(content);
            blob.ExpiresAt(expiry);
        }
    }
}
