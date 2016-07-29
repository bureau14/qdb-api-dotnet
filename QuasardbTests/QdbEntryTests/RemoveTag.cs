using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbEntryTests
{
    [TestClass]
    public class RemoveTag
    {
        [TestMethod]
        public void ReturnsTrue_AfterAddTag_WithQdbTagArg()
        {
            var entry = QdbTestCluster.CreateBlob();
            var tag = QdbTestCluster.CreateEmptyTag();

            entry.AddTag(tag);
            Assert.IsTrue(entry.RemoveTag(tag));
        }

        [TestMethod]
        public void ReturnsTrue_AfterAddTag_WithStringArg()
        {
            var entry = QdbTestCluster.CreateBlob();
            var tag = QdbTestCluster.CreateEmptyTag();

            entry.AddTag(tag.Alias);
            Assert.IsTrue(entry.RemoveTag(tag.Alias));
        }
        
        [TestMethod]
        public void ReturnsFalse_WithQdbTagArg()
        {
            var entry = QdbTestCluster.CreateBlob();
            var tag = QdbTestCluster.CreateEmptyTag();

            Assert.IsFalse(entry.RemoveTag(tag));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNull_WithQdbTagArg()
        {
            var entry = QdbTestCluster.CreateBlob();
            var tag = QdbTestCluster.CreateEmptyTag();

            entry.RemoveTag((QdbTag)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNull_WithStringArg()
        {
            var entry = QdbTestCluster.CreateBlob();
            var tag = QdbTestCluster.CreateEmptyTag();

            entry.RemoveTag((string)null);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void ThrowsIncompatibleType()
        {
            var entry = QdbTestCluster.CreateBlob();
            var tag = QdbTestCluster.CreateEmptyTag();

            entry.RemoveTag(QdbTestCluster.CreateBlob().Alias);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void ThrowsAliasNotFound()
        {
            var entry = QdbTestCluster.CreateBlob();
            var tag = QdbTestCluster.CreateEmptyTag();

            QdbTestCluster.CreateEmptyBlob().RemoveTag(tag);
        }
    }
}
