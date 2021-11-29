using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quasardb.Tests.Batch.Blob
{
    [TestClass]
    public class CompareAndSwap
    {
        private readonly QdbCluster _cluster = QdbTestCluster.Instance;

        [TestMethod]
        public void GivenMatchingComparandAlias_UpdatesContent()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Assert.Inconclusive("Skip on linux");
            }
            var batch = new QdbBatch();
            var alias = RandomGenerator.CreateUniqueAlias();
            var initialContent = RandomGenerator.CreateRandomContent();
            var newContent = RandomGenerator.CreateRandomContent();
            var expiry = new DateTime(2200, 12, 25);

            _cluster.Blob(alias).Put(initialContent);
            var future = batch.Blob(alias).CompareAndSwap(newContent, initialContent, expiry);
            _cluster.RunBatch(batch);
            var actualContent = _cluster.Blob(alias).Get();
            var actualExpiry = _cluster.Blob(alias).GetExpiryTime();
            CollectionAssert.AreEqual(newContent, actualContent);
            Assert.AreEqual(expiry, actualExpiry);
            Assert.IsNull(future.Result);
        }

        [TestMethod]
        public void GivenDifferentComparandAlias_ReturnsOriginal()
        {
            var batch = new QdbBatch();
            var alias = RandomGenerator.CreateUniqueAlias();
            var initialContent = RandomGenerator.CreateRandomContent();
            var garbage = RandomGenerator.CreateRandomContent();
            var newContent = RandomGenerator.CreateRandomContent();
            var expiry = new DateTime(2200, 12, 25);

            _cluster.Blob(alias).Put(initialContent);
            var future = batch.Blob(alias).CompareAndSwap(newContent, garbage, expiry);
            _cluster.RunBatch(batch);
            var actualContent = _cluster.Blob(alias).Get();
            var actualExpiry = _cluster.Blob(alias).GetExpiryTime();

            CollectionAssert.AreEqual(initialContent, actualContent);
            Assert.IsNull(actualExpiry);
            CollectionAssert.AreEqual(initialContent, future.Result);
        }
    }
}