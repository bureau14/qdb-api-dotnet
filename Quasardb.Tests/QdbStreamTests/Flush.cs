using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Tests.Helpers;

namespace Quasardb.Tests.QdbStreamTests
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
