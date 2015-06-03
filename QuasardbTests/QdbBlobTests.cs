using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;

namespace QuasardbTests
{
    /// <summary>
    /// Summary description for QdbBlobTests
    /// </summary>
    [TestClass]
    public class QdbBlobTests
    {
        QdbBlob _blob;
        byte[] _content1, _content2;
        DateTime _expiry1, _expiry2;

        [TestInitialize]
        public void Initialize()
        {
            var cluster = new QdbCluster("qdb://127.0.0.1:2836");
            var alias = Utils.CreateUniqueAlias();
            _blob = cluster.Blob(alias);
            _content1 = Utils.CreateRandomContent();
            _content2 = Utils.CreateRandomContent();
            _expiry1 = new DateTime(3000, 12, 25);
            _expiry2 = new DateTime(4000, 12, 25);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasAlreadyExistsException))]
        public void Put_Put()
        {
            _blob.Put(_content1);
            _blob.Put(_content2);
        }

        [TestMethod]
        public void Put_Get()
        {
            _blob.Put(_content1);
            var result = _blob.Get();

            CollectionAssert.AreEqual(_content1, result);
        }

        [TestMethod]
        public void Put_GetExpiryTime()
        {
            _blob.Put(_content1, _expiry1);
            var result = _blob.GetExpiryTime();

            Assert.AreEqual(_expiry1, result);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void Get()
        {
            _blob.Get();
        }

        [TestMethod]
        public void Put_GetAndRemove_Put()
        {
            _blob.Put(_content1);
            var result = _blob.GetAndRemove();
            _blob.Put(_content2);

            CollectionAssert.AreEqual(_content1, result);
        }

        [TestMethod]
        public void Put_GetAndUpdate_Get()
        {
            _blob.Put(_content1);
            var result = _blob.GetAndUpdate(_content2);
            var finalContent = _blob.Get();

            CollectionAssert.AreEqual(result, _content1);
            CollectionAssert.AreEqual(_content2, finalContent);
        }

        [TestMethod]
        public void Put_GetAndUpdate_GetExpiryTime()
        {
            _blob.Put(_content1, _expiry1);
            var result = _blob.GetAndUpdate(_content2, _expiry2);
            var expiryTime = _blob.GetExpiryTime();

            CollectionAssert.AreEqual(result, _content1);
            Assert.AreEqual(_expiry2, expiryTime);
        }

        [TestMethod]
        public void Update_Update_Get()
        {
            _blob.Update(_content1);
            _blob.Update(_content2);
            var finalContent = _blob.Get();

            CollectionAssert.AreEqual(_content2, finalContent);
        }

        [TestMethod]
        public void Put_Update_GetExpiryTime()
        {
            _blob.Put(_content1, _expiry1);
            _blob.Update(_content2, _expiry2);
            var expiryTime = _blob.GetExpiryTime();
            
            Assert.AreEqual(_expiry2, expiryTime);
        }

        [TestMethod]
        public void Put_CompareAndSwap_Get__NonMatching()
        {
            _blob.Put(_content1);
            var result = _blob.CompareAndSwap(_content2, _content2);
            var finalContent = _blob.Get();

            CollectionAssert.AreEqual(_content1, result);
            CollectionAssert.AreEqual(_content1, finalContent);
        }

        [TestMethod]
        public void Put_CompareAndSwap_Get__Matching()
        {
            _blob.Put(_content1);
            var result = _blob.CompareAndSwap(_content2, _content1);
            var finalContent = _blob.Get();

            CollectionAssert.AreEqual(_content1, result);
            CollectionAssert.AreEqual(_content2, finalContent);
        }

        [TestMethod]
        public void Put_CompareAndSwap_GetExpiryTime()
        {
            _blob.Put(_content1, _expiry1);
            _blob.CompareAndSwap(_content2, _content1, _expiry2);
            var expiryTime = _blob.GetExpiryTime();

            Assert.AreEqual(_expiry2, expiryTime);
        }

        [TestMethod]
        public void Put_Remove_Put()
        {
            _blob.Put(_content1);
            _blob.Remove();
            _blob.Put(_content2);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void Put_Remove_Remove()
        {
            _blob.Put(_content1);
            _blob.Remove();
            _blob.Remove();
        }

        [TestMethod]
        public void Put_RemoveIf_Get()
        {
            _blob.Put(_content1);
            var result = _blob.RemoveIf(_content2);
            _blob.Get();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Put_RemoveIf_Put()
        {
            _blob.Put(_content1);
            var result = _blob.RemoveIf(_content1);
            _blob.Put(_content2);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Put_ExpiresAt_GetExpiryTime()
        {
            _blob.Put(_content1);
            _blob.ExpiresAt(_expiry1);
            var result = _blob.GetExpiryTime();

            Assert.AreEqual(_expiry1, result);
        }

        [TestMethod]
        public void Put_ExpiresFromNow_GetExpiryTime()
        {
            _blob.Put(_content1);
            _blob.ExpiresFromNow(TimeSpan.FromMinutes(10));
            var result = _blob.GetExpiryTime();

            Assert.IsTrue(result > DateTime.UtcNow.AddMinutes(5));
            Assert.IsTrue(result < DateTime.UtcNow.AddMinutes(15));
        }
    }
}
