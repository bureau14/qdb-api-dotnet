﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.Table.Int64
{
    [TestClass]
    public class Kurtosis
    {
        readonly QdbInt64PointCollection _points = new QdbInt64PointCollection
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
            var col = QdbTestCluster.GetNonExistingInt64Column();

            try
            {
                col.Kurtosis();
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
            var col = QdbTestCluster.CreateEmptyInt64Column();

            try
            {
                col.Kurtosis();
                Assert.Fail("No exception thrown");
            }
            catch (QdbEmptyColumnException e)
            {
                Assert.AreEqual(col.Series.Alias, e.Alias);
                Assert.AreEqual(col.Name, e.Column);
            }
        }

        [TestMethod]
        public void GivenNoArgument_ReturnsKurtosisOfTable()
        {
            var col = QdbTestCluster.CreateEmptyInt64Column();
            col.Insert(_points);

            var result = col.Kurtosis();

            Assert.IsNotNull(result);
            Assert.AreEqual(0.021815331452089914, result.Value, /*delta=*/1e-5);
        }

        [TestMethod]
        public void GivenInRangeInterval_ReturnsKurtosisOfInterval()
        {
            var col = QdbTestCluster.CreateEmptyInt64Column();
            col.Insert(_points);

            var interval = new QdbTimeInterval(_points[0].Time, _points[4].Time);
            var result = col.Kurtosis(interval);

            Assert.IsNotNull(result);
            Assert.AreEqual(-1.4424596911530097, result.Value, /*delta=*/1e-5);
        }

        [TestMethod]
        public void GivenSeveralIntervals_ReturnsKurtosisOfEach()
        {
            var col = QdbTestCluster.CreateEmptyInt64Column();
            col.Insert(_points);

            var intervals = new[]
            {
                new QdbTimeInterval(new DateTime(2012, 11, 02).AddHours(1), new DateTime(2012, 11, 02).AddHours(3)),
                new QdbTimeInterval(new DateTime(2012, 11, 02).AddHours(2), new DateTime(2012, 11, 02).AddHours(5)),
                new QdbTimeInterval(new DateTime(2012, 11, 03), new DateTime(2012, 11, 04))
            };

            var results = col.Kurtosis(intervals).ToArray();

            Assert.AreEqual(3, results.Length);
            Assert.IsNotNull(results[0]);
            Assert.AreEqual(-2, results[0].Value, /*delta=*/1e-5);
            Assert.IsNotNull(results[1]);
            Assert.AreEqual(-1.5000000000000004, results[1].Value, /*delta=*/1e-5);
            Assert.IsNull(results[2]);
        }
    }
}
