using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.Tests.Helpers;

namespace Quasardb.Tests.QdbTagTests
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
        public void ThrowsAliasNotFound()
        {
            var tag = QdbTestCluster.CreateEmptyTag();
            var alias = RandomGenerator.CreateUniqueAlias();

            try
            {
                tag.DetachEntry(alias);
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(alias, e.Alias);
            }
        }

        [TestMethod]
        public void ThrowsIncompatibleType()
        {
            var tag = QdbTestCluster.CreateEmptyTag();
            QdbTestCluster.CreateBlob(tag.Alias);

            try
            {
                tag.DetachEntry(QdbTestCluster.CreateBlob());
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(tag.Alias, e.Alias);
            }
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
