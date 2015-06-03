using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;

namespace QuasardbTests
{
    [TestClass]
    public class QdbClusterTests
    {
        [TestMethod]
        public void CreateAndDispose()
        {
            var cluster = new QdbCluster(DaemonRunner.ClusterUrl);
            cluster.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbInvalidArgumentException))]
        public void ConnectToAnInvalidAddress()
        {
            var cluster = new QdbCluster("clearly this is not a uri");
            cluster.Dispose();
        }
    }
}
