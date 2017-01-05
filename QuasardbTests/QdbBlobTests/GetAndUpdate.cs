using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbBlobTests
{
    [TestClass]
    public class GetAndUpdate
    {
        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content = RandomGenerator.CreateRandomContent();

            try
            {
                blob.GetAndUpdate(content);
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(blob.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ReturnsOriginalContent_AfterPut()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content1 = RandomGenerator.CreateRandomContent();
            var content2 = RandomGenerator.CreateRandomContent();

            blob.Put(content1);
            var result = blob.GetAndUpdate(content2);

            CollectionAssert.AreEqual(content1, result);
        }
    }
}
