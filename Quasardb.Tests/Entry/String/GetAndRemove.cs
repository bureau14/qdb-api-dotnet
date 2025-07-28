using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;

namespace Quasardb.Tests.Entry.String
{
    [TestClass]
    public class GetAndRemove
    {
        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var s = QdbTestCluster.CreateEmptyString();

            try
            {
                s.GetAndRemove();
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(s.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ThrowsAliasNotFound_WhenCalledTwice()
        {
            var s = QdbTestCluster.CreateString();

            s.GetAndRemove();

            try
            {
                s.GetAndRemove();
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(s.Alias, e.Alias);
            }
        }
    }
}
