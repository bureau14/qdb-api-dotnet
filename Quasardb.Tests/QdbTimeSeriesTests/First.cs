using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.Tests.Helpers;

namespace Quasardb.Tests.QdbTimeSeriesTests
{
    [TestClass]
    public class First
    {
        readonly QdbTimeSeries.PointCollection _points = new QdbTimeSeries.PointCollection
        {
            {new DateTime(2012, 11, 02), 666},
            {new DateTime(2014, 06, 30), 42 },
            {new DateTime(2016, 02, 04), 0}
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
        public void GivenNoArgument_ReturnsFirstPointOfTimeSeries()
        {
            var ts = QdbTestCluster.CreateEmptyTimeSeries();
            ts.Insert(_points);

            var result = ts.First();

            Assert.AreEqual(_points[0], result);
        }

        [TestMethod]
        public void GivenInRangeInterval_ReturnsFirstPointOfInterval()
        {
            var ts = QdbTestCluster.CreateEmptyTimeSeries();
            ts.Insert(_points);

            var interval = new QdbTimeInterval(_points[0].Time.AddHours(1), _points[2].Time);
            var result = ts.First(interval);

            Assert.AreEqual(_points[1], result);
        }
        
        [TestMethod]
        public void GivenOutOfRangeInterval_ReturnsNull()
        {
            var ts = QdbTestCluster.CreateEmptyTimeSeries();
            ts.Insert(_points);

            var interval = new QdbTimeInterval(new DateTime(3000, 1, 1), new DateTime(4000, 1, 1));
            var result = ts.First(interval);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GivenSeveralIntervals_ReturnsFirstOfEach()
        {
            var ts = QdbTestCluster.CreateEmptyTimeSeries();
            ts.Insert(_points);

            var intervals = new []
            {
                new QdbTimeInterval(new DateTime(2012, 1, 1), new DateTime(2016, 12, 31)),
                new QdbTimeInterval(new DateTime(2014, 1, 1), new DateTime(2017, 12, 31)),
                new QdbTimeInterval(new DateTime(2016, 6, 1), new DateTime(2018, 12, 31))
            };

            var results = ts.First(intervals).ToArray();

            Assert.AreEqual(3, results.Length);
            Assert.AreEqual(_points[0], results[0]);
            Assert.AreEqual(_points[1], results[1]);
            Assert.IsNull(results[2]);
        }
    }
}
