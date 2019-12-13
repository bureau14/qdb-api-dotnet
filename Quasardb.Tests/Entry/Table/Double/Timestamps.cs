using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.Table.Double
{
    [TestClass]
    public class Timestamps
    {
        readonly QdbDoublePoint[] _points = new[]
        {
            new QdbDoublePoint(new DateTime(2012, 11, 02), 0),
            new QdbDoublePoint(new DateTime(2014, 06, 30), 42),
            new QdbDoublePoint(new DateTime(2016, 02, 04), 666)
        };

        [TestMethod]
        public void GivenNoArgument_ReturnsTimestampsOfTable()
        {
            var ts = QdbTestCluster.CreateEmptyDoubleColumn();
            ts.Insert(_points);

            var result = ts.Timestamps();

            CollectionAssert.AreEqual(_points.Select(x => x.Time).ToList(), result.ToList());
        }


        [TestMethod]
        public void GivenInRangeInterval_ReturnsTimestampsOfInterval()
        {
            var ts = QdbTestCluster.CreateEmptyDoubleColumn();
            ts.Insert(_points);

            var interval = new QdbTimeInterval(_points[0].Time, _points[2].Time);
            var result = ts.Timestamps(interval);

            CollectionAssert.AreEqual(_points.Take(2).Select(x => x.Time).ToList(), result.ToList());
        }

        [TestMethod]
        public void GivenOutOfRangeInterval_ReturnsEmptyCollection()
        {
            var ts = QdbTestCluster.CreateEmptyDoubleColumn();
            ts.Insert(_points);

            var interval = new QdbTimeInterval(new DateTime(3000, 1, 1), new DateTime(4000, 1, 1));
            var result = ts.Timestamps(interval);

            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void GivenSeveralIntervals_ReturnsTimestampsOfEach()
        {
            var ts = QdbTestCluster.CreateEmptyDoubleColumn();
            ts.Insert(_points);

            var intervals = new[]
            {
                new QdbTimeInterval(new DateTime(2014, 1, 1), new DateTime(2014, 12, 31)),
                new QdbTimeInterval(new DateTime(2016, 1, 1), new DateTime(2016, 12, 31)),
                new QdbTimeInterval(new DateTime(2018, 1, 1), new DateTime(2018, 12, 31))
            };

            var result = ts.Timestamps(intervals);

            CollectionAssert.AreEqual(_points.Skip(1).Select(x => x.Time).ToList(), result.ToList());
        }
    }
}
