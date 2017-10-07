using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.Tests.Helpers;

namespace Quasardb.Tests.QdbIntegerTests
{
    [TestClass]
    public class Add
    {
        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var integer = QdbTestCluster.CreateEmptyInteger();

            try
            {
                integer.Add(22);
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(integer.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ThrowIncompatibleType()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var integer = QdbTestCluster.CreateEmptyInteger(alias);

            QdbTestCluster.CreateBlob(alias);

            try
            {
                integer.Add(22);
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(integer.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ReturnsSum()
        {
            var integer = QdbTestCluster.CreateEmptyInteger();

            integer.Put(1934);
            var sum = integer.Add(22);

            Assert.AreEqual(1956, sum);
        }
    }
}
