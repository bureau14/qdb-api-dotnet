using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.TimeSeries.Timestamp
{
    [TestClass]
    public class DistinctCount
    {
        readonly QdbTimestampPoint[] _points =
        {
            new QdbTimestampPoint(new DateTime(2012, 11, 02), new DateTime(2012, 11, 02)),
            new QdbTimestampPoint(new DateTime(2014, 06, 30), new DateTime(2014, 06, 30)),
            new QdbTimestampPoint(new DateTime(2016, 02, 04), new DateTime(2016, 02, 04)),
            new QdbTimestampPoint(new DateTime(2017, 06, 30), new DateTime(2014, 06, 30)),
            new QdbTimestampPoint(new DateTime(2018, 11, 02), new DateTime(2012, 11, 02))
        };

        [TestMethod]
        public void ThrowsColumnNotFound()
        {
            var col = QdbTestCluster.GetNonExistingTimestampColumn();

            try
            {
                col.DistinctCount();
                Assert.Fail("No exception thrown");
            }
            catch (QdbColumnNotFoundException e)
            {
                Assert.AreEqual(col.Series.Alias, e.Alias);
                Assert.AreEqual(col.Name, e.Column);
            }
        }

        [TestMethod]
        public void GivenArgument_ReturnsNumberOfPointsInTimeSeries()
        {
            var col = QdbTestCluster.CreateEmptyTimestampColumn();
            col.Insert(_points);

            var result = col.DistinctCount();

            Assert.AreEqual(3, result);
        }


        [TestMethod]
        public void GivenInRangeInterval_ReturnsNumberOfPointsInInterval()
        {
            var col = QdbTestCluster.CreateEmptyTimestampColumn();
            col.Insert(_points);

            var interval = new QdbTimeInterval(_points[0].Time, _points[2].Time);
            var result = col.DistinctCount(interval);

            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void GivenOutOfRangeInterval_ReturnsZero()
        {
            var col = QdbTestCluster.CreateEmptyTimestampColumn();
            col.Insert(_points);

            var interval = new QdbTimeInterval(new DateTime(3000, 1, 1), new DateTime(4000, 1, 1));
            var result = col.DistinctCount(interval);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void GivenSeveralIntervals_ReturnsDistinctCountOfEach()
        {
            var col = QdbTestCluster.CreateEmptyTimestampColumn();
            col.Insert(_points);

            var intervals = new[]
            {
                new QdbTimeInterval(new DateTime(2012, 1, 1), new DateTime(2015, 12, 31)),
                new QdbTimeInterval(new DateTime(2014, 1, 1), new DateTime(2017, 12, 31)),
                new QdbTimeInterval(new DateTime(2016, 6, 1), new DateTime(2018, 12, 31))
            };

            var results = col.DistinctCount(intervals).ToArray();

            Assert.AreEqual(3, results.Length);
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(2, results[1]);
            Assert.AreEqual(2, results[2]);
        }
    }
}