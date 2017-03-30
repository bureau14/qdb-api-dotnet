using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.TimeSeries.Double
{
    [TestClass]
    public class First
    {
        readonly QdbDoublePointCollection _points = new QdbDoublePointCollection
        {
            {new DateTime(2012, 11, 02), 666},
            {new DateTime(2014, 06, 30), 42 },
            {new DateTime(2016, 02, 04), 0}
        };

        [TestMethod]
        public void ThrowsColumnNotFound()
        {
            var col = QdbTestCluster.GetNonExistingDoubleColumn();

            try
            {
                col.First();
                Assert.Fail("No exception thrown");
            }
            catch (QdbColumnNotFoundException e)
            {
                Assert.AreEqual(col.Series.Alias, e.Alias);
                Assert.AreEqual(col.Name, e.Column);
            }
        }

        [TestMethod]
        public void GivenNoArgument_ReturnsFirstPointOfTimeSeries()
        {
            var col = QdbTestCluster.CreateEmptyDoubleColumn();
            col.Insert(_points);

            var result = col.First();

            Assert.AreEqual(_points[0], result);
        }

        [TestMethod]
        public void GivenInRangeInterval_ReturnsFirstPointOfInterval()
        {
            var col = QdbTestCluster.CreateEmptyDoubleColumn();
            col.Insert(_points);

            var interval = new QdbTimeInterval(_points[0].Time.AddHours(1), _points[2].Time);
            var result = col.First(interval);

            Assert.AreEqual(_points[1], result);
        }
        
        [TestMethod]
        public void GivenOutOfRangeInterval_ReturnsNull()
        {
            var col = QdbTestCluster.CreateEmptyDoubleColumn();
            col.Insert(_points);

            var interval = new QdbTimeInterval(new DateTime(3000, 1, 1), new DateTime(4000, 1, 1));
            var result = col.First(interval);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GivenSeveralIntervals_ReturnsFirstOfEach()
        {
            var col = QdbTestCluster.CreateEmptyDoubleColumn();
            col.Insert(_points);

            var intervals = new []
            {
                new QdbTimeInterval(new DateTime(2012, 1, 1), new DateTime(2016, 12, 31)),
                new QdbTimeInterval(new DateTime(2014, 1, 1), new DateTime(2017, 12, 31)),
                new QdbTimeInterval(new DateTime(2016, 6, 1), new DateTime(2018, 12, 31))
            };

            var results = col.First(intervals).ToArray();

            Assert.AreEqual(3, results.Length);
            Assert.AreEqual(_points[0], results[0]);
            Assert.AreEqual(_points[1], results[1]);
            Assert.IsNull(results[2]);
        }

        [TestMethod]
        public void WhenFirstIsNaN_ReturnsNonNull()
        {
            var col = QdbTestCluster.CreateEmptyDoubleColumn();
            var time = new DateTime(2012,5,12);
            col.Insert(time, double.NaN);

            var result = col.First();

            Assert.AreEqual(time, result.Time);
            Assert.IsTrue(double.IsNaN(result.Value));
        }
    }
}
