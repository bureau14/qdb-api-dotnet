﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Cluster
{
    [TestClass]
    public class Entry
    {
        private readonly QdbCluster _cluster = QdbTestCluster.Instance;

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentsNull()
        {
            _cluster.Entry(null); // <- throws ArgumentNullException
        }

        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var alias = RandomGenerator.CreateUniqueAlias();

            try
            {
                _cluster.Entry(alias);
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(alias, e.Alias);
            }
        }

        [TestMethod]
        public void ReturnsBlob()
        {
            var alias = RandomGenerator.CreateUniqueAlias();

            QdbTestCluster.CreateBlob(alias);
            var result = _cluster.Entry(alias);

            Assert.IsInstanceOfType(result, typeof(QdbBlob));
            Assert.AreEqual(alias, result.Alias);
        }

        [TestMethod]
        public void ReturnsInteger()
        {
            var alias = RandomGenerator.CreateUniqueAlias();

            QdbTestCluster.CreateInteger(alias);
            var result = _cluster.Entry(alias);

            Assert.IsInstanceOfType(result, typeof(QdbInteger));
            Assert.AreEqual(alias, result.Alias);
        }

        [TestMethod]
        public void ReturnsTag()
        {
            var alias = RandomGenerator.CreateUniqueAlias();

            QdbTestCluster.CreateTag(alias);
            var result = _cluster.Entry(alias);

            Assert.IsInstanceOfType(result, typeof(QdbTag));
            Assert.AreEqual(alias, result.Alias);
        }


        [TestMethod]
        public void ReturnsTable()
        {
            var alias = RandomGenerator.CreateUniqueAlias();

            QdbTestCluster.Instance.Table(alias).Create(new QdbBlobColumnDefinition("hello"));
            var result = _cluster.Entry(alias);

            Assert.IsInstanceOfType(result, typeof(QdbTable));
            Assert.AreEqual(alias, result.Alias);
        }
    }
}
