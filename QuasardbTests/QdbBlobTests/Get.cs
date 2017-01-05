using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbBlobTests
{
    [TestClass]
    public class Get
    {
        [TestMethod]
        public void ThrowAliasNotFound()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
           
            try
            {
                blob.Get();
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(blob.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ReturnsContent_AfterPut()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content = RandomGenerator.CreateRandomContent();

            blob.Put(content);
            var result = blob.Get();

            CollectionAssert.AreEqual(content, result);
        }


        [TestMethod]
        public void ReturnsUpdatedContent_AfterGetAndUpdate()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content1 = RandomGenerator.CreateRandomContent();
            var content2 = RandomGenerator.CreateRandomContent();

            blob.Put(content1);
            blob.GetAndUpdate(content2);
            var result = blob.Get();

            CollectionAssert.AreEqual(content2, result);
        }

        [TestMethod]
        public void ReturnsUpdatedContent_AfterUpdate()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content1 = RandomGenerator.CreateRandomContent();
            var content2 = RandomGenerator.CreateRandomContent();

            blob.Put(content1);
            blob.Update(content2);
            var result = blob.Get();

            CollectionAssert.AreEqual(content2, result);
        }

        [TestMethod]
        public void ReturnsOriginalContent_AfterCompareAndSwap()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content1 = RandomGenerator.CreateRandomContent();
            var content2 = RandomGenerator.CreateRandomContent();

            blob.Put(content1);
            blob.CompareAndSwap(content2, content2);
            var result = blob.Get();

            CollectionAssert.AreEqual(content1, result);
        }

        [TestMethod]
        public void ReturnsUpdatedContent_AfterCompareAndSwap()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            var content1 = RandomGenerator.CreateRandomContent();
            var content2 = RandomGenerator.CreateRandomContent();

            blob.Put(content1);
            blob.CompareAndSwap(content2, content1);
            var result = blob.Get();

            CollectionAssert.AreEqual(content2, result);
        }
    }
}
