using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests
{
    [TestClass]
    public class QdbStreamTest
    {

        [TestMethod]
        public void Open()
        {
            Stream s = QdbTestCluster.Instance.Stream("toto").Open(FileMode.Open);
        }
    }
}
