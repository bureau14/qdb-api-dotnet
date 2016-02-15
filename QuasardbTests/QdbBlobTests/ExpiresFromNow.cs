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
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void ThrowsAliasNotFound_OnNewAlias()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var delay = TimeSpan.FromMinutes(42);

            blob.ExpiresFromNow(delay);
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
