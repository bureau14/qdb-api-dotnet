using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbStreamTests
{
    [TestClass]
    public class Flush
    {
        [TestMethod]
        public void DoesntThrow()
        {
            var stream = QdbTestCluster.CreateAndOpenStream();
            stream.Flush();
        }
    }
}
