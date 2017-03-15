using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.Tests.Helpers;

namespace Quasardb.Tests.QdbIntegerTests
{
    [TestClass]
    public class Get
    {
        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var integer = QdbTestCluster.CreateEmptyInteger();

            try
            {
                integer.Get();
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(integer.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ThrowsIncompatibleType()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var integer = QdbTestCluster.CreateEmptyInteger(alias);

            QdbTestCluster.CreateBlob(alias);

            try
            {
                integer.Get();
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(integer.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ReturnsSum_AfterAdd()
        {
            var integer = QdbTestCluster.CreateEmptyInteger();

            integer.Put(1934);
            integer.Add(22);
            var sum = integer.Get();

            Assert.AreEqual(1956, sum);
        }

        [TestMethod]
        public void ReturnsValue_AfterPut()
        {
            var integer = QdbTestCluster.CreateEmptyInteger();

            integer.Put(42);
            var value = integer.Get();
            
            Assert.AreEqual(42, value);
        }

        [TestMethod]
        public void ReturnsNewValue_AfterUpdate()
        {
            var integer = QdbTestCluster.CreateEmptyInteger();

            integer.Put(1934);
            integer.Update(1956);
            var value = integer.Get();

            Assert.AreEqual(1956, value);
        }
    }
}
