using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbTagTests
{
    [TestClass]
    public class DetachEntry
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNull_GivenNullString()
        {
            var tag = QdbTestCluster.CreateEmptyTag();
            tag.DetachEntry((string)null); // <- throws
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNull_GivenNullQdbEntry()
        {
            var tag = QdbTestCluster.CreateEmptyTag();
            tag.DetachEntry((QdbEntry)null); // <- throws
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void ThrowsAliasNotFound()
        {
            var tag = QdbTestCluster.CreateEmptyTag();
            tag.DetachEntry(RandomGenerator.CreateUniqueAlias()); // <- throws
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void ThrowsIncompatibleType()
        {
            var tag = QdbTestCluster.CreateEmptyTag();
            QdbTestCluster.CreateBlob(tag.Alias);
            tag.DetachEntry(QdbTestCluster.CreateBlob()); // <- throws
        }

        [TestMethod]
        public void ReturnsTrueOnFirstCall_GivenQdbEntry()
        {
            var tag = QdbTestCluster.CreateEmptyTag();
            var blob = QdbTestCluster.CreateTaggedBlob(tag);
            Assert.IsTrue(tag.DetachEntry(blob));
            Assert.IsFalse(tag.DetachEntry(blob));
        }

        [TestMethod]
        public void ReturnsTrueOnFirstCall_GivenString()
        {
            var tag = QdbTestCluster.CreateEmptyTag();
            var blob = QdbTestCluster.CreateTaggedBlob(tag).Alias;
            Assert.IsTrue(tag.DetachEntry(blob));
            Assert.IsFalse(tag.DetachEntry(blob));
        }
    }
}
