using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.TimeSeries.Int64
{
    [TestClass]
    public class Points
    {
        readonly QdbInt64Point[] _points = new[]
        {
            new QdbInt64Point(new DateTime(2012, 11, 02), 0),
            new QdbInt64Point(new DateTime(2014, 06, 30), 42 ),
            new QdbInt64Point(new DateTime(2016, 02, 04), 666)
        };

        [TestMethod]
        public void ThrowsColumnNotFound()
        {
            var col = QdbTestCluster.GetNonExistingInt64Column();

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
        public void GivenNoArgument_ReturnsPointsOfTimeSeries()
        {
            var ts = QdbTestCluster.CreateEmptyInt64Column();
            ts.Insert(_points);

            var result = ts.Points();

            CollectionAssert.AreEqual(_points.ToList(), result.ToList());
        }


        [TestMethod]
        public void GivenInRangeInterval_ReturnsPointsOfInterval()
        {
            var ts = QdbTestCluster.CreateEmptyInt64Column();
            ts.Insert(_points);

            var interval = new QdbTimeInterval(_points[0].Time, _points[2].Time);
            var result = ts.Points(interval);

            CollectionAssert.AreEqual(_points.Take(2).ToList(), result.ToList());
        }

        [TestMethod]
        public void GivenOutOfRangeInterval_ReturnsEmptyCollection()
        {
            var ts = QdbTestCluster.CreateEmptyInt64Column();
            ts.Insert(_points);

            var interval = new QdbTimeInterval(new DateTime(3000, 1, 1), new DateTime(4000, 1, 1));
            var result = ts.Points(interval);

            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void GivenSeveralIntervals_ReturnsPointsOfEach()
        {
            var ts = QdbTestCluster.CreateEmptyInt64Column();
            ts.Insert(_points);

            var intervals = new[]
            {
                new QdbTimeInterval(new DateTime(2014, 1, 1), new DateTime(2014, 12, 31)),
                new QdbTimeInterval(new DateTime(2016, 1, 1), new DateTime(2016, 12, 31)),
                new QdbTimeInterval(new DateTime(2018, 1, 1), new DateTime(2018, 12, 31))
            };

            var result = ts.Points(intervals);

            CollectionAssert.AreEqual(_points.Skip(1).ToList(), result.ToList());
        }
    }
}
