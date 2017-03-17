using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;

namespace Quasardb.Tests.Entry.Blob
{
    [TestClass]
    public class GetAndRemove
    {
        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();

            try
            {
                blob.GetAndRemove();
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(blob.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ThrowsAliasNotFound_WhenCalledTwice()
        {
            var blob = QdbTestCluster.CreateBlob();
            
            blob.GetAndRemove();

            try
            {
                blob.GetAndRemove();
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(blob.Alias, e.Alias);
            }
        }
    }
}
