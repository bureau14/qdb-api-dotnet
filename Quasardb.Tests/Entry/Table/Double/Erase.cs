using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.Table.Double
{
    [TestClass]
    public class Erase
    {
        private QdbDoublePoint[] _points =
            {
                new QdbDoublePoint(new DateTime(2000, 01, 01), 1),
                new QdbDoublePoint(new DateTime(2000, 01, 02), 2),
            };

        public QdbDoubleColumn CreateColumn(string alias = null, string name = null)
        {
            var ts = QdbTestCluster.Instance.Table(alias ?? RandomGenerator.CreateUniqueAlias());
            var colName = name ?? RandomGenerator.CreateUniqueAlias();
            ts.Create(new QdbDoubleColumnDefinition(colName));
            var col = ts.DoubleColumns[colName];
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
