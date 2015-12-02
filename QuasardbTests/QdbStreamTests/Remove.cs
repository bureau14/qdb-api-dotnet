using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbStreamTests
{
    [TestClass]
    public class Remove
    {
        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void ThrowsAliasNotFoundException_OnRandomAlias()
        {
            QdbTestCluster.Instance.Stream(RandomGenerator.CreateUniqueAlias()).Remove();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void ThrowsAliasNotFoundException_WhenCalledTwice()
        {
            var stream = QdbTestCluster.CreateStream();
            stream.Remove();
            stream.Remove();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbResourceLockedException))]
        public void ThrowsQdbResourceLocked()
        {
            var stream = QdbTestCluster.CreateEmptyStream();
            stream.Open(QdbStreamMode.Append);
            stream.Remove();
        }
    }
}
