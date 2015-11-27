using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbStreamTests
{
    [TestClass]
    public class CanRead
    {
        [TestMethod]
        public void ReturnsTrue()
        {
            var stream = QdbTestCluster.CreateAndOpenStream();
            Assert.IsTrue(stream.CanRead);
        }

        [TestMethod]
        public void ReturnsFalse()
        {
            var stream = QdbTestCluster.CreateAndOpenStream();
            stream.Close();
            Assert.IsFalse(stream.CanRead);
        }

        /*[TestMethod]
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
        }*/
    }
}
