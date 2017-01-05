using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbIntegerTests
{
    [TestClass]
    public class Update
    {
        [TestMethod]
        public void ThrowsIncompatible()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var integer = QdbTestCluster.CreateEmptyInteger(alias);

            QdbTestCluster.CreateBlob(alias);
            
            try
            {
                integer.Update(1956);
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(integer.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ReturnsTrue_WhenCalledOnce()
        {
            var integer = QdbTestCluster.CreateEmptyInteger();

            var created = integer.Update(42);

            Assert.IsTrue(created);
        }

        [TestMethod]
        public void ReturnsFalse_WhenCalledTwice()
        {
            var integer = QdbTestCluster.CreateEmptyInteger();

            integer.Update(1934);
            var created = integer.Update(1956);

            Assert.IsFalse(created);
        }
    }
}
