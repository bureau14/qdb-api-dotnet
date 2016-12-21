using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbClusterTests
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
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void ThrowsAliasNotFound()
        {
            var alias = RandomGenerator.CreateUniqueAlias();

            _cluster.Entry(alias); // <- throws QdbAliasNotFoundException
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
    }
}
