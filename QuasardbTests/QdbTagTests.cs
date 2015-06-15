using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;

namespace QuasardbTests
{
    [TestClass]
    public class QdbTagTests
    {
        QdbCluster _cluster;
        QdbTag _tag;

        [TestInitialize]
        public void Initialize()
        {
            _cluster = new QdbCluster(DaemonRunner.ClusterUrl);
            _tag = _cluster.Tag(Utils.CreateUniqueAlias());
        }

        [TestMethod]
        public void Empty()
        {
            var entries = _tag.GetEntries();
            Assert.IsFalse(entries.Any());
        }

        [TestMethod]
        public void OneBlob()
        {
            var alias = Utils.CreateUniqueAlias();
            _cluster.Blob(alias).Put(Utils.CreateRandomContent()).AddTag(_tag);

            var entries = _tag.GetEntries();
            var blob = (QdbBlob)entries.Single();

            Assert.AreEqual(alias, blob.Alias);
        }
    }
}
