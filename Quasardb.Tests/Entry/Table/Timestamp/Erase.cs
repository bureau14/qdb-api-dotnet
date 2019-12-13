using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.Table.Timestamp
{
    [TestClass]
    public class Erase
    {
        private QdbTimestampPoint[] _points =
            {
                new QdbTimestampPoint(new DateTime(2000, 01, 01), DateTime.Now.AddSeconds(1)),
                new QdbTimestampPoint(new DateTime(2000, 01, 02), DateTime.Now.AddSeconds(2)),
            };

        public QdbTimestampColumn CreateColumn(string alias = null, string name = null)
        {
            var ts = QdbTestCluster.Instance.Table(alias ?? RandomGenerator.CreateUniqueAlias());
            var colName = name ?? RandomGenerator.CreateUniqueAlias();
            ts.Create(new QdbTimestampColumnDefinition(colName));
            var col = ts.TimestampColumns[colName];
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
