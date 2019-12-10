using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.TimeSeries.Blob
{
    [TestClass]
    public class InsertTruncate
    {
        public static QdbBlobColumn CreateColumn(string alias = null, string name = null)
        {
            var ts = QdbTestCluster.Instance.TimeSeries(alias ?? RandomGenerator.CreateUniqueAlias());
            var colName = name ?? RandomGenerator.CreateUniqueAlias();
            ts.Create(new QdbBlobColumnDefinition(colName));
            return ts.BlobColumns[colName];
        }

        [TestMethod]
        public void ThrowsInvalidArgument_GivenNoRanges()
        {
            var col = CreateColumn();
            var points = new QdbBlobPointCollection
            {
                {new DateTime(2000, 01, 01), new byte[]{1}},
                {new DateTime(2000, 01, 02), new byte[]{2}},
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
            var points = new QdbBlobPointCollection
            {
                {new DateTime(2000, 01, 01), new byte[]{1}},
                {new DateTime(2000, 01, 02), new byte[]{2}},
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
            var points = new QdbBlobPointCollection { };

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
            var oldPoints = new QdbBlobPointCollection
            {
                {new DateTime(2000, 01, 01), new byte[]{1}},
                {new DateTime(2000, 01, 02), new byte[]{2}},
            };
            var newPoints = new QdbBlobPointCollection
            {
                {new DateTime(2019, 01, 01), new byte[]{3}},
                {new DateTime(2019, 01, 02), new byte[]{4}},
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
