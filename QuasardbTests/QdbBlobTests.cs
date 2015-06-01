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
        public void BlobIsNotNull()
        {
            Assert.IsNotNull(_blob);
        }

        [TestMethod]
        public void PutOnce()
        {
            _blob.Put(Utils.CreateRandomContent());
        }

        [TestMethod]
        [ExpectedException(typeof (QdbAliasAlreadyExistsException))]
        public void PutTwice()
        {
            _blob.Put(Utils.CreateRandomContent());
            _blob.Put(Utils.CreateRandomContent());
        }
    }
}
