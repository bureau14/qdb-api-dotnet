﻿using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quasardb.Tests.Entry.Tag
{
    [TestClass]
    public class GetEntries
    {
        [TestMethod]
        public void ReturnsEmptyCollection()
        {
            var tag = QdbTestCluster.CreateEmptyTag();

            var entries = tag.GetEntries();

            Assert.IsFalse(entries.Any());
        }

        [TestMethod]
        public void ReturnsOneBlob()
        {
            var tag = QdbTestCluster.CreateEmptyTag();
            var blob1 = QdbTestCluster.CreateTaggedBlob(tag);

            var blob2 = tag.GetEntries().Cast<QdbBlob>().Single();

            Assert.AreEqual(blob1.Alias, blob2.Alias);
        }

        [TestMethod]
        public void ReturnsOneInteger()
        {
            var tag = QdbTestCluster.CreateEmptyTag();
            var integer1 = QdbTestCluster.CreateTaggedInteger(tag);

            var integer2 = tag.GetEntries().Cast<QdbInteger>().Single();

            Assert.AreEqual(integer1.Alias, integer2.Alias);
        }

        [TestMethod]
        public void ReturnsOneTag()
        {
            var tag = QdbTestCluster.CreateEmptyTag();
            var tag1 = QdbTestCluster.CreateTaggedTag(tag);

            var tag2 = tag.GetEntries().Cast<QdbTag>().Single();

            Assert.AreEqual(tag1.Alias, tag2.Alias);
        }

        [TestMethod]
        public void ReturnOneOfEach()
        {
            var tag = QdbTestCluster.CreateEmptyTag();
            tag.AttachEntry(QdbTestCluster.CreateBlob());
            tag.AttachEntry(QdbTestCluster.CreateInteger());
            tag.AttachEntry(QdbTestCluster.CreateTag());

            Assert.AreEqual(3, tag.GetEntries().Count());
            Assert.AreEqual(1, tag.GetEntries().OfType<QdbBlob>().Count());
            Assert.AreEqual(1, tag.GetEntries().OfType<QdbInteger>().Count());
            Assert.AreEqual(1, tag.GetEntries().OfType<QdbTag>().Count());
        }
    }
}
