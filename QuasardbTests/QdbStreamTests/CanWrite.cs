using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbStreamTests
{
    [TestClass]
    public class CanWrite
    {
        [TestMethod]
        public void ReturnsFalse_InReadMode()
        {
            var stream = QdbTestCluster.CreateStream();
            Assert.IsFalse(stream.Open(QdbStreamMode.Open).CanWrite);
        }

        [TestMethod]
        public void ReturnsTrue_InAppendMode()
        {
            var stream = QdbTestCluster.CreateStream();
            Assert.IsTrue(stream.Open(QdbStreamMode.Append).CanWrite);
        }

        [TestMethod]
        public void ReturnsFalse_AfterClose()
        {
            var stream = QdbTestCluster.CreateAndOpenStream();
            stream.Close();
            Assert.IsFalse(stream.CanWrite);
        }
    }
}
