using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.Table.Double
{
    [TestClass]
    public class Points
    {
        readonly QdbDoublePoint[] _points = new[]
        {
            new QdbDoublePoint(new DateTime(2012, 11, 02), 0),
            new QdbDoublePoint(new DateTime(2014, 06, 30), 42),
            new QdbDoublePoint(new DateTime(2016, 02, 04), 666),
            new QdbDoublePoint(new DateTime(2019, 12, 11), null)
        };

        [TestMethod]
        public void ThrowsColumnNotFound()
        {
            var col = QdbTestCluster.GetNonExistingDoubleColumn();

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
        public void GivenNoArgument_ReturnsPointsOfTable()
        {
            var ts = QdbTestCluster.CreateEmptyDoubleColumn();
            ts.Insert(_points);

            var result = ts.Points();

            CollectionAssert.AreEqual(_points.ToList(), result.ToList());
        }


        [TestMethod]
        public void GivenInRangeInterval_ReturnsPointsOfInterval()
        {
            var ts = QdbTestCluster.CreateEmptyDoubleColumn();
            ts.Insert(_points);

            var interval = new QdbTimeInterval(_points[0].Time, _points[2].Time);
            var result = ts.Points(interval);

            CollectionAssert.AreEqual(_points.Take(2).ToList(), result.ToList());
        }

        [TestMethod]
        public void GivenOutOfRangeInterval_ReturnsEmptyCollection()
        {
            var ts = QdbTestCluster.CreateEmptyDoubleColumn();
            ts.Insert(_points);

            var interval = new QdbTimeInterval(new DateTime(3000, 1, 1), new DateTime(4000, 1, 1));
            var result = ts.Points(interval);

            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void GivenSeveralIntervals_ReturnsPointsOfEach()
        {
            var ts = QdbTestCluster.CreateEmptyDoubleColumn();
            ts.Insert(_points);

            var intervals = new[]
            {
                new QdbTimeInterval(new DateTime(2014, 1, 1), new DateTime(2014, 12, 31)),
                new QdbTimeInterval(new DateTime(2016, 1, 1), new DateTime(2016, 12, 31)),
                new QdbTimeInterval(new DateTime(2019, 1, 1), new DateTime(2019, 12, 31)),
                new QdbTimeInterval(new DateTime(2028, 1, 1), new DateTime(2028, 12, 31))
            };

            var result = ts.Points(intervals);

            CollectionAssert.AreEqual(_points.Skip(1).ToList(), result.ToList());
        }
    }
}
