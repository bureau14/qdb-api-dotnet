using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbBatchIntegerTests
{
    [TestClass]
    public class Add
    {
        private readonly QdbCluster _cluster = QdbTestCluster.Instance;

        [TestMethod]
        public void ReturnsSum()
        {
            var batch = new QdbBatch();
            var alias = RandomGenerator.CreateUniqueAlias();

            _cluster.Integer(alias).Put(42);
            var future = batch.Integer(alias).Add(123);
            _cluster.RunBatch(batch);

            Assert.AreEqual(165, future.Result);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void ThrowsAliasNotFound()
        {
            var batch = new QdbBatch();
            var alias = RandomGenerator.CreateUniqueAlias();

            var future = batch.Integer(alias).Add(123);
            _cluster.RunBatch(batch);

            var dummy = future.Result; // <- QdbAliasNotFoundException
        }
    }
}