using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.TimeSeries
{
    [TestClass]
    public class Reader
    {
        public QdbTimeSeries CreateTable(string alias = null)
        {
            var ts = QdbTestCluster.Instance.TimeSeries(alias ?? RandomGenerator.CreateUniqueAlias());
            ts.Create(new QdbColumnDefinition[] {
                new QdbBlobColumnDefinition("the_blob"),
                new QdbDoubleColumnDefinition("the_double"),
                new QdbInt64ColumnDefinition("the_int64"),
                new QdbTimestampColumnDefinition("the_ts"),
            });
            return ts;
        }

        public byte[][] InsertBlobPoints(QdbTimeSeries ts, DateTime time, int count)
        {
            Random random = new Random();
            var r = new byte[count][];

            var column = ts.BlobColumns["the_blob"];
            for (int i = 0; i < count; ++i)
            {
                r[i] = new byte[32];
                random.NextBytes(r[i]);
                column.Insert(time, r[i]);
                time = time.AddSeconds(1);
            }
            return r;
        }

        public double[] InsertDoublePoints(QdbTimeSeries ts, DateTime time, int count)
        {
            Random random = new Random();
            var r = new double[count];

            var column = ts.DoubleColumns["the_double"];
            for (int i = 0; i < count; ++i)
            {
                column.Insert(time, r[i] = random.NextDouble());
                time = time.AddSeconds(1);
            }
            return r;
        }

        public long[] InsertInt64Points(QdbTimeSeries ts, DateTime time, int count)
        {
            Random random = new Random();
            var r = new long[count];

            var column = ts.Int64Columns["the_int64"];
            for (int i = 0; i < count; ++i)
            {
                column.Insert(time, r[i] = random.Next());
                time = time.AddSeconds(1);
            }
            return r;
        }

        public DateTime[] InsertTimestampPoints(QdbTimeSeries ts, DateTime time, int count)
        {
            Random random = new Random();
            var r = new DateTime[count];

            var column = ts.TimestampColumns["the_ts"];
            for (int i = 0; i < count; ++i)
            {
                column.Insert(time, r[i] = DateTime.Today.AddSeconds(random.NextDouble()));
                time = time.AddSeconds(1);
            }
            return r;
        }

        [TestMethod]
        public void ThrowsInvalidArgument_GivenBadRange()
        {
            QdbTimeSeries ts = CreateTable();

            try
            {
                var now = DateTime.Now;
                ts.Reader(new QdbTimeInterval(now.AddSeconds(6), now.AddSeconds(5)));
                Assert.Fail("No exception thrown");
            }
            catch (QdbInvalidArgumentException)
            { }
        }

        [TestMethod]
        public void CanReturnNoRows()
        {
            var ts = CreateTable();

            var reader = ts.Reader();
            Assert.AreEqual(0, reader.Count());
        }

        [TestMethod]
        public void ReturnsCorrectResults()
        {
            var startTime = DateTime.Now;
            QdbTimeSeries ts = CreateTable();
            var insertedBlobData = InsertBlobPoints(ts, startTime, 10);
            var insertedDoubleData = InsertDoublePoints(ts, startTime, 10);
            var insertedInt64Data = InsertInt64Points(ts, startTime, 10);
            var insertedTimestampData = InsertTimestampPoints(ts, startTime, 10);

            var reader = ts.Reader();

            long index = 0;
            foreach (var row in reader)
            {
                Assert.AreEqual(startTime.AddSeconds(index), row.Timestamp);

                Assert.AreEqual(row.Count, 4);
                CollectionAssert.AreEqual(insertedBlobData[index], row["the_blob"].BlobValue);
                Assert.AreEqual(insertedDoubleData[index], row["the_double"].Value);
                Assert.AreEqual(insertedInt64Data[index], row["the_int64"].Value);
                Assert.AreEqual(insertedTimestampData[index], row["the_ts"].Value);
                ++index;
            }
        }

        [TestMethod]
        public void CanSelectColumns()
        {
            var startTime = DateTime.Now;
            QdbTimeSeries ts = CreateTable();
            var insertedBlobData = InsertBlobPoints(ts, startTime, 10);
            var insertedDoubleData = InsertDoublePoints(ts, startTime, 10);
            var insertedInt64Data = InsertInt64Points(ts, startTime, 10);
            var insertedTimestampData = InsertTimestampPoints(ts, startTime, 10);

            var reader = ts.Reader(new QdbColumnDefinition[]{
                new QdbDoubleColumnDefinition("the_double"),
                new QdbInt64ColumnDefinition("the_int64")
            });

            long index = 0;
            foreach (var row in reader)
            {
                Assert.AreEqual(startTime.AddSeconds(index), row.Timestamp);

                Assert.AreEqual(row.Count, 2);
                Assert.AreEqual(insertedDoubleData[index], row["the_double"].Value);
                Assert.AreEqual(insertedInt64Data[index], row["the_int64"].Value);
                ++index;
            }
            Assert.AreEqual(10L, index);
        }

        [TestMethod]
        public void CanRequestRanges()
        {
            var startTime = DateTime.Now;
            QdbTimeSeries ts = CreateTable();
            var insertedBlobData = InsertBlobPoints(ts, startTime, 10);
            var insertedDoubleData = InsertDoublePoints(ts, startTime, 10);
            var insertedInt64Data = InsertInt64Points(ts, startTime, 10);
            var insertedTimestampData = InsertTimestampPoints(ts, startTime, 10);

            var reader = ts.Reader(new QdbTimeInterval[]{
                new QdbTimeInterval(startTime, startTime.AddSeconds(1)),
                new QdbTimeInterval(startTime.AddSeconds(5), startTime.AddSeconds(6)),
            });

            long i = 0;
            long[] mapping = new long[] { 0, 5 };
            foreach (var row in reader)
            {
                long index = mapping[i];
                Assert.AreEqual(startTime.AddSeconds(index), row.Timestamp);

                Assert.AreEqual(row.Count, 4);
                CollectionAssert.AreEqual(insertedBlobData[index], row["the_blob"].BlobValue);
                Assert.AreEqual(insertedDoubleData[index], row["the_double"].Value);
                Assert.AreEqual(insertedInt64Data[index], row["the_int64"].Value);
                Assert.AreEqual(insertedTimestampData[index], row["the_ts"].Value);
                ++i;
            }
            Assert.AreEqual(2L, i);
        }
    }
}
