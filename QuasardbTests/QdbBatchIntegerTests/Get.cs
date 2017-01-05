using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbBatchIntegerTests
{
    [TestClass]
    public class Get
    {
        private readonly QdbCluster _cluster = QdbTestCluster.Instance;

        [TestMethod]
        public void ReturnsValue()
        {
            var batch = new QdbBatch();
            var alias = RandomGenerator.CreateUniqueAlias();

            _cluster.Integer(alias).Put(42);
            var future = batch.Integer(alias).Get();
            _cluster.RunBatch(batch);

            Assert.AreEqual(42, future.Result);
        }

        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var batch = new QdbBatch();
            var alias = RandomGenerator.CreateUniqueAlias();

            var future = batch.Integer(alias).Get();
            _cluster.RunBatch(batch);

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
    }
}