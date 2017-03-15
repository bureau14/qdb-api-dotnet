using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.Tests.Helpers;

namespace Quasardb.Tests.QdbTagTests
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
        public void ThrowsAliasNotFound()
        {
            var tag = QdbTestCluster.CreateEmptyTag();
            var alias = RandomGenerator.CreateUniqueAlias();

            try
            {
                tag.AttachEntry(alias);
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
            var alias = RandomGenerator.CreateUniqueAlias();
            var tag = QdbTestCluster.CreateEmptyTag(alias);
            QdbTestCluster.CreateBlob(alias);

            try
            {
                tag.AttachEntry(alias);
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(alias, e.Alias);
            }
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
