using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbIntegerTests
{
    [TestClass]
    public class Put
    {
        [TestMethod]
        public void ThrowsAliasAlreadyExists()
        {
            var integer = QdbTestCluster.CreateEmptyInteger();

            integer.Put(1934);

            try
            {
                integer.Put(1956);
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasAlreadyExistsException e)
            {
                Assert.AreEqual(integer.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ThrowsIncompatibleType()
        {
            var integer = QdbTestCluster.CreateEmptyInteger();

            QdbTestCluster.CreateBlob(integer.Alias);

            try
            {
                integer.Put(42);
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(integer.Alias, e.Alias);
            }
        }
    }
}
