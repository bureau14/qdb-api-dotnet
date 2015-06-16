﻿using System;
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
        QdbEntry _blob1, _queue1, _hashSet1, _integer1, _tag1;
        
        [TestInitialize]
        public void Initialize()
        {
            _cluster = new QdbCluster(DaemonRunner.ClusterUrl);
            _tag = _cluster.Tag(Utils.CreateUniqueAlias());
            _blob1 = _cluster.Blob(Utils.CreateUniqueAlias()).Put(Utils.CreateRandomContent());
            _queue1 = _cluster.Queue(Utils.CreateUniqueAlias()).PushBack(Utils.CreateRandomContent());
            _hashSet1 = _cluster.HashSet(Utils.CreateUniqueAlias()).Insert(Utils.CreateRandomContent());
            _integer1 = _cluster.Integer(Utils.CreateUniqueAlias()).Put(42);
            _tag1 = _cluster.Tag(Utils.CreateUniqueAlias()).AddEntry(_blob1);
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
            _blob1.AddTag(_tag);

            var blob = _tag.GetEntries().Cast<QdbBlob>().Single();

            Assert.AreEqual(_blob1.Alias, blob.Alias);
        }

        [TestMethod]
        public void OneQueue()
        {
            _queue1.AddTag(_tag);

            var queue = _tag.GetEntries().Cast<QdbQueue>().Single();

            Assert.AreEqual(_queue1.Alias, queue.Alias);
        }

        [TestMethod]
        public void OneHashSet()
        {
            _hashSet1.AddTag(_tag);

            var hashSet = _tag.GetEntries().Cast<QdbHashSet>().Single();

            Assert.AreEqual(_hashSet1.Alias, hashSet.Alias);
        }

        [TestMethod]
        public void OneInteger()
        {
            _integer1.AddTag(_tag);

            var integer = _tag.GetEntries().Cast<QdbInteger>().Single();

            Assert.AreEqual(_integer1.Alias, integer.Alias);
        }

        [TestMethod]
        public void OneTag()
        {
            _tag1.AddTag(_tag);

            var tag = _tag.GetEntries().Cast<QdbTag>().Single();

            Assert.AreEqual(_tag1.Alias, tag.Alias);
        }
    }
}
