using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbBatchIntegerTests
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
            var expiry = new DateTime(2200, 12, 25);

            _cluster.Integer(alias).Put(666);
            batch.Integer(alias).Update(42, expiry);
            _cluster.RunBatch(batch);
            var actualValue = _cluster.Integer(alias).Get();
            var actualExpiry = _cluster.Integer(alias).GetExpiryTime();

            Assert.AreEqual(42, actualValue);
            Assert.AreEqual(expiry, actualExpiry);
        }

        [TestMethod]
        public void ReturnsIncompatibleType()
        {
            var batch = new QdbBatch();
            var alias = RandomGenerator.CreateUniqueAlias();
            var garbage = RandomGenerator.CreateRandomContent();

            _cluster.Blob(alias).Put(garbage);
            var future = batch.Integer(alias).Update(42);
            _cluster.RunBatch(batch);

            Assert.IsInstanceOfType(future.Exception, typeof(QdbIncompatibleTypeException));
        }
    }
}