using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.TimeSeries.Double
{
    [TestClass]
    public class Skewness
    {
        readonly QdbDoublePointCollection _points = new QdbDoublePointCollection
        {
            {new DateTime(2012, 11, 02).AddHours(1), 1},
            {new DateTime(2012, 11, 02).AddHours(2), 42},
            {new DateTime(2012, 11, 02).AddHours(3), 666},
            {new DateTime(2012, 11, 02).AddHours(4), 1234},
            {new DateTime(2012, 11, 02).AddHours(5), 5678}
        };

        [TestMethod]
        public void ThrowsColumnNotFound()
        {
            var col = QdbTestCluster.GetNonExistingDoubleColumn();

            try
            {
                col.Skewness();
                Assert.Fail("No exception thrown");
            }
            catch (QdbColumnNotFoundException e)
            {
                Assert.AreEqual(col.Series.Alias, e.Alias);
                Assert.AreEqual(col.Name, e.Column);
            }
        }

        [TestMethod]
        public void ThrowsEmptyColumn()
        {
            var col = QdbTestCluster.CreateEmptyDoubleColumn();

            try
            {
                col.Skewness();
                Assert.Fail("No exception thrown");
            }
            catch (QdbEmptyColumnException e)
            {
                Assert.AreEqual(col.Series.Alias, e.Alias);
                Assert.AreEqual(col.Name, e.Column);
            }
        }

        [TestMethod]
        public void GivenNoArgument_ReturnsSkewnessOfTimeSeries()
        {
            var col = QdbTestCluster.CreateEmptyDoubleColumn();
            col.Insert(_points);

            var result = col.Skewness();

            Assert.AreEqual(1.3373617909019264, result);
        }

        [TestMethod]
        public void GivenInRangeInterval_ReturnsSkewnessOfInterval()
        {
            var col = QdbTestCluster.CreateEmptyDoubleColumn();
            col.Insert(_points);

            var interval = new QdbTimeInterval(_points[0].Time, _points[4].Time);
            var result = col.Skewness(interval);

            Assert.AreEqual(0.43120395429883873, result);
        }

        [TestMethod]
        public void GivenOutOfRangeInterval_ReturnsNan()
        {
            var col = QdbTestCluster.CreateEmptyDoubleColumn();
            col.Insert(_points);

            var interval = new QdbTimeInterval(new DateTime(3000, 1, 1), new DateTime(4000, 1, 1));
            var result = col.Skewness(interval);

            Assert.IsTrue(double.IsNaN(result));
        }

        [TestMethod]
        public void GivenSeveralIntervals_ReturnsSkewnessOfEach()
        {
            var col = QdbTestCluster.CreateEmptyDoubleColumn();
            col.Insert(_points);

            var intervals = new[]
            {
                new QdbTimeInterval(new DateTime(2012, 11, 02).AddHours(1), new DateTime(2012, 11, 02).AddHours(4)),
                new QdbTimeInterval(new DateTime(2012, 11, 02).AddHours(2), new DateTime(2012, 11, 02).AddHours(5)),
                new QdbTimeInterval(new DateTime(2012, 11, 03), new DateTime(2012, 11, 04))
            };

            var results = col.Skewness(intervals).ToArray();

            Assert.AreEqual(3, results.Length);
            Assert.AreEqual(0.69749030004661439, results[0]);
            Assert.AreEqual(-0.057460816522297134, results[1]);
            Assert.IsTrue(double.IsNaN(results[2]));
        }
    }
}
