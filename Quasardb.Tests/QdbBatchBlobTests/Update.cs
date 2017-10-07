using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Tests.Helpers;

namespace Quasardb.Tests.QdbBatchBlobTests
{
    [TestClass]
    public class Update
    {
        private readonly QdbCluster _cluster = QdbTestCluster.Instance;

        [TestMethod]
        public void GivenExistingAlias_UpdateContent()
        {
            var batch = new QdbBatch();
            var alias = RandomGenerator.CreateUniqueAlias();
            var garbage = RandomGenerator.CreateRandomContent();
            var content = RandomGenerator.CreateRandomContent();
            var expiry = new DateTime(2200, 12, 25);

            _cluster.Blob(alias).Put(garbage);
            batch.Blob(alias).Update(content, expiry);
            _cluster.RunBatch(batch);
            var actualContent = _cluster.Blob(alias).Get();
            var actualExpiry = _cluster.Blob(alias).GetExpiryTime();

            Assert.AreEqual(content.Length, actualContent.Length);
            CollectionAssert.AreEqual(content, actualContent);
            Assert.AreEqual(expiry, actualExpiry);
        }
    }
}