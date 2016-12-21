using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbClusterTests
{
    [TestClass]
    public class Entries
    {
        [TestMethod]
        public void SearchByPrefix()
        {
            var prefix = RandomGenerator.CreateUniqueAlias();
            var aliases = new [] {prefix + "AAAA", prefix + "BBBB"};
            foreach (var alias in aliases)
                QdbTestCluster.CreateBlob(alias);

            var results = QdbTestCluster.Instance.Entries(new QdbPrefixSelector(prefix, 10));

            CollectionAssert.AreEqual(aliases, results.Select(x => x.Alias).ToArray());
        }

        [TestMethod]
        public void SearchBySuffix()
        {
            var suffix = RandomGenerator.CreateUniqueAlias();
            var aliases = new[] { "AAAA" + suffix, "BBBB" + suffix };
            foreach (var alias in aliases)
                QdbTestCluster.CreateBlob(alias);

            var results = QdbTestCluster.Instance.Entries(new QdbSuffixSelector(suffix, 10));

            CollectionAssert.AreEqual(aliases, results.Select(x => x.Alias).ToArray());
        }

        [TestMethod]
        public void SearchByTag()
        {
            var tag = RandomGenerator.CreateUniqueAlias();
            var suffix = RandomGenerator.CreateUniqueAlias();
            var aliases = new[] { "AAAA" + suffix, "AAAB" + suffix };
            foreach (var alias in aliases)
                QdbTestCluster.CreateBlob(alias).AttachTag(tag);

            var results = QdbTestCluster.Instance.Entries(new QdbTagSelector(tag));

            CollectionAssert.AreEquivalent(aliases, results.Select(x => x.Alias).ToArray());
        }
    }
}
