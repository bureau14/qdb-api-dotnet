using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbTimeSeriesTests
{
    [TestClass]
    public class Min
    {
        readonly QdbTimeSeries.PointCollection _points = new QdbTimeSeries.PointCollection
        {
            {new DateTime(2012, 11, 02), 666},
            {new DateTime(2014, 06, 30), 42 },
            {new DateTime(2016, 02, 04), 0} // <- min is here
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

            var result = ts.Min();

            Assert.AreEqual(_points[2], result);
        }

        [TestMethod]
        public void GivenInterval_ReturnsMinPointOfInterval()
        {
            var ts = QdbTestCluster.CreateEmptyTimeSeries();
            ts.Insert(_points);

            var result = ts.Min(_points[0].Time, _points[2].Time);

            Assert.AreEqual(_points[1], result);
        }
        
        [TestMethod]
        public void GivenOutOfRangeInterval_ReturnsNull()
        {
            var ts = QdbTestCluster.CreateEmptyTimeSeries();
            ts.Insert(_points);

            var result = ts.Min(new DateTime(3000, 1, 1), new DateTime(4000, 1, 1));

            Assert.IsNull(result);
        }
    }
}
