using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;

namespace Quasardb.Tests.Entry.Stream
{
    [TestClass]
    public class Remove
    {
        [TestMethod]
        public void ReturnsFalse_WhenAliasDoesntExist()
        {
            var stream = QdbTestCluster.Instance.Stream(RandomGenerator.CreateUniqueAlias());

            var result = stream.Remove();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ReturnsFalse_WhenCalledTwice()
        {
            var stream = QdbTestCluster.CreateStream();

            var result1 = stream.Remove();
            var result2 = stream.Remove();

            Assert.IsTrue(result1);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void ThrowsQdbResourceLocked()
        {
            var stream = QdbTestCluster.CreateEmptyStream();
            stream.Open(QdbStreamMode.Append);

            try
            {
                stream.Remove();
                Assert.Fail("No exception thrown");
            }
            catch (QdbResourceLockedException e)
            {
                Assert.AreEqual(stream.Alias, e.Alias);
            }
        }
    }
}
