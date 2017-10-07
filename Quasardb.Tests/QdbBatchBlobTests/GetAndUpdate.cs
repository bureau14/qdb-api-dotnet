using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.Tests.Helpers;

namespace Quasardb.Tests.QdbBatchBlobTests
{
    [TestClass]
    public class GetAndUpdate
    {
        private readonly QdbCluster _cluster = QdbTestCluster.Instance;

        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var batch = new QdbBatch();
            var alias = RandomGenerator.CreateUniqueAlias();
            var newContent = RandomGenerator.CreateRandomContent();

            var future = batch.Blob(alias).GetAndUpdate(newContent);
            _cluster.RunBatch(batch);

            Assert.IsInstanceOfType(future.Exception, typeof(QdbAliasNotFoundException));

            try
            {
                var dummy = future.Result;
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(alias, e.Alias);
            }
        }

        [TestMethod]
        public void ReturnsOriginalContent_ReplacesWithNewContents()
        {
            var batch = new QdbBatch();
            var alias = RandomGenerator.CreateUniqueAlias();
            var newContent = RandomGenerator.CreateRandomContent();
            var originalContent = RandomGenerator.CreateRandomContent();
            var expiry = new DateTime(2200, 12, 25);

            _cluster.Blob(alias).Put(originalContent);
            var future = batch.Blob(alias).GetAndUpdate(newContent, expiry);
            _cluster.RunBatch(batch);
            var actualContent = _cluster.Blob(alias).Get();
            var actualExpiry = _cluster.Blob(alias).GetExpiryTime();

            CollectionAssert.AreEqual(newContent, actualContent);
            CollectionAssert.AreEqual(originalContent, future.Result);
            Assert.AreEqual(expiry, actualExpiry);
        }
    }
}
