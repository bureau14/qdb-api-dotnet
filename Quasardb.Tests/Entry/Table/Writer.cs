using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;
using Quasardb.TimeSeries.Writer;
using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;

namespace Quasardb.Tests.Entry.Table
{
    [TestClass]
    public class Writer
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

        public static QdbBlobPointCollection CreateBlobPoints(DateTime time, int count)
        {
            var random = new Random();
            var r = new QdbBlobPointCollection(count);

            for (var i = 0; i < count; ++i)
            {
                var value = new byte[32];
                random.NextBytes(value);
                r.Add(time, value);
                time = time.AddSeconds(1);
            }
            return r;
        }

        public static QdbDoublePointCollection CreateDoublePoints(DateTime time, int count)
        {
            var random = new Random();
            var r = new QdbDoublePointCollection(count);

            for (var i = 0; i < count; ++i)
            {
                r.Add(time, random.NextDouble());
                time = time.AddSeconds(1);
            }
            return r;
        }

        public static QdbInt64PointCollection CreateInt64Points(DateTime time, int count)
        {
            var random = new Random();
            var r = new QdbInt64PointCollection(count);

            for (var i = 0; i < count; ++i)
            {
                r.Add(time, random.Next());
                time = time.AddSeconds(1);
            }
            return r;
        }

        public static QdbTimestampPointCollection CreateTimestampPoints(DateTime time, int count)
        {
            var random = new Random();
            var r = new QdbTimestampPointCollection(count);

            for (var i = 0; i < count; ++i)
            {
                r.Add(time, DateTime.Today.AddSeconds(random.NextDouble()));
                time = time.AddSeconds(1);
            }
            return r;
        }

        public static QdbStringPointCollection CreateStringPoints(DateTime time, int count)
        {
            var random = new Random();
            var r = new QdbStringPointCollection(count);

            for (var i = 0; i < count; ++i)
            {
                var value = new byte[32];
                random.NextBytes(value);
                r.Add(time, System.Text.Encoding.UTF8.GetString(value));
                time = time.AddSeconds(1);
            }
            return r;
        }

        public static QdbTableWriter Insert(QdbTable ts,
            DateTime startTime,
            QdbBlobPointCollection blobPoints,
            QdbDoublePointCollection doublePoints,
            QdbInt64PointCollection int64Points,
            QdbStringPointCollection stringPoints,
            QdbTimestampPointCollection timestampPoints)
        {
            if (ts == null) throw new ArgumentNullException(nameof(ts));
            if (blobPoints == null) throw new ArgumentNullException(nameof(blobPoints));
            if (doublePoints == null) throw new ArgumentNullException(nameof(doublePoints));
            if (int64Points == null) throw new ArgumentNullException(nameof(int64Points));
            if (stringPoints == null) throw new ArgumentNullException(nameof(stringPoints));
            if (timestampPoints == null) throw new ArgumentNullException(nameof(timestampPoints));

            var writer = ts.Writer();
            for (var i = 0; i < 10; ++i)
            {
                writer.StartRow(startTime.AddSeconds(i));
                writer.SetBlob("the_blob", blobPoints[i].Value);
                writer.SetDouble("the_double", doublePoints[i].Value);
                writer.SetInt64("the_int64", int64Points[i].Value);
                writer.SetString("the_string", stringPoints[i].Value);
                writer.SetTimestamp("the_ts", timestampPoints[i].Value);
            }
            return writer;
        }

        public static void CheckTable(QdbTable ts,
            QdbBlobPointCollection blobPoints,
            QdbDoublePointCollection doublePoints,
            QdbInt64PointCollection int64Points,
            QdbStringPointCollection stringPoints,
            QdbTimestampPointCollection timestampPoints)
        {
            if (ts == null) throw new ArgumentNullException(nameof(ts));
            if (blobPoints == null) throw new ArgumentNullException(nameof(blobPoints));
            if (doublePoints == null) throw new ArgumentNullException(nameof(doublePoints));
            if (int64Points == null) throw new ArgumentNullException(nameof(int64Points));
            if (stringPoints == null) throw new ArgumentNullException(nameof(stringPoints));
            if (timestampPoints == null) throw new ArgumentNullException(nameof(timestampPoints));

            var blobColumn = ts.BlobColumns["the_blob"];
            CollectionAssert.AreEqual(blobPoints.ToArray(), blobColumn.Points().ToArray());

            var doubleColumn = ts.DoubleColumns["the_double"];
            CollectionAssert.AreEqual(doublePoints.ToArray(), doubleColumn.Points().ToArray());

            var int64Column = ts.Int64Columns["the_int64"];
            CollectionAssert.AreEqual(int64Points.ToArray(), int64Column.Points().ToArray());

            var stringColumn = ts.StringColumns["the_string"];
            CollectionAssert.AreEqual(stringPoints.ToArray(), stringColumn.Points().ToArray());

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
            QdbTable ts = CreateTable();
            var blobData = CreateBlobPoints(startTime, 10);
            var doubleData = CreateDoublePoints(startTime, 10);
            var int64Data = CreateInt64Points(startTime, 10);
            var stringData = CreateStringPoints(startTime, 10);
            var timestampData = CreateTimestampPoints(startTime, 10);

            var writer = Insert(ts, startTime, blobData, doubleData, int64Data, stringData, timestampData);
            writer.Push();

            CheckTable(ts, blobData, doubleData, int64Data, stringData, timestampData);
        }

