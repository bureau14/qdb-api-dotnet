﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.Table.Int64
{
    [TestClass]
    public class PopulationStdDev
    {
        readonly QdbInt64PointCollection _points = new QdbInt64PointCollection
        {
            {new DateTime(2012, 11, 02), 1},
            {new DateTime(2014, 06, 30), 42},
            {new DateTime(2016, 02, 04), 666},
            {new DateTime(2016, 03, 05), 1234},
            {new DateTime(2016, 04, 06), 5678}
        };

        [TestMethod]
        public void ThrowsColumnNotFound()
        {
            var col = QdbTestCluster.GetNonExistingInt64Column();

            try
            {
                col.PopulationStdDev();
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
                col.PopulationStdDev();
                Assert.Fail("No exception thrown");
            }
            catch (QdbEmptyColumnException e)
            {
                Assert.AreEqual(col.Series.Alias, e.Alias);
                Assert.AreEqual(col.Name, e.Column);
            }
        }

        [TestMethod]
        public void GivenNoArgument_ReturnsPopulationStdDevOfTable()
        {
            var col = QdbTestCluster.CreateEmptyInt64Column();
            col.Insert(_points);

            var result = col.PopulationStdDev();

            Assert.IsNotNull(result);
            Assert.AreEqual(2125.6449750605111, result.Value, /*delta=*/1e-5);
        }

        [TestMethod]
        public void GivenInRangeInterval_ReturnsPopulationStdDevOfInterval()
        {
            var col = QdbTestCluster.CreateEmptyInt64Column();
            col.Insert(_points);

            var interval = new QdbTimeInterval(_points[0].Time, _points[4].Time);
            var result = col.PopulationStdDev(interval);

            Assert.IsNotNull(result);
            Assert.AreEqual(506.02982866625558, result.Value, /*delta=*/1e-5);
        }

        [TestMethod]
        public void GivenSeveralIntervals_ReturnsPopulationStdDevOfEach()
        {
            var col = QdbTestCluster.CreateEmptyInt64Column();
            col.Insert(_points);

            var intervals = new[]
            {
                new QdbTimeInterval(new DateTime(2012, 1, 1), new DateTime(2015, 12, 31)),
                new QdbTimeInterval(new DateTime(2014, 1, 1), new DateTime(2017, 12, 31)),
                new QdbTimeInterval(new DateTime(2016, 6, 1), new DateTime(2018, 12, 31))
            };

            var results = col.PopulationStdDev(intervals).ToArray();

            Assert.AreEqual(3, results.Length);
            Assert.IsNotNull(results[0]);
            Assert.AreEqual(20.5, results[0].Value, /*delta=*/1e-5);
            Assert.IsNotNull(results[1]);
            Assert.AreEqual(2218.7642957285934, results[1].Value, /*delta=*/1e-5);
            Assert.IsNull(results[2]);
        }
    }
}
