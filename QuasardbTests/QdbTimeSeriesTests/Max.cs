using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbTimeSeriesTests
{
    [TestClass]
    public class Max
    {
        readonly QdbTimeSeries.PointCollection _points = new QdbTimeSeries.PointCollection
        {
            {new DateTime(2012, 11, 02), 0},
            {new DateTime(2014, 06, 30), 42 },
            {new DateTime(2016, 02, 04), 666} // <- max is here
        };

        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var ts = QdbTestCluster.CreateEmptyTimeSeries();

            try
            {
                ts.First();
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(ts.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void GivenNoArgument_ReturnsMinPointOfTimeSeries()
        {
            var ts = QdbTestCluster.CreateEmptyTimeSeries();
            ts.Insert(_points);

            var result = ts.Max();

            Assert.AreEqual(_points[2], result);
        }

        [TestMethod]
        public void GivenInRangeInterval_ReturnsMinPointOfInterval()
        {
            var ts = QdbTestCluster.CreateEmptyTimeSeries();
            ts.Insert(_points);

            var interval = new QdbTimeInterval(_points[0].Time, _points[2].Time);
            var result = ts.Max(interval);

            Assert.AreEqual(_points[1], result);
        }

        [TestMethod]
        public void GivenOutOfRangeInterval_ReturnsNull()
        {
            var ts = QdbTestCluster.CreateEmptyTimeSeries();
            ts.Insert(_points);

            var interval = new QdbTimeInterval(new DateTime(3000, 1, 1), new DateTime(4000, 1, 1));
            var result = ts.Max(interval);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GivenSeveralIntervals_ReturnsMaxOfEach()
        {
            var ts = QdbTestCluster.CreateEmptyTimeSeries();
            ts.Insert(_points);

            var intervals = new[]
            {
                new QdbTimeInterval(new DateTime(2012, 1, 1), new DateTime(2015, 12, 31)),
                new QdbTimeInterval(new DateTime(2014, 1, 1), new DateTime(2017, 12, 31)),
                new QdbTimeInterval(new DateTime(2016, 6, 1), new DateTime(2018, 12, 31))
            };

            var results = ts.Max(intervals).ToArray();

            Assert.AreEqual(3, results.Length);
            Assert.AreEqual(_points[1], results[0]);
            Assert.AreEqual(_points[2], results[1]);
            Assert.IsNull(results[2]);
        }
    }
}
