using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbStream
{
    [TestClass]
    public class Open
    {
        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void ThrowsAliasNotFoundException()
        {
            QdbTestCluster.Instance.Stream("toto").Open(QdbStreamMode.Open);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void ThrowsIncompatibleTypeException_OpenMode()
        {
            var blob = QdbTestCluster.CreateBlob();
            QdbTestCluster.Instance.Stream(blob.Alias).Open(QdbStreamMode.Open);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void ThrowsIncompatibleTypeException_AppendMode()
        {
            var blob = QdbTestCluster.CreateBlob();
            QdbTestCluster.Instance.Stream(blob.Alias).Open(QdbStreamMode.Append);
        }
    }
}
