using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbTimeSeriesTests
{
    [TestClass]
    public class Count
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
                ts.Count();
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(ts.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void GivenArgument_ReturnsNumberOfPointsInTimeSeries()
        {
            var ts = QdbTestCluster.CreateEmptyTimeSeries();
            ts.Insert(_points);

            var result = ts.Count();

            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void GivenInRangeInterval_ReturnsNumberOfPointsInInterval()
        {
            var ts = QdbTestCluster.CreateEmptyTimeSeries();
            ts.Insert(_points);

            var result = ts.Count(_points[0].Time, _points[2].Time);

            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void GivenOutOfRangeInterval_ReturnsZero()
        {
            var ts = QdbTestCluster.CreateEmptyTimeSeries();
            ts.Insert(_points);

            var result = ts.Count(new DateTime(3000, 1, 1), new DateTime(4000, 1, 1));

            Assert.AreEqual(0, result);
        }
    }
}
