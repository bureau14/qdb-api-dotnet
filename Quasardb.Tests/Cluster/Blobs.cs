using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quasardb.Tests.Cluster
{
    [TestClass]
    public class Blobs
    {
        private readonly QdbCluster _cluster = QdbTestCluster.Instance;

        [TestMethod]
        public void GivenPattern_ReturnsMatchingBlobs()
        {
            var pattern = RandomGenerator.CreateUniqueAlias();
            var aliases = new[] { RandomGenerator.CreateUniqueAlias(), RandomGenerator.CreateUniqueAlias() };
            var content = Encoding.UTF8.GetBytes($"Hello {pattern} world!!!!");
            foreach (var alias in aliases)
                _cluster.Blob(alias).Put(content);

            var results = _cluster.Blobs(new QdbPatternSelector(pattern, 100));

            CollectionAssert.AreEquivalent(aliases, results.Select(x => x.Alias).ToArray());
        }

        [TestMethod]
        public void GivenPattern_ReturnsEmptyCollection()
        {
            var pattern = RandomGenerator.CreateUniqueAlias();
            var results = _cluster.Blobs(new QdbPatternSelector(pattern, 100));
            Assert.AreEqual(0, results.Count());
        }

        [TestMethod]
        public void GivenRegex_ReturnsMatchingBlobs()
        {
            var pattern = RandomGenerator.CreateUniqueAlias();
            var aliases = new[] { RandomGenerator.CreateUniqueAlias(), RandomGenerator.CreateUniqueAlias() };
            var content = Encoding.UTF8.GetBytes($"Pipi {pattern} Prout");
            foreach (var alias in aliases)
                _cluster.Blob(alias).Put(content);

            var results = _cluster.Blobs(new QdbRegexSelector($".*{pattern}.*", 100));

            CollectionAssert.AreEquivalent(aliases, results.Select(x => x.Alias).ToArray());
        }

        [TestMethod]
        public void GivenRegex_ReturnsEmptyCollection()
        {
            var pattern = RandomGenerator.CreateUniqueAlias();
            var results = _cluster.Blobs(new QdbRegexSelector(pattern, 100));
            Assert.AreEqual(0, results.Count());
        }
    }
}
