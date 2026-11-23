using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quasardb.Tests.Batch
{
    [TestClass]
    public class QdbBatchTests
    {
        [TestMethod]
        public void HasTag_EncodesNullTerminatedUtf8()
        {
            var tagBytes = QdbBatchEntry.EncodeTag("café");

            CollectionAssert.AreEqual(new byte[] { 0x63, 0x61, 0x66, 0xC3, 0xA9, 0x00 }, tagBytes);
        }

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