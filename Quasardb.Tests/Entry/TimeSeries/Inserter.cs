using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.TimeSeries
{
    [TestClass]
    public class Inserter
    {
        private readonly QdbCluster _cluster = QdbTestCluster.Instance;

        public QdbTimeSeries CreateTable(string alias = null)
        {
            var ts = _cluster.TimeSeries(alias ?? RandomGenerator.CreateUniqueAlias());
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

        public QdbTimeSeriesBatch Insert(QdbTimeSeries ts,
            DateTime startTime,
            QdbBlobPointCollection blobPoints,
            QdbDoublePointCollection doublePoints,
            QdbInt64PointCollection int64Points,
            QdbTimestampPointCollection timestampPoints)
        {
            var batch = _cluster.Inserter(new QdbBatchColumnDefinition[]{
                new QdbBatchColumnDefinition(ts.Alias, "the_blob"),
                new QdbBatchColumnDefinition(ts.Alias, "the_double"),
                new QdbBatchColumnDefinition(ts.Alias, "the_int64"),
                new QdbBatchColumnDefinition(ts.Alias, "the_ts"),
            });
            for (int i = 0; i < 10; ++i)
            {
                batch.StartRow(startTime.AddSeconds(i));
                batch.SetBlob(0, blobPoints[i].Value);
                batch.SetDouble(1, doublePoints[i].Value);
                batch.SetInt64(2, int64Points[i].Value);
                batch.SetTimestamp(3, timestampPoints[i].Value);
            }
            return batch;
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
                var batch = _cluster.Inserter(new QdbBatchColumnDefinition(ts.Alias, "col"));
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

            var batch = Insert(ts, startTime, blobData, doubleData, int64Data, timestampData);
            batch.Push();

            CheckTable(ts, blobData, doubleData, int64Data, timestampData);
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

            var batch = Insert(ts, startTime, blobData, doubleData, int64Data, timestampData);
            batch.PushFast();

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

            var batch = Insert(ts, startTime, blobData, doubleData, int64Data, timestampData);
            batch.PushAsync();

            // Wait for push_async to complete
            // Ideally we could be able to get the proper flush interval
            Thread.Sleep(8 * 1000);

            CheckTable(ts, blobData, doubleData, int64Data, timestampData);
        }
    }
}
