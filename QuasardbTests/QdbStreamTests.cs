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
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void GivenRandomAlias_WhenOpen_ThenAliasNotFound()
        {
            QdbTestCluster.Instance.Stream("toto").Open(QdbStreamMode.Open);
        }
    }
}
