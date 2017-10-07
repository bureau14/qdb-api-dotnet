using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Tests.Helpers;

namespace Quasardb.Tests
{
    [TestClass]
    public class QdbBatchTests
    {
        [TestMethod]
        public void RunEmpty()
        {
            var batch = new QdbBatch();
            QdbTestCluster.Instance.RunBatch(batch);
        }

        [TestMethod]
        public void GivenEmptyBatch_SizeShouldReturn0()
        {
            var batch = new QdbBatch();
            Assert.AreEqual(0, batch.Size);
        }
    }
}