using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.TimeSeries.Int64
{
    [TestClass]
    public class InsertTruncate
    {
        public static QdbInt64Column CreateColumn(string alias = null, string name = null)
        {
            var ts = QdbTestCluster.Instance.TimeSeries(alias ?? RandomGenerator.CreateUniqueAlias());
            var colName = name ?? RandomGenerator.CreateUniqueAlias();
            ts.Create(new QdbInt64ColumnDefinition(colName));
            return ts.Int64Columns[colName];
        }

        [TestMethod]
        public void ThrowsInvalidArgument_GivenNoRanges()
        {
            var col = CreateColumn();
            var points = new QdbInt64PointCollection
            {
                {new DateTime(2000, 01, 01), 1},
                {new DateTime(2000, 01, 02), 2},
            };

            try
            {
                col.InsertTruncate(new QdbTimeInterval[0], points);
                Assert.Fail("No exception thrown");
            }
            catch (QdbInvalidArgumentException)
            { }
        }

        [TestMethod]
        public void ThrowsInvalidArgument_GivenEmptyRange()
        {
            var col = CreateColumn();
            var points = new QdbInt64PointCollection
            {
                {new DateTime(2000, 01, 01), 1},
                {new DateTime(2000, 01, 02), 2},
            };

            try
            {
                col.InsertTruncate(new QdbTimeInterval(), points);
                Assert.Fail("No exception thrown");
            }
            catch (QdbInvalidArgumentException)
            { }
        }

        [TestMethod]
        public void Ok_GivenEmptyPoints()
        {
            var col = CreateColumn();
            var points = new QdbInt64PointCollection { };

            try
            {
                col.InsertTruncate(QdbTimeInterval.Everything, points);
                Assert.AreEqual(0L, col.Count());
            }
            catch (QdbInvalidArgumentException)
            {
                Assert.Fail("InsertTruncate with empty points should not throw");
            }
        }

        [TestMethod]
        public void Ok_GivenFullRange()
        {
            var col = CreateColumn();
            var oldPoints = new QdbInt64PointCollection
            {
                {new DateTime(2000, 01, 01), 1},
                {new DateTime(2000, 01, 02), 2},
            };
            var newPoints = new QdbInt64PointCollection
            {
                {new DateTime(2019, 01, 01), 3},
                {new DateTime(2019, 01, 02), 4},
            };

            col.Insert(oldPoints);
            col.InsertTruncate(QdbTimeInterval.Everything, newPoints);
            Assert.AreEqual(2L, col.Count());

            int i = 0;
            foreach (var point in col.Points())
            {
                Assert.AreEqual(newPoints[i++], point);
            }
        }
    }
}
