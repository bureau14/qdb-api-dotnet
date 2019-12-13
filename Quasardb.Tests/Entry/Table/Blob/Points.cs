using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.Table.Blob
{
    [TestClass]
    public class Points
    {
        readonly QdbBlobPoint[] _points =
        {
            new QdbBlobPoint(new DateTime(2012, 11, 02), Encoding.UTF8.GetBytes("Hello World!")),
            new QdbBlobPoint(new DateTime(2014, 06, 30), RandomGenerator.CreateRandomContent()),
            new QdbBlobPoint(new DateTime(2016, 02, 04), RandomGenerator.CreateRandomContent())
        };

        [TestMethod]
        public void ThrowsColumnNotFound()
        {
            var col = QdbTestCluster.GetNonExistingBlobColumn();

            try
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                col.Points().ToArray();
                Assert.Fail("No exception thrown");
            }
            catch (QdbColumnNotFoundException e)
            {
                Assert.AreEqual(col.Series.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void GivenDoubleColumn_ThrowsIncompatibleType()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var name = "hello";
            QdbTestCluster.CreateEmptyDoubleColumn(alias, name);
            var col = QdbTestCluster.Instance.Table(alias).BlobColumns[name];

            try
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                col.Points().ToArray();
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(col.Series.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void GivenNoArgument_ReturnsPointsOfTable()
        {
            var col = QdbTestCluster.CreateEmptyBlobColumn();
            col.Insert(_points);

            var result = col.Points();

            CollectionAssert.AreEqual(_points.ToList(), result.ToList());
        }

        [TestMethod]
        public void GivenInRangeInterval_ReturnsPointsOfInterval()
        {
            var col = QdbTestCluster.CreateEmptyBlobColumn();
            col.Insert(_points);

            var interval = new QdbTimeInterval(_points[0].Time, _points[2].Time);
            var result = col.Points(interval);

            CollectionAssert.AreEqual(_points.Take(2).ToList(), result.ToList());
        }

        [TestMethod]
        public void GivenOutOfRangeInterval_ReturnsEmptyCollection()
        {
            var col = QdbTestCluster.CreateEmptyBlobColumn();
            col.Insert(_points);

            var interval = new QdbTimeInterval(new DateTime(3000, 1, 1), new DateTime(4000, 1, 1));
            var result = col.Points(interval);

            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void GivenSeveralIntervals_ReturnsPointsOfEach()
        {
            var col = QdbTestCluster.CreateEmptyBlobColumn();
            col.Insert(_points);

            var intervals = new[]
            {
                new QdbTimeInterval(new DateTime(2014, 1, 1), new DateTime(2014, 12, 31)),
                new QdbTimeInterval(new DateTime(2016, 1, 1), new DateTime(2016, 12, 31)),
                new QdbTimeInterval(new DateTime(2018, 1, 1), new DateTime(2018, 12, 31))
            };

            var result = col.Points(intervals);

            CollectionAssert.AreEqual(_points.Skip(1).ToList(), result.ToList());
        }
    }
}
