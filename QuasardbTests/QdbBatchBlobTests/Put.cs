using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbBatchBlobTests
{
    [TestClass]
    public class Put
    {
        private readonly QdbCluster _cluster = QdbTestCluster.Instance;

        [TestMethod]
        public void GivenNewAlias_CreatesTheBlob()
        {
            var batch = new QdbBatch();
            var alias = RandomGenerator.CreateUniqueAlias();
            var content = RandomGenerator.CreateRandomContent();
            var expiry = new DateTime(2200, 12, 25);

            batch.Blob(alias).Put(content, expiry);
            _cluster.RunBatch(batch);
            var actualContent = _cluster.Blob(alias).Get();
            var actualExpiry = _cluster.Blob(alias).GetExpiryTime();

            Assert.AreEqual(content.Length, actualContent.Length);
            CollectionAssert.AreEqual(content, actualContent);
            Assert.AreEqual(expiry, actualExpiry);
        }

        [TestMethod]
        public void GivenExistingAlias_ReturnsAliasAlreadyExists()
        {
            var batch = new QdbBatch();
            var alias = RandomGenerator.CreateUniqueAlias();
            var content = RandomGenerator.CreateRandomContent();

            _cluster.Blob(alias).Put(content);
            var future = batch.Blob(alias).Put(content);
            _cluster.RunBatch(batch);

            Assert.IsInstanceOfType(future.Exception, typeof(QdbAliasAlreadyExistsException));
        }
    }
}