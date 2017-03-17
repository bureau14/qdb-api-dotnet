using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;

namespace Quasardb.Tests.Batch.Integer
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
        public void ThrowsAliasNotFound()
        {
            var batch = new QdbBatch();
            var alias = RandomGenerator.CreateUniqueAlias();

            var future = batch.Integer(alias).Add(123);
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