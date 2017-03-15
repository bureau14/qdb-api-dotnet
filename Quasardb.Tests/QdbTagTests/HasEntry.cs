using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.Tests.Helpers;

namespace Quasardb.Tests.QdbTagTests
{
    [TestClass]
    public class HasEntry
    {
        [TestMethod]
        public void ReturnsTrue_AfterAttachEntry_GivenQdbEntry()
        {
            var tag = QdbTestCluster.CreateEmptyTag();
            var blob = QdbTestCluster.CreateBlob();

            Assert.IsFalse(tag.HasEntry(blob));
            tag.AttachEntry(blob);
            Assert.IsTrue(tag.HasEntry(blob));
        }

        [TestMethod]
        public void ReturnsTrue_AfterAttachEntry_GivenString()
        {
            var tag = QdbTestCluster.CreateEmptyTag();
            var blob = QdbTestCluster.CreateBlob().Alias;

            Assert.IsFalse(tag.HasEntry(blob));
            tag.AttachEntry(blob);
            Assert.IsTrue(tag.HasEntry(blob));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNull_GivenNullString()
        {
            var tag = QdbTestCluster.CreateEmptyTag();
            tag.HasEntry((string)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNull_GivenNullQdbEntry()
        {
            var tag = QdbTestCluster.CreateEmptyTag();
            tag.HasEntry((QdbEntry)null);
        }

        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var tag = QdbTestCluster.CreateEmptyTag();
            var alias = RandomGenerator.CreateUniqueAlias();
           
            try
            {
                tag.HasEntry(alias);
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(alias, e.Alias);
            }
        }
    }
}
