﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.Table.Timestamp
{
    [TestClass]
    public class AbsMin
    {
        readonly QdbTimestampPointCollection _points = new QdbTimestampPointCollection
        {
            {new DateTime(2012, 11, 02), DateTime.Now.AddSeconds(666)},
            {new DateTime(2014, 06, 30), DateTime.Now.AddSeconds(42)},
            {new DateTime(2016, 02, 04), DateTime.Now.AddSeconds(0)} // <- min is here
        };

        [TestMethod]
        public void ThrowsColumnNotFound()
        {
            var col = QdbTestCluster.GetNonExistingTimestampColumn();

            try
            {
                col.AbsMin();
                Assert.Fail("No exception thrown");
            }
            catch (QdbColumnNotFoundException e)
            {
                Assert.AreEqual(col.Series.Alias, e.Alias);
                Assert.AreEqual(col.Name, e.Column);
            }
        }

        [TestMethod]
        public void GivenNoArgument_ReturnsAbsMinPointOfTable()
        {
            var col = QdbTestCluster.CreateEmptyTimestampColumn();
            col.Insert(_points);

            var result = col.AbsMin();

            Assert.AreEqual(_points[2], result);
        }

        [TestMethod]
        public void GivenInterval_ReturnsAbsMinPointOfInterval()
        {
            var col = QdbTestCluster.CreateEmptyTimestampColumn();
            col.Insert(_points);

            var interval = new QdbTimeInterval(_points[0].Time, _points[2].Time);
            var result = col.AbsMin(interval);

            Assert.AreEqual(_points[1], result);
        }

        [TestMethod]
        public void GivenOutOfRangeInterval_ReturnsNull()
        {
            var col = QdbTestCluster.CreateEmptyTimestampColumn();
            col.Insert(_points);

            var interval = new QdbTimeInterval(new DateTime(3000, 1, 1), new DateTime(4000, 1, 1));
            var result = col.AbsMin(interval);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GivenSeveralIntervals_ReturnsAbsMinOfEach()
        {
            var col = QdbTestCluster.CreateEmptyTimestampColumn();
            col.Insert(_points);

            var intervals = new[]
            {
                new QdbTimeInterval(new DateTime(2012, 1, 1), new DateTime(2015, 12, 31)),
                new QdbTimeInterval(new DateTime(2014, 1, 1), new DateTime(2017, 12, 31)),
                new QdbTimeInterval(new DateTime(2016, 6, 1), new DateTime(2018, 12, 31))
            };

            var results = col.AbsMin(intervals).ToArray();

            Assert.AreEqual(3, results.Length);
            Assert.AreEqual(_points[1], results[0]);
            Assert.AreEqual(_points[2], results[1]);
            Assert.IsNull(results[2]);
        }

        [TestMethod]
        public void ThrowsEmptyColumn()
        {
            var col = QdbTestCluster.CreateEmptyTimestampColumn();

            try
            {
                col.AbsMin();
                Assert.Fail("No exception thrown");
            }
            catch (QdbEmptyColumnException e)
            {
                Assert.AreEqual(col.Series.Alias, e.Alias);
                Assert.AreEqual(col.Name, e.Column);
            }
        }
    }
}
