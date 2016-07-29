using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbEntryTests
{
    [TestClass]
    public class AddTag
    {
        [TestMethod]
        public void ReturnsTrue_WhenCalledOnce()
        {
            var entry = QdbTestCluster.CreateBlob();
            var tag = QdbTestCluster.CreateEmptyTag();

            var result = entry.AddTag(tag);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ReturnsFalse_WhenCalledTwice()
        {
            var entry = QdbTestCluster.CreateBlob();
            var tag = QdbTestCluster.CreateEmptyTag();

            entry.AddTag(tag);
            var result = entry.AddTag(tag);

            Assert.IsFalse(result);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void ThrowsIncompatibleType()
        {
            var entry = QdbTestCluster.CreateBlob();

            entry.AddTag(entry.Alias);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNull_WithQdbTagArg()
        {
            var entry = QdbTestCluster.CreateBlob();

            entry.AddTag((QdbTag)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNull_WithStringArg()
        {
            var entry = QdbTestCluster.CreateBlob();

            entry.AddTag((string)null);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void ThrowsAliasNotFound()
        {
            var tag = QdbTestCluster.CreateEmptyTag();

            QdbTestCluster.CreateEmptyBlob().AddTag(tag);
        }
    }
}
