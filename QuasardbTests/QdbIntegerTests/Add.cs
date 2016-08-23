using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbIntegerTests
{
    [TestClass]
    public class Add
    {
        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void ThrowsAliasNotFound()
        {
            var integer = QdbTestCluster.CreateEmptyInteger();

            integer.Add(22); // <- throws
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void ThrowIncompatibleType()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var integer = QdbTestCluster.CreateEmptyInteger(alias);

            QdbTestCluster.CreateBlob(alias);
            integer.Add(22); // <- throws
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
