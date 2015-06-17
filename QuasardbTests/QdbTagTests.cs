using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using QuasardbTests.Helpers;

namespace QuasardbTests
{
    [TestClass]
    public class QdbTagTests
    {
        QdbTag _tag;
        
        [TestInitialize]
        public void Initialize()
        {
            _tag = QdbTestCluster.CreateEmptyTag();
        }

        [TestMethod]
        [Ignore]
        public void Empty()
        {
            var entries = _tag.GetEntries();
            Assert.IsFalse(entries.Any());
        }

        [TestMethod]
        public void OneBlob()
        {
            var blob1 = QdbTestCluster.CreateTaggedBlob(_tag);
            var blob2 = _tag.GetEntries().Cast<QdbBlob>().Single();

            Assert.AreEqual(blob1.Alias, blob2.Alias);
        }

        [TestMethod]
        public void OneQueue()
        {
            var queue1 = QdbTestCluster.CreateTaggedQueue(_tag);
            var queue2 = _tag.GetEntries().Cast<QdbQueue>().Single();

            Assert.AreEqual(queue1.Alias, queue2.Alias);
        }

        [TestMethod]
        public void OneHashSet()
        {
            var hashSet1 = QdbTestCluster.CreateTaggedHashSet(_tag);
            var hashSet2 = _tag.GetEntries().Cast<QdbHashSet>().Single();

            Assert.AreEqual(hashSet1.Alias, hashSet2.Alias);
        }

        [TestMethod]
        public void OneInteger()
        {
            var integer1 = QdbTestCluster.CreateTaggedInteger(_tag);
            var integer2 = _tag.GetEntries().Cast<QdbInteger>().Single();

            Assert.AreEqual(integer1.Alias, integer2.Alias);
        }

        [TestMethod]
        public void OneTag()
        {
            var tag1 = QdbTestCluster.CreateTaggedTag(_tag);
            var tag2 = _tag.GetEntries().Cast<QdbTag>().Single();

            Assert.AreEqual(tag1.Alias, tag2.Alias);
        }
    }
}
