using System;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;
using Quasardb.TimeSeries.Writer;

namespace Quasardb.Tests.Cluster
{
    [TestClass]
    public class Writer
    {
        private readonly QdbCluster _cluster = QdbTestCluster.Instance;

        public QdbTable CreateTable(string alias = null)
        {
            var ts = _cluster.Table(alias ?? RandomGenerator.CreateUniqueAlias());
            ts.Create(new QdbColumnDefinition[] {
                new QdbBlobColumnDefinition("the_blob"),
                new QdbDoubleColumnDefinition("the_double"),
                new QdbInt64ColumnDefinition("the_int64"),
                new QdbStringColumnDefinition("the_string"),
                new QdbTimestampColumnDefinition("the_ts"),
                new QdbSymbolColumnDefinition("the_symbol", "symtable"),
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
        
        public static string RandomString(int length, Random r)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[r.Next(s.Length)]).ToArray());
        }

        public QdbStringPointCollection CreateStringPoints(DateTime time, int count)
        {
            Random random = new Random();
            var r = new QdbStringPointCollection(count);

            for (int i = 0; i < count; ++i)
            {
                var value = new byte[32];
                random.NextBytes(value);

                r.Add(time, RandomString(32, random));
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

        public QdbSymbolPointCollection CreateSymbolPoints(DateTime time, int count)
        {
            Random random = new Random();
            var r = new QdbSymbolPointCollection(count);

            for (int i = 0; i < count; ++i)
            {
                r.Add(time, RandomString(32, random));
                time = time.AddSeconds(1);
            }
            return r;
        }

        public QdbTableWriter Insert(QdbTable ts1, QdbTable ts2,
            DateTime startTime,
            QdbBlobPointCollection blobPoints,
            QdbDoublePointCollection doublePoints,
            QdbInt64PointCollection int64Points,
            QdbStringPointCollection stringPoints,
            QdbTimestampPointCollection timestampPoints,
            QdbSymbolPointCollection symbolPoints)
        {
            var batch = _cluster.Writer(new QdbBatchColumnDefinition[]{
                new QdbBatchColumnDefinition(ts1.Alias, "the_blob"),
                new QdbBatchColumnDefinition(ts1.Alias, "the_double"),
                new QdbBatchColumnDefinition(ts1.Alias, "the_int64"),
                new QdbBatchColumnDefinition(ts2.Alias, "the_string"),
                new QdbBatchColumnDefinition(ts2.Alias, "the_ts"),
                new QdbBatchColumnDefinition(ts2.Alias, "the_symbol"),
            });
            for (int i = 0; i < 10; ++i)
            {
                batch.StartRow(startTime.AddSeconds(i));
                batch.SetBlob("the_blob", blobPoints[i].Value);
                batch.SetDouble("the_double", doublePoints[i].Value);
                batch.SetInt64("the_int64", int64Points[i].Value);
                batch.SetString("the_string", stringPoints[i].Value);
                batch.SetTimestamp("the_ts", timestampPoints[i].Value);
                batch.SetSymbol("the_symbol", symbolPoints[i].Value);
            }
            return batch;
        }

        public void CheckTables(QdbTable ts1, QdbTable ts2,
            QdbBlobPointCollection blobPoints,
            QdbDoublePointCollection doublePoints,
            QdbInt64PointCollection int64Points,
            QdbStringPointCollection stringPoints,
            QdbTimestampPointCollection timestampPoints,
            QdbSymbolPointCollection symbolPoints)
        {
            var blobColumn = ts1.BlobColumns["the_blob"];
            Console.WriteLine("EXP " + blobPoints.Count + " BLOBS");
            Console.WriteLine("GOT " + blobColumn.Points().Count() + " BLOBS");
            CollectionAssert.AreEqual(blobPoints.ToArray(), blobColumn.Points().ToArray());

            var doubleColumn = ts1.DoubleColumns["the_double"];
            Console.WriteLine("EXP " + doublePoints.Count + " DOUBLES");
            Console.WriteLine("GOT " + doubleColumn.Points().Count() + " DOUBLES");
            CollectionAssert.AreEqual(doublePoints.ToArray(), doubleColumn.Points().ToArray());

            var int64Column = ts1.Int64Columns["the_int64"];
            CollectionAssert.AreEqual(int64Points.ToArray(), int64Column.Points().ToArray());

            var stringColumn = ts2.StringColumns["the_string"];
            Console.WriteLine("EXP " + stringPoints.Count + " STRINGS");
            Console.WriteLine("GOT " + stringColumn.Points().Count() + " STRINGS");
            CollectionAssert.AreEqual(stringPoints.ToArray(), stringColumn.Points().ToArray());

            var timestampColumn = ts2.TimestampColumns["the_ts"];
            CollectionAssert.AreEqual(timestampPoints.ToArray(), timestampColumn.Points().ToArray());
            
            var symbolColumn = ts2.SymbolColumns["the_symbol"];
            Console.WriteLine("EXP " + symbolPoints.Count + " SYMBOLS");
            Console.WriteLine("GOT " + symbolColumn.Points().Count() + " SYMBOLS");
            CollectionAssert.AreEqual(symbolPoints.ToArray(), symbolColumn.Points().ToArray());
        }

        [TestMethod]
        public void ThrowsColumnNotFound_GivenNonExistingColumns()
        {
            var ts = CreateTable();

            try
            {
                var batch = _cluster.Writer(new QdbBatchColumnDefinition(ts.Alias, "col"));
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
            QdbTable ts1 = CreateTable();
            QdbTable ts2 = CreateTable();
            var blobData      = CreateBlobPoints(startTime, 10);
            var doubleData    = CreateDoublePoints(startTime, 10);
            var int64Data     = CreateInt64Points(startTime, 10);
            var stringData    = CreateStringPoints(startTime, 10);
            var timestampData = CreateTimestampPoints(startTime, 10);
            var symbolData    = CreateSymbolPoints(startTime, 10);

            var batch = Insert(ts1, ts2, startTime, blobData, doubleData, int64Data, stringData, timestampData, symbolData);
            
            batch.Push();

            CheckTables(ts1, ts2, blobData, doubleData, int64Data, stringData, timestampData, symbolData);
        }

        [TestMethod]
        public void Ok_BulkRowFastInsert()
        {
            var startTime = DateTime.Now;
            QdbTable ts1 = CreateTable();
            QdbTable ts2 = CreateTable();
            var blobData      = CreateBlobPoints(startTime, 10);
            var doubleData    = CreateDoublePoints(startTime, 10);
            var int64Data     = CreateInt64Points(startTime, 10);
            var stringData    = CreateStringPoints(startTime, 10);
            var timestampData = CreateTimestampPoints(startTime, 10);
            var symbolData    = CreateSymbolPoints(startTime, 10);

            var batch = Insert(ts1, ts2, startTime, blobData, doubleData, int64Data, stringData, timestampData, symbolData);
            
            batch.PushFast();

            CheckTables(ts1, ts2, blobData, doubleData, int64Data, stringData, timestampData, symbolData);
        }

        [TestMethod]
        public void Ok_BulkRowAsyncInsert()
        {
            var startTime = DateTime.Now;
            QdbTable ts1 = CreateTable();
            QdbTable ts2 = CreateTable();
            var blobData      = CreateBlobPoints(startTime, 10);
            var doubleData    = CreateDoublePoints(startTime, 10);
            var int64Data     = CreateInt64Points(startTime, 10);
            var stringData    = CreateStringPoints(startTime, 10);
            var timestampData = CreateTimestampPoints(startTime, 10);
            var symbolData    = CreateSymbolPoints(startTime, 10);

            var batch = Insert(ts1, ts2, startTime, blobData, doubleData, int64Data, stringData, timestampData, symbolData);
            
            batch.PushAsync();

            // Wait for push_async to complete
            // Ideally we could be able to get the proper flush interval
            Thread.Sleep(8 * 1000);

            CheckTables(ts1, ts2, blobData, doubleData, int64Data, stringData, timestampData, symbolData);
        }
    }
}
