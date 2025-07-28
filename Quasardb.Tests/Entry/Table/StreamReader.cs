using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.Table
{
    [TestClass]
    public class StreamReader
    {
        public static QdbTable CreateTable(string alias = null)
        {
            var ts = QdbTestCluster.Instance.Table(alias ?? RandomGenerator.CreateUniqueAlias());
            ts.Create(new QdbColumnDefinition[] {
                new QdbBlobColumnDefinition("the_blob"),
                new QdbDoubleColumnDefinition("the_double"),
                new QdbInt64ColumnDefinition("the_int64"),
                new QdbStringColumnDefinition("the_string"),
                new QdbTimestampColumnDefinition("the_ts"),
            });
            return ts;
        }

        public static QdbBlobPointCollection InsertBlobPoints(QdbTable ts, DateTime time, int count)
        {
            if (ts == null) throw new ArgumentNullException(nameof(ts));

            var random = new Random();
            var r = new QdbBlobPointCollection(count);

            var column = ts.BlobColumns["the_blob"];
            for (var i = 0; i < count; ++i)
            {
                var value = new byte[32];
                random.NextBytes(value);
                column.Insert(time, value);
                r.Add(time, value);
                time = time.AddSeconds(1);
            }
            return r;
        }

        public static QdbDoublePointCollection InsertDoublePoints(QdbTable ts, DateTime time, int count)
        {
            if (ts == null) throw new ArgumentNullException(nameof(ts));

            var random = new Random();
            var r = new QdbDoublePointCollection(count);

            var column = ts.DoubleColumns["the_double"];
            for (var i = 0; i < count; ++i)
            {
                var value = random.NextDouble();
                column.Insert(time, value);
                r.Add(time, value);
                time = time.AddSeconds(1);
            }
            return r;
        }

        public static QdbInt64PointCollection InsertInt64Points(QdbTable ts, DateTime time, int count)
        {
            if (ts == null) throw new ArgumentNullException(nameof(ts));

            var random = new Random();
            var r = new QdbInt64PointCollection(count);

            var column = ts.Int64Columns["the_int64"];
            for (var i = 0; i < count; ++i)
            {
                var value = random.Next();
                column.Insert(time, value);
                r.Add(time, value);
                time = time.AddSeconds(1);
            }
            return r;
        }

        public static QdbTimestampPointCollection InsertTimestampPoints(QdbTable ts, DateTime time, int count)
        {
            if (ts == null) throw new ArgumentNullException(nameof(ts));

            var random = new Random();
            var r = new QdbTimestampPointCollection(count);

            var column = ts.TimestampColumns["the_ts"];
            for (var i = 0; i < count; ++i)
            {
                var value = DateTime.Today.AddSeconds(random.NextDouble());
                column.Insert(time, value);
                r.Add(time, value);
                time = time.AddSeconds(1);
            }
            return r;
        }

        public static QdbStringPointCollection InsertStringPoints(QdbTable ts, DateTime time, int count)
        {
            if (ts == null) throw new ArgumentNullException(nameof(ts));

            var random = new Random();
            var r = new QdbStringPointCollection(count);

            var column = ts.StringColumns["the_string"];
            for (var i = 0; i < count; ++i)
            {
                string value = "content";
                column.Insert(time, value);
                r.Add(time, value);
                time = time.AddSeconds(1);
            }
            return r;
        }

        [TestMethod]
        public void ThrowsInvalidArgument_GivenBadRange()
        {
            QdbTable ts = CreateTable();

            try
            {
                var now = DateTime.Now;
                ts.StreamReader(new QdbTimeInterval(now.AddSeconds(6), now.AddSeconds(5)));
                Assert.Fail("No exception thrown");
            }
            catch (QdbInvalidArgumentException)
            { }
        }

        [TestMethod]
        public void ReturnsCorrectResults()
        {
            var startTime = DateTime.Now;
            QdbTable ts = CreateTable();
            var insertedBlobData = InsertBlobPoints(ts, startTime, 10);
            var insertedDoubleData = InsertDoublePoints(ts, startTime, 10);
            var insertedInt64Data = InsertInt64Points(ts, startTime, 10);
            var insertedStringData = InsertStringPoints(ts, startTime, 10);
            var insertedTimestampData = InsertTimestampPoints(ts, startTime, 10);

            var reader = ts.StreamReader(new QdbTimeInterval(startTime, startTime.AddSeconds(10)));

            for (var i = 0; i < insertedBlobData.Count; i++)
            {
                var row = reader.NextRow();
                Assert.AreEqual(startTime.AddSeconds(i), row.Timestamp);

                Assert.AreEqual(row.Count, 5);
                CollectionAssert.AreEqual(insertedBlobData[i].Value, row["the_blob"].BlobValue);
                Assert.AreEqual(insertedDoubleData[i].Value, row["the_double"].Value);
                Assert.AreEqual(insertedStringData[i].Value, row["the_string"].Value);
                Assert.AreEqual(insertedInt64Data[i].Value, row["the_int64"].Value);
                Assert.AreEqual(insertedTimestampData[i].Value, row["the_ts"].Value);
            }
        }

        [TestMethod]
        public void ReturnsCorrectResults_WithNulls()
        {
            var startTime = DateTime.Now;
            QdbTable ts = CreateTable();
            var insertedBlobData = InsertBlobPoints(ts, startTime, 9);
            var insertedDoubleData = InsertDoublePoints(ts, startTime, 9);
            var insertedInt64Data = InsertInt64Points(ts, startTime, 9);
            var insertedStringData = InsertStringPoints(ts, startTime, 9);
            var insertedTimestampData = InsertTimestampPoints(ts, startTime, 9);

            ts.BlobColumns["the_blob"].Insert(startTime.AddSeconds(9), null);
            insertedBlobData.Add(startTime.AddSeconds(9), null);
            insertedDoubleData.Add(startTime.AddSeconds(9), null);
            insertedInt64Data.Add(startTime.AddSeconds(9), null);
            insertedStringData.Add(startTime.AddSeconds(9), null);
            insertedTimestampData.Add(startTime.AddSeconds(9), null);

            var reader = ts.StreamReader(new QdbTimeInterval(startTime, startTime.AddSeconds(10)));

            for (var i = 0; i < insertedBlobData.Count; i++)
            {
                var row = reader.NextRow();
                Assert.AreEqual(startTime.AddSeconds(i), row.Timestamp);

                Assert.AreEqual(row.Count, 5);
                CollectionAssert.AreEqual(insertedBlobData[i].Value, row["the_blob"].BlobValue);
                Assert.AreEqual(insertedDoubleData[i].Value, row["the_double"].Value);
                Assert.AreEqual(insertedInt64Data[i].Value, row["the_int64"].Value);
                Assert.AreEqual(insertedStringData[i].Value, row["the_string"].Value);
                Assert.AreEqual(insertedTimestampData[i].Value, row["the_ts"].Value);
            }
        }

        [TestMethod]
        public void CanSelectColumns()
        {
            var startTime = DateTime.Now;
            QdbTable ts = CreateTable();
            var insertedBlobData = InsertBlobPoints(ts, startTime, 10);
            var insertedDoubleData = InsertDoublePoints(ts, startTime, 10);
            var insertedInt64Data = InsertInt64Points(ts, startTime, 10);
            var insertedTimestampData = InsertTimestampPoints(ts, startTime, 10);

            var reader = ts.StreamReader(new QdbColumnDefinition[]{
                new QdbDoubleColumnDefinition("the_double"),
                new QdbInt64ColumnDefinition("the_int64")
            }, new QdbTimeInterval(startTime, startTime.AddSeconds(10)));

            int index = 0;
            for (var i = 0; i < insertedDoubleData.Count; i++)
            {
                var row = reader.NextRow();
                Assert.AreEqual(startTime.AddSeconds(i), row.Timestamp);

                Assert.AreEqual(row.Count, 2);
                Assert.AreEqual(insertedDoubleData[i].Value, row["the_double"].Value);
                Assert.AreEqual(insertedInt64Data[i].Value, row["the_int64"].Value);
                index = i;
            }
            Assert.AreEqual(9, index);
        }

        [TestMethod]
        public void CanRequestRanges()
        {
            var startTime = DateTime.Now;
            QdbTable ts = CreateTable();
            var insertedBlobData = InsertBlobPoints(ts, startTime, 10);
            var insertedDoubleData = InsertDoublePoints(ts, startTime, 10);
            var insertedInt64Data = InsertInt64Points(ts, startTime, 10);
            var insertedStringData = InsertStringPoints(ts, startTime, 10);
            var insertedTimestampData = InsertTimestampPoints(ts, startTime, 10);

            var reader = ts.StreamReader(new QdbTimeInterval[]{
                new QdbTimeInterval(startTime, startTime.AddSeconds(1)),
                new QdbTimeInterval(startTime.AddSeconds(5), startTime.AddSeconds(6)),
            });

            int[] mapping = new int[] { 0, 5 };
            for (var i = 0; i < 2; i++)
            {
                var row = reader.NextRow();
                int index = mapping[i];
                Assert.AreEqual(startTime.AddSeconds(index), row.Timestamp);

                Assert.AreEqual(row.Count, 5);
                CollectionAssert.AreEqual(insertedBlobData[index].Value, row["the_blob"].BlobValue);
                Assert.AreEqual(insertedDoubleData[index].Value, row["the_double"].Value);
                Assert.AreEqual(insertedInt64Data[index].Value, row["the_int64"].Value);
                Assert.AreEqual(insertedStringData[index].Value, row["the_string"].Value);
                Assert.AreEqual(insertedTimestampData[index].Value, row["the_ts"].Value);
            }
        }
    }
}
