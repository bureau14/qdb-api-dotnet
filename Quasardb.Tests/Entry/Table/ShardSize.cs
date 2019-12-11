using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.Table
{
    [TestClass]
    public class ShardSize
    {
        [TestMethod]
        public void ReturnsCreationShardSize()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.Instance.Table(alias);

            var shardSize = TimeSpan.FromHours(5);
            ts.Create(shardSize);
            Assert.AreEqual(shardSize, ts.ShardSize);
        }
    }
}
