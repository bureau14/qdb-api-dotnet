using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.TimeSeries
{
    [TestClass]
    public class Timestamps
    {
        readonly DateTime[] _timestamps =
        {
            new DateTime(2012, 11, 02),
            new DateTime(2014, 06, 30),
            new DateTime(2016, 02, 04)
        };

        public QdbTimeSeries CreateTable(string alias = null)
        {
            var ts = QdbTestCluster.Instance.TimeSeries(alias ?? RandomGenerator.CreateUniqueAlias());
            ts.Create(new QdbColumnDefinition[] {
                new QdbBlobColumnDefinition("the_blob"),
                new QdbDoubleColumnDefinition("the_double"),
                new QdbInt64ColumnDefinition("the_int64")
            });
            ts.BlobColumns["the_blob"].Insert(new QdbBlobPoint(_timestamps[0], Encoding.UTF8.GetBytes("Hello World!")));
            ts.DoubleColumns["the_double"].Insert(new QdbDoublePoint(_timestamps[1], 42));
            ts.Int64Columns["the_int64"].Insert(new QdbInt64Point(_timestamps[2], 666));
            return ts;
        }

        [TestMethod]
        public void GivenNoArgument_ReturnsTimestampsOfTimeSeries()
        {
            var ts = CreateTable();

            var result = ts.Timestamps();

            CollectionAssert.AreEqual(_timestamps.ToList(), result.ToList());
        }

        [TestMethod]
        public void GivenInRangeInterval_ReturnsTimestampsOfInterval()
        {
            var ts = CreateTable();

            var interval = new QdbTimeInterval(_timestamps[0], _timestamps[2]);
            var result = ts.Timestamps(interval);

            CollectionAssert.AreEqual(_timestamps.Take(2).ToList(), result.ToList());
        }

        [TestMethod]
        public void GivenOutOfRangeInterval_ReturnsEmptyCollection()
        {
            var ts = CreateTable();

            var interval = new QdbTimeInterval(new DateTime(3000, 1, 1), new DateTime(4000, 1, 1));
            var result = ts.Timestamps(interval);

            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void GivenSeveralIntervals_ReturnsTimestampsOfEach()
        {
            var ts = CreateTable();

            var intervals = new[]
            {
                new QdbTimeInterval(new DateTime(2014, 1, 1), new DateTime(2014, 12, 31)),
                new QdbTimeInterval(new DateTime(2016, 1, 1), new DateTime(2016, 12, 31)),
                new QdbTimeInterval(new DateTime(2018, 1, 1), new DateTime(2018, 12, 31))
            };

            var result = ts.Timestamps(intervals);

            CollectionAssert.AreEqual(_timestamps.Skip(1).ToList(), result.ToList());
        }
    }
}
