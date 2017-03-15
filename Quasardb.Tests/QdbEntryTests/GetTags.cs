using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.Tests.Helpers;

namespace Quasardb.Tests.QdbEntryTests
{
    [TestClass]
    public class GetTags
    {
        [TestMethod]
        public void ReturnsEmptyList()
        {
            var entry = QdbTestCluster.CreateBlob();

            var tags = entry.GetTags();
            Assert.IsFalse(tags.Any());
        }

        [TestMethod]
        public void ReturnsOneTag_AfterAttachTag()
        {
            var entry = QdbTestCluster.CreateBlob();
            var tag1 = QdbTestCluster.CreateEmptyTag();

            entry.AttachTag(tag1);
            var tag2 = entry.GetTags().Single();

            Assert.AreEqual(tag1.Alias, tag2.Alias);
        }

        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var blob = QdbTestCluster.CreateEmptyBlob();
            
            try
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                blob.GetTags().Any();
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(blob.Alias, e.Alias);
            }
        }
    }
}
