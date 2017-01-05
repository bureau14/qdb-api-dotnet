using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbBlobTests
{
    [TestClass]
    public class GetExpiryTime
    {
        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            
            try
            {
                blob.GetExpiryTime();
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(blob.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ReturnsNonZeroValue_AfterPut()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content = RandomGenerator.CreateRandomContent();
            var expiry = new DateTime(2200, 12, 25);

            blob.Put(content, expiry);
            var result = blob.GetExpiryTime();

            Assert.AreEqual(expiry, result);
        }

        [TestMethod]
        public void ReturnsUpdatedValue_AfterGetAndUpdate()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content = RandomGenerator.CreateRandomContent();
            var expiry1 = new DateTime(2200, 12, 25);
            var expiry2 = new DateTime(2250, 12, 25);

            blob.Put(content, expiry1);
            blob.GetAndUpdate(content, expiry2);
            var result = blob.GetExpiryTime();

            Assert.AreEqual(expiry2, result);
        }

        [TestMethod]
        public void ReturnsUpdatedValue_AfterUpdate()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content = RandomGenerator.CreateRandomContent();
            var expiry1 = new DateTime(2200, 12, 25);
            var expiry2 = new DateTime(2250, 12, 25);

            blob.Put(content, expiry1);
            blob.Update(content, expiry2);
            var result = blob.GetExpiryTime();

            Assert.AreEqual(expiry2, result);
        }

        [TestMethod]
        public void ReturnsUpdatedValue_AfterCompareAndSwap()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content1 = RandomGenerator.CreateRandomContent();
            var content2 = RandomGenerator.CreateRandomContent();
            var expiry1 = new DateTime(2200, 12, 25);
            var expiry2 = new DateTime(2250, 12, 25);

            blob.Put(content1, expiry1);
            blob.CompareAndSwap(content2, content1, expiry2);
            var result = blob.GetExpiryTime();

            Assert.AreEqual(expiry2, result);
        }

        [TestMethod]
        public void ReturnsOriginalValue_AfterCompareAndSwap()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content1 = RandomGenerator.CreateRandomContent();
            var content2 = RandomGenerator.CreateRandomContent();
            var expiry1 = new DateTime(2200, 12, 25);
            var expiry2 = new DateTime(2250, 12, 25);

            blob.Put(content1, expiry1);
            blob.CompareAndSwap(content2, content2, expiry2);
            var result = blob.GetExpiryTime();

            Assert.AreEqual(expiry1, result);
        }

        [TestMethod]
        public void ReturnsUpdatedValue_AfterExpiresAt()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content = RandomGenerator.CreateRandomContent();
            var expiry = new DateTime(2200, 12, 25);

            blob.Put(content);
            blob.ExpiresAt(expiry);
            var result = blob.GetExpiryTime();

            Assert.AreEqual(expiry, result);
        }

        [TestMethod]
        public void ReturnsUpdatedValue_AfterExpiresFromNow()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content = RandomGenerator.CreateRandomContent();

            blob.Put(content);
            blob.ExpiresFromNow(TimeSpan.FromMinutes(10));
            var result = blob.GetExpiryTime();

            Assert.IsTrue(result > DateTime.UtcNow.AddMinutes(5));
            Assert.IsTrue(result < DateTime.UtcNow.AddMinutes(15));
        }
    }
}
