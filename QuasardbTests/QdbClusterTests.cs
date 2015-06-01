using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;

namespace QuasardbTests
{
    [TestClass]
    public class QdbClusterTests
    {
        [TestMethod]
        public void CreateAndDispose()
        {
            var cluster = new QdbCluster("qdb://127.0.0.1:2836");
            cluster.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConnectToAnInvalidAddress()
        {
            var cluster = new QdbCluster("clearly this is not a uri");
            cluster.Dispose();
        }
    }
}
