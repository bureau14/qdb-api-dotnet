using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.Tests.Helpers;

namespace Quasardb.Tests.QdbBatchIntegerTests
{
    [TestClass]
    public class Put
    {
        private readonly QdbCluster _cluster = QdbTestCluster.Instance;

        [TestMethod]
        public void GivenNewAlias_CreatesTheInteger()
        {
            var batch = new QdbBatch();
            var alias = RandomGenerator.CreateUniqueAlias();
            var expiry = new DateTime(2200, 12, 25);

            batch.Integer(alias).Put(42, expiry);
            _cluster.RunBatch(batch);
            var actualValue = _cluster.Integer(alias).Get();
            var actualExpiry = _cluster.Integer(alias).GetExpiryTime();

            Assert.AreEqual(42, actualValue);
            Assert.AreEqual(expiry, actualExpiry);
        }

        [TestMethod]
        public void GivenExistingAlias_ReturnsAliasAlreadyExists()
        {
            var batch = new QdbBatch();
            var alias = RandomGenerator.CreateUniqueAlias();
            
            _cluster.Integer(alias).Put(42);
            var future = batch.Integer(alias).Put(42);
            _cluster.RunBatch(batch);

            Assert.IsInstanceOfType(future.Exception, typeof(QdbAliasAlreadyExistsException));
        }
    }
}