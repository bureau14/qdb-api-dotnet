using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;

namespace Quasardb.Tests.Entry.String
{
    [TestClass]
    public class GetExpiryTime
    {
        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var s = QdbTestCluster.CreateEmptyString();

            try
            {
                s.GetExpiryTime();
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(s.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ReturnsNonZeroValue_AfterPut()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content = RandomGenerator.CreateRandomString();
            var expiry = new DateTime(2200, 12, 25);

            s.Put(content, expiry);
            var result = s.GetExpiryTime();

            Assert.AreEqual(expiry, result);
        }

        [TestMethod]
        public void ReturnsUpdatedValue_AfterGetAndUpdate()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content = RandomGenerator.CreateRandomString();
            var expiry1 = new DateTime(2200, 12, 25);
            var expiry2 = new DateTime(2250, 12, 25);

            s.Put(content, expiry1);
            s.GetAndUpdate(content, expiry2);
            var result = s.GetExpiryTime();

            Assert.AreEqual(expiry2, result);
        }

        [TestMethod]
        public void ReturnsUpdatedValue_AfterUpdate()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content = RandomGenerator.CreateRandomString();
            var expiry1 = new DateTime(2200, 12, 25);
            var expiry2 = new DateTime(2250, 12, 25);

            s.Put(content, expiry1);
            s.Update(content, expiry2);
            var result = s.GetExpiryTime();

            Assert.AreEqual(expiry2, result);
        }

        [TestMethod]
        public void ReturnsUpdatedValue_AfterCompareAndSwap()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content1 = RandomGenerator.CreateRandomString();
            var content2 = RandomGenerator.CreateRandomString();
            var expiry1 = new DateTime(2200, 12, 25);
            var expiry2 = new DateTime(2250, 12, 25);

            s.Put(content1, expiry1);
            s.CompareAndSwap(content2, content1, expiry2);
            var result = s.GetExpiryTime();

            Assert.AreEqual(expiry2, result);
        }

        [TestMethod]
        public void ReturnsOriginalValue_AfterCompareAndSwap()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content1 = RandomGenerator.CreateRandomString();
            var content2 = RandomGenerator.CreateRandomString();
            var expiry1 = new DateTime(2200, 12, 25);
            var expiry2 = new DateTime(2250, 12, 25);

            s.Put(content1, expiry1);
            s.CompareAndSwap(content2, content2, expiry2);
            var result = s.GetExpiryTime();

            Assert.AreEqual(expiry1, result);
        }

        [TestMethod]
        public void ReturnsUpdatedValue_AfterExpiresAt()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content = RandomGenerator.CreateRandomString();
            var expiry = new DateTime(2200, 12, 25);

            s.Put(content);
            s.ExpiresAt(expiry);
            var result = s.GetExpiryTime();

            Assert.AreEqual(expiry, result);
        }

        [TestMethod]
        public void ReturnsUpdatedValue_AfterExpiresFromNow()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content = RandomGenerator.CreateRandomString();

            s.Put(content);
            s.ExpiresFromNow(TimeSpan.FromMinutes(10));
            var result = s.GetExpiryTime();

            Assert.IsTrue(result > DateTime.UtcNow.AddMinutes(5));
            Assert.IsTrue(result < DateTime.UtcNow.AddMinutes(15));
        }
    }
}
