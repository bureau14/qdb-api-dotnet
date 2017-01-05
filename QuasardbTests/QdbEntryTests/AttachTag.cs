using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbEntryTests
{
    [TestClass]
    public class AttachTag
    {
        [TestMethod]
        public void ReturnsTrue_WhenCalledOnce()
        {
            var entry = QdbTestCluster.CreateBlob();
            var tag = QdbTestCluster.CreateEmptyTag();

            var result = entry.AttachTag(tag);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ReturnsFalse_WhenCalledTwice()
        {
            var entry = QdbTestCluster.CreateBlob();
            var tag = QdbTestCluster.CreateEmptyTag();

            entry.AttachTag(tag);
            var result = entry.AttachTag(tag);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ThrowsIncompatibleType()
        {
            var entry = QdbTestCluster.CreateBlob();
            
            try
            {
                entry.AttachTag(entry.Alias);
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(entry.Alias, e.Alias);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNull_WithQdbTagArg()
        {
            var entry = QdbTestCluster.CreateBlob();

            entry.AttachTag((QdbTag)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNull_WithStringArg()
        {
            var entry = QdbTestCluster.CreateBlob();

            entry.AttachTag((string)null);
        }

        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var tag = QdbTestCluster.CreateEmptyTag();
            var blob = QdbTestCluster.CreateEmptyBlob();
            
            try
            {
                blob.AttachTag(tag);
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(blob.Alias, e.Alias);
            }
        }
    }
}
