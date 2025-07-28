using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;

namespace Quasardb.Tests.Entry.Double
{
    [TestClass]
    public class Add
    {
        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var d = QdbTestCluster.CreateEmptyDouble();

            try
            {
                d.Add(22.33);
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(d.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ThrowIncompatibleType()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var d = QdbTestCluster.CreateEmptyDouble(alias);

            QdbTestCluster.CreateBlob(alias);

            try
            {
                d.Add(22.33);
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(d.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ReturnsSum()
        {
            var d = QdbTestCluster.CreateEmptyDouble();

            d.Put(1934.33);
            var sum = d.Add(22.11);

            Assert.AreEqual(1956.44, sum, 0.001);
        }
    }
}
