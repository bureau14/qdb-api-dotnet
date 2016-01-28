using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests
{
    [TestClass]
    public class QdbEntryTests
    {
        QdbEntry _entry;
        QdbTag _tag1;

        [TestInitialize]
        public void Initialize()
        {
            _entry = QdbTestCluster.CreateBlob();
            _tag1 = QdbTestCluster.CreateEmptyTag();
        }

        [TestMethod]
        public void AddTag()
        {
            var result = _entry.AddTag(_tag1);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AddTag_AddTag()
        {
            _entry.AddTag(_tag1);
            var result = _entry.AddTag(_tag1);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void AddTag_GetTags()
        {
            _entry.AddTag(_tag1);
            var tag2 = _entry.GetTags().Single();

            Assert.AreEqual(_tag1.Alias, tag2.Alias);
        }

        [TestMethod]
        public void AddTag_HasTag_QdbTag()
        {
            _entry.AddTag(_tag1);
            Assert.IsTrue(_entry.HasTag(_tag1));
        }

        [TestMethod]
        public void AddTag_HasTag_String()
        {
            _entry.AddTag(_tag1.Alias);
            Assert.IsTrue(_entry.HasTag(_tag1.Alias));
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void AddTag_NotFound()
        {
            QdbTestCluster.CreateEmptyBlob().AddTag(_tag1);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void AddTag_OnSelf()
        {
            _entry.AddTag(_entry.Alias);
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void AddTag_QdgTag_Null()
        {
            _entry.AddTag((QdbTag)null);
        }

        [TestMethod]
        public void AddTag_RemoveTag_QdbTag()
        {
            _entry.AddTag(_tag1);
            Assert.IsTrue(_entry.RemoveTag(_tag1));
        }

        [TestMethod]
        public void AddTag_RemoveTag_String()
        {
            _entry.AddTag(_tag1.Alias);
            Assert.IsTrue(_entry.RemoveTag(_tag1.Alias));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddTag_String_Null()
        {
            _entry.AddTag((string)null);
        }

        [TestMethod]
        [ExpectedException(typeof (QdbAliasNotFoundException))]
        public void GetTags_NotFound()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            QdbTestCluster.CreateEmptyBlob().GetTags().Any();
        }

        [TestMethod]
        public void GetTags()
        {
            var tags = _entry.GetTags();
            Assert.IsFalse(tags.Any());
        }

        [TestMethod]
        public void HasTag()
        {
            Assert.IsFalse(_entry.HasTag(_tag1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HasTag_QdbTag_Null()
        {
            _entry.HasTag((QdbTag)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HasTag_String_Null()
        {
            _entry.HasTag((string)null);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void HasTag_NotFound()
        {
            QdbTestCluster.CreateEmptyBlob().HasTag(_tag1);
        }

        [TestMethod]
        public void RemoveTag()
        {
            Assert.IsFalse(_entry.RemoveTag(_tag1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveTag_QdbTag_Null()
        {
            _entry.RemoveTag((QdbTag)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveTag_String_Null()
        {
            _entry.RemoveTag((string)null);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void RemoveTag_Incompatible()
        {
            _entry.RemoveTag(QdbTestCluster.CreateBlob().Alias);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void RemoveTag_NotFound()
        {
            QdbTestCluster.CreateEmptyBlob().RemoveTag(_tag1);
        }
    }
}
