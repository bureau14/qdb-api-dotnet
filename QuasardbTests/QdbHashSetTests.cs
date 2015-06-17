using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;

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
            var cluster = new QdbCluster(DaemonRunner.ClusterUrl);
            var alias = Utils.CreateUniqueAlias();
            _hashSet = cluster.HashSet(alias);
            _content1 = Utils.CreateRandomContent();
            _content2 = Utils.CreateRandomContent();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Insert_Null()
        {
            _hashSet.Insert(null);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Erase_Null()
        {
            _hashSet.Contains(null);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Contains_Null()
        {
            _hashSet.Erase(null);
        }


        [TestMethod]
        public void Insert_Insert()
        {
            var result1 = _hashSet.Insert(_content1);
            var result2 = _hashSet.Insert(_content1);

            Assert.IsTrue(result1);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void Insert_Erase_Erase()
        {
            _hashSet.Insert(_content1);
            var result1 = _hashSet.Erase(_content1);
            var result2 = _hashSet.Erase(_content1);

            Assert.IsTrue(result1);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void Insert_Contains()
        {
            _hashSet.Insert(_content1);
            var result1 = _hashSet.Contains(_content1);
            var result2 = _hashSet.Contains(_content2);

            Assert.IsTrue(result1);
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void Insert_Erase_Contains()
        {
            _hashSet.Insert(_content1);
            _hashSet.Erase(_content1);
            var result = _hashSet.Contains(_content1);

            Assert.IsFalse(result);
        }
    }
}
