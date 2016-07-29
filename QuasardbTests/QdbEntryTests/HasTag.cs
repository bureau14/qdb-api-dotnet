using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbEntryTests
{
    [TestClass]
    public class HasTag
    {
        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void ThrowsAliasNotFound()
        {
            var tag = QdbTestCluster.CreateEmptyTag();

            QdbTestCluster.CreateEmptyBlob().HasTag(tag);
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
        public void ReturnsFalse_AfterRemoveTag()
        {
            var entry = QdbTestCluster.CreateBlob();
            var tag = QdbTestCluster.CreateEmptyTag();

            entry.AddTag(tag);
            entry.RemoveTag(tag);

            Assert.IsFalse(entry.HasTag(tag));
        }

        [TestMethod]
        public void ReturnsTrue_AfterAddTag_WithQdbTagArg()
        {
            var entry = QdbTestCluster.CreateBlob();
            var tag = QdbTestCluster.CreateEmptyTag();

            entry.AddTag(tag);
            Assert.IsTrue(entry.HasTag(tag));
        }

        [TestMethod]
        public void ReturnsTrue_AfterAddTag_WithStringArg()
        {
            var entry = QdbTestCluster.CreateBlob();
            var tag = QdbTestCluster.CreateEmptyTag();

            entry.AddTag(tag.Alias);
            Assert.IsTrue(entry.HasTag(tag.Alias));
        }
    }
}
