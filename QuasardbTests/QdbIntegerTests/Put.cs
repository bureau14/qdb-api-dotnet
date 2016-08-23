using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbIntegerTests
{
    [TestClass]
    public class Put
    {
        [TestMethod]
        [ExpectedException(typeof(QdbAliasAlreadyExistsException))]
        public void ThrowsAliasAlreadyExists()
        {
            var integer = QdbTestCluster.CreateEmptyInteger();

            integer.Put(1934);
            integer.Put(1956); // <- throws
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void ThrowsIncompatibleType()
        {
            var integer = QdbTestCluster.CreateEmptyInteger();

            QdbTestCluster.CreateBlob(integer.Alias);
            integer.Put(42); // <- throws
        }
    }
}
