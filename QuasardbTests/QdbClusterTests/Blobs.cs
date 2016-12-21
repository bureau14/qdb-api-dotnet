using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbClusterTests
{
    [TestClass]
    public class Blobs
    {
        private readonly QdbCluster _cluster = QdbTestCluster.Instance;

        [TestMethod]
        public void GivenPattern_ReturnsMatchingBlobs()
        {
            var aliases = new[] { RandomGenerator.CreateUniqueAlias(), RandomGenerator.CreateUniqueAlias() };
            var content = Encoding.UTF8.GetBytes("Hello world!!!!");
            foreach (var alias in aliases)
                _cluster.Blob(alias).Put(content);

            var results = _cluster.Blobs(new QdbPatternSelector("world", 100));

            CollectionAssert.AreEquivalent(aliases, results.Select(x => x.Alias).ToArray());
        }

        [TestMethod]
        public void GivenPattern_ReturnsEmptyCollection()
        {
            var results = _cluster.Blobs(new QdbPatternSelector("gros zizi", 100));
            Assert.AreEqual(0, results.Count());
        }

        [TestMethod]
        public void GivenRegex_ReturnsMatchingBlobs()
        {
            var aliases = new[] { RandomGenerator.CreateUniqueAlias(), RandomGenerator.CreateUniqueAlias() };
            var content = Encoding.UTF8.GetBytes("Pipi Caca Prout");
            foreach (var alias in aliases)
                _cluster.Blob(alias).Put(content);

            var results = _cluster.Blobs(new QdbRegexSelector(".* Caca .*", 100));

            CollectionAssert.AreEquivalent(aliases, results.Select(x => x.Alias).ToArray());
        }

        [TestMethod]
        public void GivenRegex_ReturnsEmptyCollection()
        {
            var results = _cluster.Blobs(new QdbRegexSelector("gros zizi", 100));
            Assert.AreEqual(0, results.Count());
        }
    }
}
