using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbIntegerTests
{
    [TestClass]
    public class Get
    {
        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void ThrowsAliasNotFound()
        {
            var integer = QdbTestCluster.CreateEmptyInteger();

            integer.Get(); // <- throws
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void ThrowsIncompatibleType()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var integer = QdbTestCluster.CreateEmptyInteger(alias);

            QdbTestCluster.CreateBlob(alias);
            integer.Get(); // <- throws
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
