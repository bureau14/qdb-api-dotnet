using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;
using Quasardb.TimeSeries.Writer;

namespace Quasardb.Tests.Entry.TimeSeries
{
    [TestClass]
    public class Writer
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

        public QdbBlobPointCollection CreateBlobPoints(DateTime time, int count)
        {
            Random random = new Random();
            var r = new QdbBlobPointCollection(count);

            for (int i = 0; i < count; ++i)
            {
                var value = new byte[32];
                random.NextBytes(value);
                r.Add(time, value);
                time = time.AddSeconds(1);
            }
            return r;
        }

        public QdbDoublePointCollection CreateDoublePoints(DateTime time, int count)
        {
            Random random = new Random();
            var r = new QdbDoublePointCollection(count);

            for (int i = 0; i < count; ++i)
            {
                r.Add(time, random.NextDouble());
                time = time.AddSeconds(1);
            }
            return r;
        }

        public QdbInt64PointCollection CreateInt64Points(DateTime time, int count)
        {
            Random random = new Random();
            var r = new QdbInt64PointCollection(count);

            for (int i = 0; i < count; ++i)
            {
                r.Add(time, random.Next());
                time = time.AddSeconds(1);
            }
            return r;
        }

        public QdbTimestampPointCollection CreateTimestampPoints(DateTime time, int count)
        {
            Random random = new Random();
            var r = new QdbTimestampPointCollection(count);

            for (int i = 0; i < count; ++i)
            {
                r.Add(time, DateTime.Today.AddSeconds(random.NextDouble()));
                time = time.AddSeconds(1);
            }
            return r;
        }

        public QdbTimeSeriesWriter Insert(QdbTimeSeries ts,
            DateTime startTime,
            QdbBlobPointCollection blobPoints,
            QdbDoublePointCollection doublePoints,
            QdbInt64PointCollection int64Points,
            QdbTimestampPointCollection timestampPoints)
        {
            var writer = ts.Writer();
            for (int i = 0; i < 10; ++i)
            {
                writer.StartRow(startTime.AddSeconds(i));
                writer.SetBlob("the_blob", blobPoints[i].Value);
                writer.SetDouble("the_double", doublePoints[i].Value);
                writer.SetInt64("the_int64", int64Points[i].Value);
                writer.SetTimestamp("the_ts", timestampPoints[i].Value);
            }
            return writer;
        }

        public void CheckTable(QdbTimeSeries ts,
            QdbBlobPointCollection blobPoints,
            QdbDoublePointCollection doublePoints,
            QdbInt64PointCollection int64Points,
            QdbTimestampPointCollection timestampPoints)
        {
            var blobColumn = ts.BlobColumns["the_blob"];
            CollectionAssert.AreEqual(blobPoints.ToArray(), blobColumn.Points().ToArray());

            var doubleColumn = ts.DoubleColumns["the_double"];
            CollectionAssert.AreEqual(doublePoints.ToArray(), doubleColumn.Points().ToArray());

            var int64Column = ts.Int64Columns["the_int64"];
            CollectionAssert.AreEqual(int64Points.ToArray(), int64Column.Points().ToArray());

            var timestampColumn = ts.TimestampColumns["the_ts"];
            CollectionAssert.AreEqual(timestampPoints.ToArray(), timestampColumn.Points().ToArray());
        }

        [TestMethod]
        public void ThrowsColumnNotFound_GivenNonExistingColumns()
        {
            var ts = CreateTable();

            try
            {
                ts.Writer(new QdbColumnDefinition[]{
                    new QdbDoubleColumnDefinition("col")
                });
                Assert.Fail("No exception thrown");
            }
            catch (QdbColumnNotFoundException e)
            {
                // we do not know which column it is
                Assert.IsNull(e.Alias);
                Assert.IsNull(e.Column);
            }
        }

        [TestMethod]
        public void Ok_BulkRowInsert()
        {
            var startTime = DateTime.Now;
            QdbTimeSeries ts = CreateTable();
            var blobData = CreateBlobPoints(startTime, 10);
            var doubleData = CreateDoublePoints(startTime, 10);
            var int64Data = CreateInt64Points(startTime, 10);
            var timestampData = CreateTimestampPoints(startTime, 10);

            var writer = Insert(ts, startTime, blobData, doubleData, int64Data, timestampData);
            writer.Push();

            CheckTable(ts, blobData, doubleData, int64Data, timestampData);
        }

        [TestMethod]
        public void Ok_BulkPartialRowInsert()
        {
            var startTime = DateTime.Now;
            QdbTimeSeries ts = CreateTable();
            var blobPoints = CreateBlobPoints(startTime, 10);

            var writer = ts.Writer(new QdbColumnDefinition[]{
                new QdbBlobColumnDefinition("the_blob")
            });
            for (int i = 0; i < 10; ++i)
            {
                writer.StartRow(startTime.AddSeconds(i));
                writer.SetBlob("the_blob", blobPoints[i].Value);
            }
            writer.Push();

            var blobColumn = ts.BlobColumns["the_blob"];
            CollectionAssert.AreEqual(blobPoints.ToArray(), blobColumn.Points().ToArray());

            var doubleColumn = ts.DoubleColumns["the_double"];
            Assert.AreEqual(blobPoints.Count, doubleColumn.Points().ToArray().Length);

            var int64Column = ts.Int64Columns["the_int64"];
            Assert.AreEqual(blobPoints.Count, int64Column.Points().ToArray().Length);

            var timestampColumn = ts.TimestampColumns["the_ts"];
            Assert.AreEqual(blobPoints.Count, timestampColumn.Points().ToArray().Length);
        }

        [TestMethod]
        public void Ok_BulkRowFastInsert()
        {
            var startTime = DateTime.Now;
            QdbTimeSeries ts = CreateTable();
            var blobData = CreateBlobPoints(startTime, 10);
            var doubleData = CreateDoublePoints(startTime, 10);
            var int64Data = CreateInt64Points(startTime, 10);
            var timestampData = CreateTimestampPoints(startTime, 10);

            var writer = Insert(ts, startTime, blobData, doubleData, int64Data, timestampData);
            writer.PushFast();

            CheckTable(ts, blobData, doubleData, int64Data, timestampData);
        }

        [TestMethod]
        public void Ok_BulkRowAsyncInsert()
        {
            var startTime = DateTime.Now;
            QdbTimeSeries ts = CreateTable();
            var blobData = CreateBlobPoints(startTime, 10);
            var doubleData = CreateDoublePoints(startTime, 10);
            var int64Data = CreateInt64Points(startTime, 10);
            var timestampData = CreateTimestampPoints(startTime, 10);

            var writer = Insert(ts, startTime, blobData, doubleData, int64Data, timestampData);
            writer.PushAsync();

            // Wait for push_async to complete
            // Ideally we could be able to get the proper flush interval
            Thread.Sleep(8 * 1000);

            CheckTable(ts, blobData, doubleData, int64Data, timestampData);
        }
    }
}
