using System;
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
        public void ReturnsDeque()
        {
            var alias = RandomGenerator.CreateUniqueAlias();

            QdbTestCluster.CreateQueue(alias);
            var result = _cluster.Entry(alias);

            Assert.IsInstanceOfType(result, typeof(QdbDeque));
            Assert.AreEqual(alias, result.Alias);
        }

        [TestMethod]
        public void ReturnsHashSet()
        {
            var alias = RandomGenerator.CreateUniqueAlias();

            QdbTestCluster.CreateHashSet(alias);
            var result = _cluster.Entry(alias);

            Assert.IsInstanceOfType(result, typeof(QdbHashSet));
            Assert.AreEqual(alias, result.Alias);
        }

        [TestMethod]
        public void ReturnsStream()
        {
            var alias = RandomGenerator.CreateUniqueAlias();

            QdbTestCluster.CreateStream(alias);
            var result = _cluster.Entry(alias);

            Assert.IsInstanceOfType(result, typeof(QdbStream));
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
        [Ignore] // Requires support for columns in qdb_api.dll
        public void ReturnsTimeSeries()
        {
            var alias = RandomGenerator.CreateUniqueAlias();

            QdbTestCluster.CreateDoubleColumn(alias);
            var result = _cluster.Entry(alias);

            Assert.IsInstanceOfType(result, typeof(QdbTimeSeries));
            Assert.AreEqual(alias, result.Alias);
        }
    }
}
