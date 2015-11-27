using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbStreamTests
{
    [TestClass]
    public class Open
    {
        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void ThrowsAliasNotFoundException()
        {
            var stream = QdbTestCluster.Instance.Stream(RandomGenerator.CreateUniqueAlias());
            stream.Open(QdbStreamMode.Read);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void ThrowsIncompatibleTypeException_OpenMode()
        {
            var blob = QdbTestCluster.CreateBlob();
            QdbTestCluster.Instance.Stream(blob.Alias).Open(QdbStreamMode.Read);
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
