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

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void GivenExistingAlias_WhenOpen_ThenIncompatibleType()
        {
            var blob = QdbTestCluster.CreateBlob();
            QdbTestCluster.Instance.Stream(blob.Alias).Open(QdbStreamMode.Open);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void GivenExistingAlias_WhenAppend_ThenIncompatibleType()
        {
            var blob = QdbTestCluster.CreateBlob();
            QdbTestCluster.Instance.Stream(blob.Alias).Open(QdbStreamMode.Append);
        }

        [TestMethod]
        public void GivenEmptyStream_ThenLengthIsZero()
        {
            using (Stream stream = QdbTestCluster.CreateStream().Open(QdbStreamMode.Append))
            {
                Assert.AreEqual(0, stream.Length);
            }
        }
    }
}
