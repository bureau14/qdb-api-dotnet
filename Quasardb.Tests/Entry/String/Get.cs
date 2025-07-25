using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;

namespace Quasardb.Tests.Entry.String
{
    [TestClass]
    public class Get
    {
        [TestMethod]
        public void ThrowAliasNotFound()
        {
            var s = QdbTestCluster.CreateEmptyString();

            try
            {
                s.Get();
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(s.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ReturnsContent_AfterPut()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content = RandomGenerator.CreateRandomString();

            s.Put(content);
            var result = s.Get();

            Assert.AreEqual(content, result);
        }

        [TestMethod]
        public void ReturnsUpdatedContent_AfterGetAndUpdate()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content1 = RandomGenerator.CreateRandomString();
            var content2 = RandomGenerator.CreateRandomString();

            s.Put(content1);
            s.GetAndUpdate(content2);
            var result = s.Get();

            Assert.AreEqual(content2, result);
        }

        [TestMethod]
        public void ReturnsUpdatedContent_AfterUpdate()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content1 = RandomGenerator.CreateRandomString();
            var content2 = RandomGenerator.CreateRandomString();

            s.Put(content1);
            s.Update(content2);
            var result = s.Get();

            Assert.AreEqual(content2, result);
        }

        [TestMethod]
        public void ReturnsOriginalContent_AfterCompareAndSwap()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content1 = RandomGenerator.CreateRandomString();
            var content2 = RandomGenerator.CreateRandomString();

            s.Put(content1);
            s.CompareAndSwap(content2, content2);
            var result = s.Get();

            Assert.AreEqual(content1, result);
        }

        [TestMethod]
        public void ReturnsUpdatedContent_AfterCompareAndSwap()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content1 = RandomGenerator.CreateRandomString();
            var content2 = RandomGenerator.CreateRandomString();

            s.Put(content1);
            s.CompareAndSwap(content2, content1);
            var result = s.Get();

            Assert.AreEqual(content2, result);
        }
    }
}
