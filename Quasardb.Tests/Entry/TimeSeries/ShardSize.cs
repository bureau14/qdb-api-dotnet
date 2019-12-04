using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.TimeSeries
{
    [TestClass]
    public class ShardSize
    {
        [TestMethod]
        public void ReturnsCreationShardSize()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.Instance.TimeSeries(alias);

            var shardSize = TimeSpan.FromHours(5);
            ts.Create(shardSize);
            Assert.AreEqual(shardSize, ts.ShardSize);
        }
    }
}
