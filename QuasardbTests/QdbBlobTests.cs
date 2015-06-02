using System;
using System.Text;
using System.Collections.Generic;
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

        [TestInitialize]
        public void Initialize()
        {
            var cluster = new QdbCluster("qdb://127.0.0.1:2836");
            var alias = Utils.CreateUniqueAlias();
            _blob = cluster.Blob(alias);
        }

        [TestMethod]
        [ExpectedException(typeof (QdbAliasAlreadyExistsException))]
        public void PutTwice()
        {
            _blob.Put(Utils.CreateRandomContent());
            _blob.Put(Utils.CreateRandomContent());
        }

        [TestMethod]
        public void PutThenGet()
        {
            var originalContent = Utils.CreateRandomContent();
            _blob.Put(originalContent);

            var retreivedContent = _blob.Get();
            CollectionAssert.AreEqual(originalContent, retreivedContent);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void GetNonExisting()
        {
            _blob.Get();
        }

        [TestMethod]
        public void GetAndRemove()
        {
            var originalContent = Utils.CreateRandomContent();
            _blob.Put(originalContent);

            var retreivedContent = _blob.GetAndRemove();
            CollectionAssert.AreEqual(originalContent, retreivedContent);

            _blob.Put(originalContent);
        }

        [TestMethod]
        public void UpdateThenGet()
        {
            var originalContent = Utils.CreateRandomContent();
            _blob.Update(originalContent);

            var retreivedContent = _blob.Get();
            CollectionAssert.AreEqual(originalContent, retreivedContent);
        }

        [TestMethod]
        public void UpdateTwice()
        {
            _blob.Update(Utils.CreateRandomContent());
            _blob.Update(Utils.CreateRandomContent());
        }
    }
}
