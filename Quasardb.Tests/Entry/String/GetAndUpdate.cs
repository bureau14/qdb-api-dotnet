using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;

namespace Quasardb.Tests.Entry.String
{
    [TestClass]
    public class GetAndUpdate
    {
        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content = RandomGenerator.CreateRandomString();

            try
            {
                s.GetAndUpdate(content);
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(s.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ReturnsOriginalContent_AfterPut()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content1 = RandomGenerator.CreateRandomString();
            var content2 = RandomGenerator.CreateRandomString();

            s.Put(content1);
            var result = s.GetAndUpdate(content2);

            Assert.AreEqual(content1, result);
        }
    }
}
