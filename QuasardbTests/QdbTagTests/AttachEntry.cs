using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbTagTests
{
    [TestClass]
    public class AttachEntry
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNull_GivenNullString()
        {
            var tag = QdbTestCluster.CreateEmptyTag();
            tag.AttachEntry((string)null); // <- throws
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNull_GivenNullQdbEntry()
        {
            var tag = QdbTestCluster.CreateEmptyTag();

            tag.AttachEntry((QdbEntry)null); // <- throws
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void ThrowsAliasNotFound()
        {
            var tag = QdbTestCluster.CreateEmptyTag();
            var alias = RandomGenerator.CreateUniqueAlias();

            tag.AttachEntry(alias); // <- throws
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void ThrowsIncompatibleType()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var tag = QdbTestCluster.CreateEmptyTag(alias);
            QdbTestCluster.CreateBlob(alias);

            tag.AttachEntry(QdbTestCluster.CreateBlob()); // <- throws
        }

        [TestMethod]
        public void ReturnsTrueOnFirstCall_GiveQdbEntry()
        {
            var tag = QdbTestCluster.CreateEmptyTag();
            var blob = QdbTestCluster.CreateBlob();
            Assert.IsTrue(tag.AttachEntry(blob));
            Assert.IsFalse(tag.AttachEntry(blob));
        }

        [TestMethod]
        public void ReturnsTrueOnFirstCall_GiveString()
        {
            var tag = QdbTestCluster.CreateEmptyTag();
            var blob = QdbTestCluster.CreateBlob().Alias;
            Assert.IsTrue(tag.AttachEntry(blob));
            Assert.IsFalse(tag.AttachEntry(blob));
        }
    }
}
