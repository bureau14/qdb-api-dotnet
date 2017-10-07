using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Tests.Helpers;

namespace Quasardb.Tests.QdbStreamTests
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
    }
}
