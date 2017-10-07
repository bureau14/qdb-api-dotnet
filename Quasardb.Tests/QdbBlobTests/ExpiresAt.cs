using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.Tests.Helpers;

namespace Quasardb.Tests.QdbBlobTests
{
    [TestClass]
    public class ExpiresAt
    {
        [TestMethod]
        public void ThrowsAliasNotFound_OnNewAlias()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var expiry = new DateTime(3000, 12, 25);

            try
            {
                blob.ExpiresAt(expiry);
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(blob.Alias, e.Alias);
            }
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
