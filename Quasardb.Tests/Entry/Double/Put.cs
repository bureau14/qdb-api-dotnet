using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;

namespace Quasardb.Tests.Entry.Double
{
    [TestClass]
    public class Put
    {
        [TestMethod]
        public void ThrowsAliasAlreadyExists()
        {
            var d = QdbTestCluster.CreateEmptyDouble();

            d.Put(1934.33);

            try
            {
                d.Put(1956.33);
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasAlreadyExistsException e)
            {
                Assert.AreEqual(d.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ThrowsIncompatibleType()
        {
            var d = QdbTestCluster.CreateEmptyDouble();

            QdbTestCluster.CreateBlob(d.Alias);

            try
            {
                d.Put(42.33);
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(d.Alias, e.Alias);
            }
        }
    }
}
