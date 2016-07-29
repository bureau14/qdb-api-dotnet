using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbEntryTests
{
    [TestClass]
    public class GetTags
    {
        [TestMethod]
        public void ReturnsEmptyList()
        {
            var entry = QdbTestCluster.CreateBlob();

            var tags = entry.GetTags();
            Assert.IsFalse(tags.Any());
        }

        [TestMethod]
        public void ReturnsOneTag_AfterAddTag()
        {
            var entry = QdbTestCluster.CreateBlob();
            var tag1 = QdbTestCluster.CreateEmptyTag();

            entry.AddTag(tag1);
            var tag2 = entry.GetTags().Single();

            Assert.AreEqual(tag1.Alias, tag2.Alias);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void ThrowsAliasNotFound()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            QdbTestCluster.CreateEmptyBlob().GetTags().Any();
        }
    }
}
