using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbTimeSeriesTests
{
    [TestClass]
    public class Sum
    {
        readonly QdbTimeSeries.PointCollection _points = new QdbTimeSeries.PointCollection
        {
            {new DateTime(2012, 11, 02), 0},
            {new DateTime(2014, 06, 30), 42 },
            {new DateTime(2016, 02, 04), 666}
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

            var result = ts.Sum();

            Assert.AreEqual(42+666, result);
        }

        [TestMethod]
        public void GivenInRangeInterval_ReturnsMinPointOfInterval()
        {
            var ts = QdbTestCluster.CreateEmptyTimeSeries();
            ts.Insert(_points);

            var result = ts.Sum(_points[0].Time, _points[2].Time);

            Assert.AreEqual(42, result);
        }

        [TestMethod]
        public void GivenOutOfRangeInterval_ReturnsNan()
        {
            var ts = QdbTestCluster.CreateEmptyTimeSeries();
            ts.Insert(_points);

            var result = ts.Sum(new DateTime(3000,1,1), new DateTime(4000, 1, 1));

            Assert.IsTrue(double.IsNaN(result));
        }
    }
}
