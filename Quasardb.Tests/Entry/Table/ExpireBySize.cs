using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.Table
{
    [TestClass]
    public class ExpireBySize
    {
        [TestMethod]
        public void ThrowsInvalidArgument_GivenNegativeSize()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.Instance.Table(alias);

            ts.Create();

            try
            {
                ts.ExpireBySize(-1);
                Assert.Fail("No exception thrown");
            }
            catch (QdbInvalidArgumentException)
            { }
        }

        // These tests are no longer valid
        // [TestMethod]
        // public void RemovesSingleEntry()
        // {
            // var ts = QdbTestCluster.Instance.Table(RandomGenerator.CreateUniqueAlias());
            // ts.Create(new QdbColumnDefinition[] { new QdbInt64ColumnDefinition("the_int64") });
            // var col = ts.Int64Columns["the_int64"];
            // col.Insert(new DateTime(2017, 1, 1), 1);
            // ts.ExpireBySize(1900);
            // Assert.AreEqual(0L, col.Count());
        // }
        // [TestMethod]
        // public void RemovesOlderEntry()
        // {
            // var points = new QdbInt64PointCollection
            // {
                // {new DateTime(2017, 1, 1), 1},
                // {new DateTime(2017, 1, 2), 2},
            // };
            // var ts = QdbTestCluster.Instance.Table(RandomGenerator.CreateUniqueAlias());
            // ts.Create(new QdbColumnDefinition[] { new QdbInt64ColumnDefinition("the_int64") });
            // var col = ts.Int64Columns["the_int64"];
            // col.Insert(points);
            // The below size limit must be greater than the size of the oldest bucket, but smaller than the size of 2 buckets.
            // ts.ExpireBySize(2000);
            // Assert.AreEqual(1L, col.Count());
            // CollectionAssert.AreEqual(points.Skip(1).ToList(), col.Points().ToList());
        // }
    }
}
