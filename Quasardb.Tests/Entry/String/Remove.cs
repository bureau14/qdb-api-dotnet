using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quasardb.Tests.Entry.String
{
    [TestClass]
    public class Remove
    {
        [TestMethod]
        public void ReturnsFalse_WhenAliasDoesntExist()
        {
            var s = QdbTestCluster.CreateEmptyString();

            var result = s.Remove();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ReturnsTrue_WhenAliasExists()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content = RandomGenerator.CreateRandomString();

            s.Put(content);
            var result = s.Remove();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ReturnsFalse_WhenCalledTwice()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content = RandomGenerator.CreateRandomString();

            s.Put(content);
            s.Remove();
            var result = s.Remove();

            Assert.IsFalse(result);
        }
    }
}
