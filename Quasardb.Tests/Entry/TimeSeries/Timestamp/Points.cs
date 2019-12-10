using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.TimeSeries.Timestamp
{
    [TestClass]
    public class Points
    {
        readonly QdbTimestampPoint[] _points =
        {
            new QdbTimestampPoint(new DateTime(2012, 11, 02), new DateTime(2012, 11, 02)),
            new QdbTimestampPoint(new DateTime(2014, 06, 30), new DateTime(2014, 06, 30)),
            new QdbTimestampPoint(new DateTime(2016, 02, 04), new DateTime(2016, 02, 04))
        };

        [TestMethod]
        public void ThrowsColumnNotFound()
        {
            var col = QdbTestCluster.GetNonExistingTimestampColumn();

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
            var col = QdbTestCluster.Instance.TimeSeries(alias).TimestampColumns[name];

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
        public void GivenNoArgument_ReturnsPointsOfTimeSeries()
        {
            var col = QdbTestCluster.CreateEmptyTimestampColumn();
            col.Insert(_points);

            var result = col.Points();

            CollectionAssert.AreEqual(_points.ToList(), result.ToList());
        }

        [TestMethod]
        public void GivenInRangeInterval_ReturnsPointsOfInterval()
        {
            var col = QdbTestCluster.CreateEmptyTimestampColumn();
            col.Insert(_points);

            var interval = new QdbTimeInterval(_points[0].Time, _points[2].Time);
            var result = col.Points(interval);

            CollectionAssert.AreEqual(_points.Take(2).ToList(), result.ToList());
        }

        [TestMethod]
        public void GivenOutOfRangeInterval_ReturnsEmptyCollection()
        {
            var col = QdbTestCluster.CreateEmptyTimestampColumn();
            col.Insert(_points);

            var interval = new QdbTimeInterval(new DateTime(3000, 1, 1), new DateTime(4000, 1, 1));
            var result = col.Points(interval);

            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void GivenSeveralIntervals_ReturnsPointsOfEach()
        {
            var col = QdbTestCluster.CreateEmptyTimestampColumn();
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
