using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
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
        public void Empty()
        {
            var entries = _tag.GetEntries();
            Assert.IsFalse(entries.Any());
        }

        [TestMethod]
        public void GetEntries_OneBlob()
        {
            var blob1 = QdbTestCluster.CreateTaggedBlob(_tag);
            var blob2 = _tag.GetEntries().Cast<QdbBlob>().Single();

            Assert.AreEqual(blob1.Alias, blob2.Alias);
        }

        [TestMethod]
        public void GetEntries_OneQueue()
        {
            var queue1 = QdbTestCluster.CreateTaggedQueue(_tag);
            var queue2 = _tag.GetEntries().Cast<QdbQueue>().Single();

            Assert.AreEqual(queue1.Alias, queue2.Alias);
        }

        [TestMethod]
        public void GetEntries_OneHashSet()
        {
            var hashSet1 = QdbTestCluster.CreateTaggedHashSet(_tag);
            var hashSet2 = _tag.GetEntries().Cast<QdbHashSet>().Single();

            Assert.AreEqual(hashSet1.Alias, hashSet2.Alias);
        }

        [TestMethod]
        public void GetEntries_OneInteger()
        {
            var integer1 = QdbTestCluster.CreateTaggedInteger(_tag);
            var integer2 = _tag.GetEntries().Cast<QdbInteger>().Single();

            Assert.AreEqual(integer1.Alias, integer2.Alias);
        }

        [TestMethod]
        public void GetEntries_OneTag()
        {
            var tag1 = QdbTestCluster.CreateTaggedTag(_tag);
            var tag2 = _tag.GetEntries().Cast<QdbTag>().Single();

            Assert.AreEqual(tag1.Alias, tag2.Alias);
        }

        [TestMethod]
        public void GetEntries_OneOfEach()
        {
            _tag.AddEntry(QdbTestCluster.CreateBlob());
            _tag.AddEntry(QdbTestCluster.CreateHashSet());
            _tag.AddEntry(QdbTestCluster.CreateInteger());
            _tag.AddEntry(QdbTestCluster.CreateQueue());
            _tag.AddEntry(QdbTestCluster.CreateTag());

            Assert.AreEqual(5, _tag.GetEntries().Count());
            Assert.AreEqual(1, _tag.GetEntries().OfType<QdbBlob>().Count());
            Assert.AreEqual(1, _tag.GetEntries().OfType<QdbHashSet>().Count());
            Assert.AreEqual(1, _tag.GetEntries().OfType<QdbInteger>().Count());
            Assert.AreEqual(1, _tag.GetEntries().OfType<QdbQueue>().Count());
            Assert.AreEqual(1, _tag.GetEntries().OfType<QdbTag>().Count());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddEntry_String_Null()
        {
            _tag.AddEntry((string)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddEntry_QdbEntry_Null()
        {
            _tag.AddEntry((QdbEntry)null);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void AddEntry_NotFound()
        {
            _tag.AddEntry(RandomGenerator.CreateUniqueAlias());
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void AddEntry_Incompatible()
        {
            QdbTestCluster.CreateBlob(_tag.Alias);
            _tag.AddEntry(QdbTestCluster.CreateBlob());
        }

        [TestMethod]
        public void AddEntry_QdbEntry()
        {
            var blob = QdbTestCluster.CreateBlob();
            Assert.IsTrue(_tag.AddEntry(blob));
            Assert.IsFalse(_tag.AddEntry(blob));
        }

        [TestMethod]
        public void AddEntry_String()
        {
            var blob = QdbTestCluster.CreateBlob().Alias;
            Assert.IsTrue(_tag.AddEntry(blob));
            Assert.IsFalse(_tag.AddEntry(blob));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveEntry_String_Null()
        {
            _tag.RemoveEntry((string)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveEntry_QdbEntry_Null()
        {
            _tag.RemoveEntry((QdbEntry)null);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void RemoveEntry_NotFound()
        {
            _tag.RemoveEntry(RandomGenerator.CreateUniqueAlias());
        }

        [TestMethod]
        [Ignore]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void RemoveEntry_Incompatible()
        {
            QdbTestCluster.CreateBlob(_tag.Alias);
            _tag.RemoveEntry(QdbTestCluster.CreateBlob());
        }

        [TestMethod]
        public void RemoveEntry_QdbEntry()
        {
            var blob = QdbTestCluster.CreateTaggedBlob(_tag);
            Assert.IsTrue(_tag.RemoveEntry(blob));
            Assert.IsFalse(_tag.RemoveEntry(blob));
        }

        [TestMethod]
        public void RemoveEntry_String()
        {
            var blob = QdbTestCluster.CreateTaggedBlob(_tag).Alias;
            Assert.IsTrue(_tag.RemoveEntry(blob));
            Assert.IsFalse(_tag.RemoveEntry(blob));
        }

        [TestMethod]
        public void HasEntry_QdbEntry()
        {
            var blob = QdbTestCluster.CreateBlob();

            Assert.IsFalse(_tag.HasEntry(blob));
            _tag.AddEntry(blob);
            Assert.IsTrue(_tag.HasEntry(blob));
        }

        [TestMethod]
        public void HasEntry_String()
        {
            var blob = QdbTestCluster.CreateBlob().Alias;

            Assert.IsFalse(_tag.HasEntry(blob));
            _tag.AddEntry(blob);
            Assert.IsTrue(_tag.HasEntry(blob));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HasEntry_String_Null()
        {
            _tag.HasEntry((string)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HasEntry_QdbEntry_Null()
        {
            _tag.HasEntry((QdbEntry)null);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void HasEntry_NotFound()
        {
            _tag.HasEntry(RandomGenerator.CreateUniqueAlias());
        }

        [TestMethod]
        [Ignore]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void HasEntry_Incompatible()
        {
            QdbTestCluster.CreateBlob(_tag.Alias);
            _tag.HasEntry(QdbTestCluster.CreateBlob());
        }
    }
}
