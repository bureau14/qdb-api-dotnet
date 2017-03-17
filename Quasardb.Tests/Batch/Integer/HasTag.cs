using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quasardb.Tests.Batch.Integer
{
    [TestClass]
    public class HasTag
    {
        private readonly QdbCluster _cluster = QdbTestCluster.Instance;

        [TestMethod]
        public void ReturnsFalse()
        {
            var batch = new QdbBatch();
            var alias = RandomGenerator.CreateUniqueAlias();
            var tag = "dzedez";

            _cluster.Integer(alias).Put(42);
            var future = batch.Integer(alias).HasTag(tag);
            _cluster.RunBatch(batch);

            Assert.IsFalse(future.Result);
        }

        [TestMethod]
        public void ReturnsTrue()
        {
            var batch = new QdbBatch();
            var alias = RandomGenerator.CreateUniqueAlias();
            var tag = RandomGenerator.CreateUniqueAlias();

            _cluster.Integer(alias).Put(42);
            _cluster.Integer(alias).AttachTag(tag);
            var future = batch.Integer(alias).HasTag(tag);
            _cluster.RunBatch(batch);

            Assert.IsTrue(future.Result);
        }
    }
}