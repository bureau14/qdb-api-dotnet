using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.TimeSeries.Blob
{
    [TestClass]
    public class Count
    {
        readonly QdbBlobPoint[] _points =
        {
            new QdbBlobPoint(new DateTime(2012, 11, 02), RandomGenerator.CreateRandomContent()),
            new QdbBlobPoint(new DateTime(2014, 06, 30), RandomGenerator.CreateRandomContent()),
            new QdbBlobPoint(new DateTime(2016, 02, 04), RandomGenerator.CreateRandomContent())
        };

        [TestMethod]
        public void ThrowsColumnNotFound()
        {
            var col = QdbTestCluster.GetNonExistingBlobColumn();

            try
            {
                col.Count();
                Assert.Fail("No exception thrown");
            }
            catch (QdbColumnNotFoundException e)
            {
                Assert.AreEqual(col.Series.Alias, e.Alias);
                Assert.AreEqual(col.Name, e.Column);
            }
        }

        [TestMethod]
        public void GivenArgument_ReturnsNumberOfPointsInTimeSeries()
        {
            var col = QdbTestCluster.CreateEmptyBlobColumn();
            col.Insert(_points);

            var result = col.Count();

            Assert.AreEqual(3, result);
        }

        
        [TestMethod]
        public void GivenInRangeInterval_ReturnsNumberOfPointsInInterval()
        {
            var col = QdbTestCluster.CreateEmptyBlobColumn();
            col.Insert(_points);
        
            var interval = new QdbTimeInterval(_points[0].Time, _points[2].Time);
            var result = col.Count(interval);
        
            Assert.AreEqual(2, result);
        }
        
        [TestMethod]
        public void GivenOutOfRangeInterval_ReturnsZero()
        {
            var col = QdbTestCluster.CreateEmptyBlobColumn();
            col.Insert(_points);
        
            var interval = new QdbTimeInterval(new DateTime(3000, 1, 1), new DateTime(4000, 1, 1));
            var result = col.Count(interval);
        
            Assert.AreEqual(0, result);
        }
        
        [TestMethod]
        public void GivenSeveralIntervals_ReturnsCountOfEach()
        {
            var col = QdbTestCluster.CreateEmptyBlobColumn();
            col.Insert(_points);
        
            var intervals = new[]
            {
                new QdbTimeInterval(new DateTime(2012, 1, 1), new DateTime(2015, 12, 31)),
                new QdbTimeInterval(new DateTime(2014, 1, 1), new DateTime(2017, 12, 31)),
                new QdbTimeInterval(new DateTime(2016, 6, 1), new DateTime(2018, 12, 31))
            };
        
            var results = col.Count(intervals).ToArray();
        
            Assert.AreEqual(3, results.Length);
            Assert.AreEqual(2, results[0]);
            Assert.AreEqual(2, results[1]);
            Assert.AreEqual(0, results[2]);
        }
    }
}