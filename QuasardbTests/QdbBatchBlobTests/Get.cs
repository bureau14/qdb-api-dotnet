using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbBatchBlobTests
{
    [TestClass]
    public class Get
    {
        private readonly QdbCluster _cluster = QdbTestCluster.Instance;

        [TestMethod]
        public void ReturnsContentOfBlob()
        {
            var batch = new QdbBatch();
            var alias = RandomGenerator.CreateUniqueAlias();
            var content = RandomGenerator.CreateRandomContent();

            _cluster.Blob(alias).Put(content);
            var future = batch.Blob(alias).Get();
            _cluster.RunBatch(batch);

            Assert.AreEqual(content.Length, future.Result.Length);
            CollectionAssert.AreEqual(content, future.Result);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void ThrowsAliasNotFound()
        {
            var batch = new QdbBatch();
            var alias = RandomGenerator.CreateUniqueAlias();

            var future = batch.Blob(alias).Get();
            _cluster.RunBatch(batch);

            var dummy = future.Result; // <- QdbAliasNotFoundException
        }
    }
}