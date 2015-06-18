using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests
{
    [TestClass]
    public class QdbHashSetTests
    {
        QdbHashSet _hashSet;
        byte[] _content1, _content2;

        [TestInitialize]
        public void Initialize()
        {
            _hashSet = QdbTestCluster.CreateEmptyHashSet();
            _content1 = RandomGenerator.CreateRandomContent();
            _content2 = RandomGenerator.CreateRandomContent();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Contains_Null()
        {
            _hashSet.Contains(null);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void Contains_Incompatible()
        {
            QdbTestCluster.CreateBlob(_hashSet.Alias);
            _hashSet.Contains(RandomGenerator.CreateRandomContent());
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void Contains_NotFound()
        {
            _hashSet.Contains(RandomGenerator.CreateRandomContent());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Erase_Null()
        {
            _hashSet.Erase(null);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void Erase_NotFound()
        {
            _hashSet.Erase(RandomGenerator.CreateRandomContent());
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void Erase_Incompatible()
        {
            QdbTestCluster.CreateBlob(_hashSet.Alias);
            _hashSet.Erase(RandomGenerator.CreateRandomContent());
        }

        [TestMethod]
        public void Insert_Contains()
        {
            _hashSet.Insert(_content1);
            Assert.IsTrue(_hashSet.Contains(_content1));
            Assert.IsFalse(_hashSet.Contains(_content2));
        }

        [TestMethod]
        public void Insert_Erase_Contains()
        {
            _hashSet.Insert(_content1);
            _hashSet.Erase(_content1);
            Assert.IsFalse(_hashSet.Contains(_content1));
        }

        [TestMethod]
        public void Insert_Erase_Erase()
        {
            _hashSet.Insert(_content1);
            Assert.IsTrue(_hashSet.Erase(_content1));
            Assert.IsFalse(_hashSet.Erase(_content1));
        }

        [TestMethod]
        public void Insert_Insert()
        {
            Assert.IsTrue(_hashSet.Insert(_content1));
            Assert.IsFalse(_hashSet.Insert(_content1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Insert_Null()
        {
            _hashSet.Insert(null);
        }
    }
}
