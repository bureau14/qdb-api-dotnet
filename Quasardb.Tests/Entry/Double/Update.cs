using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;

namespace Quasardb.Tests.Entry.Double
{
    [TestClass]
    public class Update
    {
        [TestMethod]
        public void ThrowsIncompatible()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var d = QdbTestCluster.CreateEmptyDouble(alias);

            QdbTestCluster.CreateBlob(alias);
            
            try
            {
                d.Update(1956.33);
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(d.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ReturnsTrue_WhenCalledOnce()
        {
            var d = QdbTestCluster.CreateEmptyDouble();

            var created = d.Update(42.33);

            Assert.IsTrue(created);
        }

        [TestMethod]
        public void ReturnsFalse_WhenCalledTwice()
        {
            var d = QdbTestCluster.CreateEmptyDouble();

            d.Update(1934.33);
            var created = d.Update(1956.33);

            Assert.IsFalse(created);
        }
    }
}
