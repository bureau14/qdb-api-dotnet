using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quasardb.Tests.Entry.Stream
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
