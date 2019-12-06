using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.TimeSeries.Int64
{
    [TestClass]
    public class Erase
    {
        private QdbInt64Point[] _points =
            {
                new QdbInt64Point(new DateTime(2000, 01, 01), 1),
                new QdbInt64Point(new DateTime(2000, 01, 02), 2),
            };

        public QdbInt64Column CreateColumn(string alias = null, string name = null)
        {
            var ts = QdbTestCluster.Instance.TimeSeries(alias ?? RandomGenerator.CreateUniqueAlias());
            var colName = name ?? RandomGenerator.CreateUniqueAlias();
            ts.Create(new QdbInt64ColumnDefinition(colName));
            var col = ts.Int64Columns[colName];
            col.Insert(_points);
            return col;
        }

        [TestMethod]
        public void Ok_GivenNoRanges()
        {
            var col = CreateColumn();

            try
            {
                col.Erase(new QdbTimeInterval[0]);
            }
            catch (QdbInvalidArgumentException)
            {
                Assert.Fail("Erase with no ranges should not throw");
            }
        }

        [TestMethod]
        public void Ok_GivenEmptyRange()
        {
            var col = CreateColumn();

            try
            {
                col.Erase(new QdbTimeInterval());
            }
            catch (QdbInvalidArgumentException)
            {
                Assert.Fail("Erase with empty range should not throw");
            }
        }

        [TestMethod]
        public void Ok_GivenFullRange()
        {
            var col = CreateColumn();

            col.Erase(QdbTimeInterval.Everything);
            Assert.AreEqual(0L, col.Count());
        }

        [TestMethod]
        public void Ok_GivenRange()
        {
            var col = CreateColumn();

            col.Erase(new QdbTimeInterval(_points[0].Time, _points[1].Time));
            Assert.AreEqual(1L, col.Count());
            CollectionAssert.AreEqual(_points.Skip(1).ToList(), col.Points().ToList());
        }
    }
}