        [TestMethod]
        public void Ok_BulkRowInsertPartial()
        {
            var startTime = DateTime.Now;
            QdbTable ts = CreateTable();
            var blobPoints = CreateBlobPoints(startTime, 10);

            var writer = ts.Writer(new QdbColumnDefinition[]{
                new QdbBlobColumnDefinition("the_blob")
            });
            for (var i = 0; i < 10; ++i)
            {
                writer.StartRow(startTime.AddSeconds(i));
                writer.SetBlob("the_blob", blobPoints[i].Value);
            }
            writer.Push();

            var blobColumn = ts.BlobColumns["the_blob"];
            CollectionAssert.AreEqual(blobPoints.ToArray(), blobColumn.Points().ToArray());

            var doubleColumn = ts.DoubleColumns["the_double"];
            var doublePoints = doubleColumn.Points().ToArray();
            Assert.IsTrue(doublePoints.All(x => x.Value == null));
            Assert.AreEqual(blobPoints.Count, doublePoints.Length);

            var int64Column = ts.Int64Columns["the_int64"];
            var int64Points = int64Column.Points().ToArray();
            Assert.IsTrue(int64Points.All(x => x.Value == null));
            Assert.AreEqual(blobPoints.Count, int64Points.Length);

            var stringColumn = ts.StringColumns["the_blob"];
            var stringPoints = stringColumn.Points().ToArray();
            CollectionAssert.AreEqual(stringPoints.ToArray(), stringColumn.Points().ToArray());
            Assert.AreEqual(blobPoints.Count, stringPoints.Length);

            var timestampColumn = ts.TimestampColumns["the_ts"];
            var timestampPoints = timestampColumn.Points().ToArray();
            Assert.IsTrue(timestampPoints.All(x => x.Value == null));
            Assert.AreEqual(blobPoints.Count, timestampPoints.Length);
        }

        [TestMethod]
        public void Ok_BulkRowInsertWithNulls()
        {
            var startTime = DateTime.Now;
            QdbTable ts = CreateTable();
            var blobData = CreateBlobPoints(startTime, 9);
            var doubleData = CreateDoublePoints(startTime, 9);
            var int64Data = CreateInt64Points(startTime, 9);
            var stringData = CreateStringPoints(startTime, 9);
            var timestampData = CreateTimestampPoints(startTime, 9);

            blobData.Add(startTime.AddSeconds(9), new byte[] { 10 });
            doubleData.Add(startTime.AddSeconds(9), null);
            int64Data.Add(startTime.AddSeconds(9), null);
            stringData.Add(startTime.AddSeconds(9), System.Text.Encoding.UTF8.GetString(new byte[] { 10 }));
            timestampData.Add(startTime.AddSeconds(9), null);

            var writer = Insert(ts, startTime, blobData, doubleData, int64Data, stringData, timestampData);
            writer.Push();

            CheckTable(ts, blobData, doubleData, int64Data, stringData, timestampData);
        }

        [TestMethod]
        public void Ok_BulkRowFastInsert()
        {
            var startTime = DateTime.Now;
            QdbTable ts = CreateTable();
            var blobData = CreateBlobPoints(startTime, 10);
            var doubleData = CreateDoublePoints(startTime, 10);
            var int64Data = CreateInt64Points(startTime, 10);
            var stringData = CreateStringPoints(startTime, 10);
            var timestampData = CreateTimestampPoints(startTime, 10);

            var writer = Insert(ts, startTime, blobData, doubleData, int64Data, stringData, timestampData);
            writer.PushFast();

            CheckTable(ts, blobData, doubleData, int64Data, stringData, timestampData);
        }

        [TestMethod]
        public void Ok_BulkRowAsyncInsert()
        {
            var startTime = DateTime.Now;
            QdbTable ts = CreateTable();
            var blobData = CreateBlobPoints(startTime, 10);
            var doubleData = CreateDoublePoints(startTime, 10);
            var int64Data = CreateInt64Points(startTime, 10);
            var stringData = CreateStringPoints(startTime, 10);
            var timestampData = CreateTimestampPoints(startTime, 10);

            var writer = Insert(ts, startTime, blobData, doubleData, int64Data, stringData, timestampData);
            writer.PushAsync();

            // Wait for push_async to complete
            // Ideally we could be able to get the proper flush interval
            Thread.Sleep(8 * 1000);

            CheckTable(ts, blobData, doubleData, int64Data, stringData, timestampData);
        }
    }
}
