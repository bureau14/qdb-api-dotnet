using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbBlobTests
{
    [TestClass]
    public class ExpiresFromNow
    {
        [TestMethod]
        public void ThrowsAliasNotFound_OnNewAlias()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var delay = TimeSpan.FromMinutes(42);

            try
            {
                blob.ExpiresFromNow(delay);
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
            var delay = TimeSpan.FromMinutes(42);

            blob.Put(content);
            blob.ExpiresFromNow(delay);
        }
    }
}
