using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.Tests.Helpers;

namespace Quasardb.Tests.QdbEntryTests
{
    [TestClass]
    public class HasTag
    {
        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var tag = QdbTestCluster.CreateEmptyTag();
            var blob = QdbTestCluster.CreateEmptyBlob();
            
            try
            {
                blob.HasTag(tag);
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(blob.Alias, e.Alias);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNull_WithStringArg()
        {
            var entry = QdbTestCluster.CreateBlob();

            entry.HasTag((string)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNull_WithQdbTagArg()
        {
            var entry = QdbTestCluster.CreateBlob();

            entry.HasTag((QdbTag)null);
        }

        [TestMethod]
        public void ReturnsFalse()
        {
            var entry = QdbTestCluster.CreateBlob();
            var tag = QdbTestCluster.CreateEmptyTag();

            Assert.IsFalse(entry.HasTag(tag));
        }

        [TestMethod]
        public void ReturnsFalse_AfterDetachTag()
        {
            var entry = QdbTestCluster.CreateBlob();
            var tag = QdbTestCluster.CreateEmptyTag();

            entry.AttachTag(tag);
            entry.DetachTag(tag);

            Assert.IsFalse(entry.HasTag(tag));
        }

        [TestMethod]
        public void ReturnsTrue_AfterAttachTag_WithQdbTagArg()
        {
            var entry = QdbTestCluster.CreateBlob();
            var tag = QdbTestCluster.CreateEmptyTag();

            entry.AttachTag(tag);
            Assert.IsTrue(entry.HasTag(tag));
        }

        [TestMethod]
        public void ReturnsTrue_AfterAttachTag_WithStringArg()
        {
            var entry = QdbTestCluster.CreateBlob();
            var tag = QdbTestCluster.CreateEmptyTag();

            entry.AttachTag(tag.Alias);
            Assert.IsTrue(entry.HasTag(tag.Alias));
        }
    }
}
