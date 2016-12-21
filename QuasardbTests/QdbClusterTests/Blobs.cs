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
        [TestMethod]
        public void SearchByPattern()
        {
            var cluster = QdbTestCluster.Instance;
            var aliases = new[] { RandomGenerator.CreateUniqueAlias(), RandomGenerator.CreateUniqueAlias() };
            var content = Encoding.UTF8.GetBytes("Hello world!!!!");
            foreach (var alias in aliases)
                cluster.Blob(alias).Put(content);

            var results = cluster.Blobs(new QdbPatternSelector("world", 100));

            CollectionAssert.AreEquivalent(aliases, results.Select(x => x.Alias).ToArray());
        }

        [TestMethod]
        public void SearchByRegex()
        {
            var cluster = QdbTestCluster.Instance;
            var aliases = new[] { RandomGenerator.CreateUniqueAlias(), RandomGenerator.CreateUniqueAlias() };
            var content = Encoding.UTF8.GetBytes("Pipi Caca Prout");
            foreach (var alias in aliases)
                cluster.Blob(alias).Put(content);

            var results = cluster.Blobs(new QdbRegexSelector(".* Caca .*", 100));

            CollectionAssert.AreEquivalent(aliases, results.Select(x => x.Alias).ToArray());
        }
    }
}
