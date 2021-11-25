using System;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb;
using Quasardb.TimeSeries;
using Quasardb.TimeSeries.ExpWriter;

namespace Quasardb.Tests.Table
{
    [TestClass]
    public class ExpWriterTests
    {
        private readonly QdbCluster _cluster = QdbTestCluster.Instance;

        public DateTime[] MakeTimestamps(int count)
        {
            Random random = new Random();
            var r = new DateTime[count];

            DateTime date = DateTime.Parse("2021-01-01T00:00:00Z");
            for (int i = 0; i < count; ++i)
            {
                r[i] = date.AddSeconds(i);
            }
            return r;
        }

        public byte[][] MakeBlobArray(int count)
        {
            Random random = new Random();
            var r = new byte[count][];

            for (int i = 0; i < count; ++i)
            {
                var value = new byte[32];
                random.NextBytes(value);
                r[i] = value;
            }
            return r;
        }

        public double[] MakeDoubleArray(int count)
        {
            var r = new double[count];

            for (int i = 0; i < count; ++i)
            {
                r[i] = (double)i;
            }
            return r;
        }

        public long[] MakeInt64Array(int count)
        {
            var r = new long[count];

            for (int i = 0; i < count; ++i)
            {
                r[i] = (long)i;
            }
            return r;
        }
        
        public static string RandomString(int length, Random r)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[r.Next(s.Length)]).ToArray());
        }

        public string[] MakeStringArray(int count)
        {
            Random random = new Random();
            var r = new string[count];

            for (int i = 0; i < count; ++i)
            {
                r[i] = RandomString(32, random);
            }
            return r;
        }

        public DateTime[] CreateTimestampPoints(int count)
        {
            return MakeTimestamps(count);
        }

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

        public QdbTableExpWriter InsertByName(string ts,
            QdbTableExpWriterOptions options,
            byte[][] blobs,
            double[] doubles,
            long[] int64s,
            string[] strings,
            DateTime[] timestamps)
        {
            string[] tables = new string[1];
            tables[0] = ts;
            var batch = _cluster.ExpWriter(tables, options);
            batch.SetTimestamps(ts, timestamps);
            batch.SetBlobColumn(ts, "the_blob", blobs);
            batch.SetDoubleColumn(ts, "the_double", doubles);
            batch.SetInt64Column(ts, "the_int64", int64s);
            batch.SetStringColumn(ts, "the_string", strings);
            batch.SetTimestampColumn(ts, "the_ts", timestamps);
            batch.SetStringColumn(ts, "the_symbol", strings);
            return batch;
        }

        public QdbTableExpWriter Insert(string ts,
            QdbTableExpWriterOptions options,
            byte[][] blobs,
            double[] doubles,
            long[] int64s,
            string[] strings,
            DateTime[] timestamps)
        {
            string[] tables = new string[1];
            tables[0] = ts;
            var batch = _cluster.ExpWriter(tables, options);

            batch.SetTimestamps(0, timestamps);
            batch.SetBlobColumn(0, 0, blobs);
            batch.SetDoubleColumn(0, 1, doubles);
            batch.SetInt64Column(0, 2, int64s);
            batch.SetStringColumn(0, 3, strings);
            batch.SetTimestampColumn(0, 4, timestamps);
            batch.SetStringColumn(0, 5, strings);
            return batch;
        }

        public QdbTableExpWriter Insert(string ts,
            QdbTableExpWriterOptions options,
            byte[][] values,
            DateTime[] timestamps)
        {
            string[] tables = new string[1];
            tables[0] = ts;
            var batch = _cluster.ExpWriter(tables, options);
            batch.SetTimestamps(0, timestamps);
            batch.SetBlobColumn(0, 0, values);
            return batch;
        }

        public QdbTableExpWriter Insert(string ts,
            QdbTableExpWriterOptions options,
            double[] values,
            DateTime[] timestamps)
        {
            string[] tables = new string[1];
            tables[0] = ts;
            var batch = _cluster.ExpWriter(tables, options);
            batch.SetTimestamps(0, timestamps);
            batch.SetDoubleColumn(0, 1, values);
            return batch;
        }

        public QdbTableExpWriter Insert(string ts,
            QdbTableExpWriterOptions options,
            long[] values,
            DateTime[] timestamps)
        {
            string[] tables = new string[1];
            tables[0] = ts;
            var batch = _cluster.ExpWriter(tables, options);
            batch.SetTimestamps(0, timestamps);
            batch.SetInt64Column(0, 2, values);
            return batch;
        }

        public QdbTableExpWriter Insert(string ts,
            QdbTableExpWriterOptions options,
            string[] values,
            DateTime[] timestamps)
        {
            string[] tables = new string[1];
            tables[0] = ts;
            var batch = _cluster.ExpWriter(tables, options);
            batch.SetTimestamps(0, timestamps);
            batch.SetStringColumn(0, 3, values);
            return batch;
        }

        public QdbTableExpWriter Insert(string ts,
            QdbTableExpWriterOptions options,
            DateTime[] values,
            DateTime[] timestamps)
        {
            string[] tables = new string[1];
            tables[0] = ts;
            var batch = _cluster.ExpWriter(tables, options);
            batch.SetTimestamps(0, timestamps);
            batch.SetTimestampColumn(0, 4, values);
            return batch;
        }

        public QdbTableExpWriter InsertSymbol(string ts,
            QdbTableExpWriterOptions options,
            string[] values,
            DateTime[] timestamps)
        {
            string[] tables = new string[1];
            tables[0] = ts;
            var batch = _cluster.ExpWriter(tables, options);
            batch.SetTimestamps(0, timestamps);
            batch.SetStringColumn(0, 5, values);
            return batch;
        }

        public void CheckTables(QdbTable ts,
            byte[][] blobs,
            double[] doubles,
            long[] ints,
            string[] strings,
            DateTime[] timestamps)
        {
            var blob_arr = ts.BlobColumns["the_blob"].Points().ToArray();
            var double_arr = ts.DoubleColumns["the_double"].Points().ToArray();
            var int_arr = ts.Int64Columns["the_int64"].Points().ToArray();
            var string_arr = ts.StringColumns["the_string"].Points().ToArray();
            var ts_arr = ts.TimestampColumns["the_ts"].Points().ToArray();
            var symbol_arr = ts.SymbolColumns["the_symbol"].Points().ToArray();
            for (int idx = 0; idx < timestamps.Length; idx++)
            {
                Assert.AreEqual(blob_arr[idx].Time, timestamps[idx]);
                CollectionAssert.AreEqual(blob_arr[idx].Value, blobs[idx]);
                Assert.AreEqual(double_arr[idx].Value, doubles[idx]);
                Assert.AreEqual(int_arr[idx].Value, ints[idx]);
                Assert.AreEqual(string_arr[idx].Value, strings[idx]);
                Assert.AreEqual(ts_arr[idx].Value, timestamps[idx]);
                Assert.AreEqual(symbol_arr[idx].Value, strings[idx]);
            }
        }

        public void CheckColumn(QdbTable ts,
            byte[][] values,
            DateTime[] timestamps)
        {
            var arr = ts.BlobColumns["the_blob"].Points().ToArray();
            for (int idx = 0; idx < timestamps.Length; idx++)
            {
                Assert.AreEqual(arr[idx].Time, timestamps[idx]);
                CollectionAssert.AreEqual(arr[idx].Value, values[idx]);
            }
        }

        public void CheckColumn(QdbTable ts,
            double[] values,
            DateTime[] timestamps)
        {
            var arr = ts.DoubleColumns["the_double"].Points().ToArray();
            for (int idx = 0; idx < timestamps.Length; idx++)
            {
                Assert.AreEqual(arr[idx].Time, timestamps[idx]);
                Assert.AreEqual(arr[idx].Value, values[idx]);
            }
        }

        public void CheckColumn(QdbTable ts,
            long[] values,
            DateTime[] timestamps)
        {
            var arr = ts.Int64Columns["the_int64"].Points().ToArray();
            for (int idx = 0; idx < timestamps.Length; idx++)
            {
                Assert.AreEqual(arr[idx].Time, timestamps[idx]);
                Assert.AreEqual(arr[idx].Value, values[idx]);
            }
        }

        public void CheckColumn(QdbTable ts,
            string[] values,
            DateTime[] timestamps)
        {
            var arr = ts.StringColumns["the_string"].Points().ToArray();
            for (int idx = 0; idx < timestamps.Length; idx++)
            {
                Assert.AreEqual(arr[idx].Time, timestamps[idx]);
                Assert.AreEqual(arr[idx].Value, values[idx]);
            }
        }

        public void CheckColumn(QdbTable ts,
            DateTime[] values,
            DateTime[] timestamps)
        {
            var arr = ts.TimestampColumns["the_ts"].Points().ToArray();
            for (int idx = 0; idx < timestamps.Length; idx++)
            {
                Assert.AreEqual(arr[idx].Time, timestamps[idx]);
                Assert.AreEqual(arr[idx].Value, values[idx]);
            }
        }

        public void CheckSymbolColumn(QdbTable ts,
            string[] values,
            DateTime[] timestamps)
        {
            var arr = ts.SymbolColumns["the_symbol"].Points().ToArray();
            for (int idx = 0; idx < timestamps.Length; idx++)
            {
                Assert.AreEqual(arr[idx].Time, timestamps[idx]);
                Assert.AreEqual(arr[idx].Value, values[idx]);
            }
        }

        [TestMethod]
        public void Ok_BulkRowInsert()
        {
            QdbTable ts = CreateTable();

            var blobs      = MakeBlobArray(10);
            var doubles    = MakeDoubleArray(10);
            var int64s     = MakeInt64Array(10);
            var strings    = MakeStringArray(10);
            var timestamps = MakeTimestamps(10);

            var batch = Insert(ts.Alias, new QdbTableExpWriterOptions().Transactional(), blobs, doubles, int64s, strings, timestamps);

            batch.Push();

            CheckTables(ts, blobs, doubles, int64s, strings, timestamps);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbException))]
        public void Ok_BulkRowInsertWithWrongName()
        {
            QdbTable ts = CreateTable();

            var blobs = MakeBlobArray(10);
            var timestamps = MakeTimestamps(10);

            string[] tables = new string[1];
            tables[0] = ts.Alias;

            var batch = _cluster.ExpWriter(tables, new QdbTableExpWriterOptions().Transactional());
            batch.SetTimestamps(ts.Alias, timestamps);
            batch.SetBlobColumn(ts.Alias, "the_wrong_blob", blobs);

            batch.Push();
        }

        //[TestMethod]
        //public void Ok_BulkRowInsertBlobs()
        //{
        //    QdbTable ts = CreateTable();

        //    var values     = MakeBlobArray(10);
        //    var timestamps = MakeTimestamps(10);

        //    var batch = Insert(ts.Alias, new QdbTableExpWriterOptions().Transactional(), values, timestamps);

        //    batch.Push();

        //    CheckColumn(ts, values, timestamps);
        //}

        //[TestMethod]
        //public void Ok_BulkRowInsertDoubles()
        //{
        //    QdbTable ts = CreateTable();

        //    var values      = MakeDoubleArray(10);
        //    var timestamps = MakeTimestamps(10);

        //    var batch = Insert(ts.Alias, new QdbTableExpWriterOptions().Transactional(), values, timestamps);

        //    batch.Push();

        //    CheckColumn(ts, values, timestamps);
        //}

        //[TestMethod]
        //public void Ok_BulkRowInsertInts()
        //{
        //    QdbTable ts = CreateTable();

        //    var values     = MakeInt64Array(10);
        //    var timestamps = MakeTimestamps(10);

        //    var batch = Insert(ts.Alias, new QdbTableExpWriterOptions().Transactional(), values, timestamps);

        //    batch.Push();

        //    CheckColumn(ts, values, timestamps);
        //}

        //[TestMethod]
        //public void Ok_BulkRowInsertStrings()
        //{
        //    QdbTable ts = CreateTable();

        //    var values      = MakeStringArray(10);
        //    var timestamps = MakeTimestamps(10);

        //    var batch = Insert(ts.Alias, new QdbTableExpWriterOptions().Transactional(), values, timestamps);

        //    batch.Push();

        //    CheckColumn(ts, values, timestamps);
        //}

        //[TestMethod]
        //public void Ok_BulkRowInsertTimestamps()
        //{
        //    QdbTable ts = CreateTable();

        //    var values     = MakeTimestamps(10);
        //    var timestamps = MakeTimestamps(10);

        //    var batch = Insert(ts.Alias, new QdbTableExpWriterOptions().Transactional(), values, timestamps);

        //    batch.Push();

        //    CheckColumn(ts, values, timestamps);
        //}

        //[TestMethod]
        //public void Ok_BulkRowInsertSymbols()
        //{
        //    QdbTable ts = CreateTable();

        //    var values      = MakeStringArray(10);
        //    var timestamps = MakeTimestamps(10);

        //    var batch = InsertSymbol(ts.Alias, new QdbTableExpWriterOptions().Transactional(), values, timestamps);

        //    batch.Push();

        //    CheckSymbolColumn(ts, values, timestamps);
        //}

        [TestMethod]
        public void Ok_BulkRowInsertByName()
        {
            QdbTable ts = CreateTable();

            var blobs      = MakeBlobArray(10);
            var doubles    = MakeDoubleArray(10);
            var int64s     = MakeInt64Array(10);
            var strings    = MakeStringArray(10);
            var timestamps = MakeTimestamps(10);

            var batch = InsertByName(ts.Alias, new QdbTableExpWriterOptions().Transactional(), blobs, doubles, int64s, strings, timestamps);

            batch.Push();

            CheckTables(ts, blobs, doubles, int64s, strings, timestamps);
        }

        [TestMethod]
        public void Ok_BulkRowInsertFast()
        {
            QdbTable ts = CreateTable();

            var blobs = MakeBlobArray(10);
            var doubles = MakeDoubleArray(10);
            var int64s = MakeInt64Array(10);
            var strings = MakeStringArray(10);
            var timestamps = MakeTimestamps(10);

            var batch = Insert(ts.Alias, new QdbTableExpWriterOptions().Fast(), blobs, doubles, int64s, strings, timestamps);

            batch.Push();

            CheckTables(ts, blobs, doubles, int64s, strings, timestamps);
        }

        [TestMethod]
        public void Ok_BulkRowInsertFastByName()
        {
            QdbTable ts = CreateTable();

            var blobs = MakeBlobArray(10);
            var doubles = MakeDoubleArray(10);
            var int64s = MakeInt64Array(10);
            var strings = MakeStringArray(10);
            var timestamps = MakeTimestamps(10);

            var batch = InsertByName(ts.Alias, new QdbTableExpWriterOptions().Fast(), blobs, doubles, int64s, strings, timestamps);

            batch.Push();

            CheckTables(ts, blobs, doubles, int64s, strings, timestamps);
        }

        [TestMethod]
        public void Ok_BulkRowInsertASync()
        {
            QdbTable ts = CreateTable();

            var blobs = MakeBlobArray(10);
            var doubles = MakeDoubleArray(10);
            var int64s = MakeInt64Array(10);
            var strings = MakeStringArray(10);
            var timestamps = MakeTimestamps(10);

            var batch = Insert(ts.Alias, new QdbTableExpWriterOptions().Async(), blobs, doubles, int64s, strings, timestamps);

            batch.Push();

            // Wait for push_async to complete
            // Ideally we could be able to get the proper flush interval
            Thread.Sleep(8 * 1000);

            CheckTables(ts, blobs, doubles, int64s, strings, timestamps);
        }

        [TestMethod]
        public void Ok_BulkRowInsertASyncByName()
        {
            QdbTable ts = CreateTable();

            var blobs = MakeBlobArray(10);
            var doubles = MakeDoubleArray(10);
            var int64s = MakeInt64Array(10);
            var strings = MakeStringArray(10);
            var timestamps = MakeTimestamps(10);

            var batch = InsertByName(ts.Alias, new QdbTableExpWriterOptions().Async(), blobs, doubles, int64s, strings, timestamps);

            batch.Push();

            // Wait for push_async to complete
            // Ideally we could be able to get the proper flush interval
            Thread.Sleep(8 * 1000);

            CheckTables(ts, blobs, doubles, int64s, strings, timestamps);
        }

        [TestMethod]
        public void Ok_BulkRowInsertTruncate()
        {
            QdbTable ts = CreateTable();
            var timestamps = MakeTimestamps(10);

            {
                var values = MakeBlobArray(10);

                var batch = Insert(ts.Alias, new QdbTableExpWriterOptions().Transactional(), values, timestamps);
                batch.Push();

                CheckColumn(ts, values, timestamps);
            }

            {
                var values = MakeBlobArray(10);

                var begin = timestamps[0];
                var end = timestamps[timestamps.Length - 1].AddSeconds(1);
                QdbTimeInterval interval = new QdbTimeInterval(begin, end);
                var batch = Insert(ts.Alias, new QdbTableExpWriterOptions().Truncate(interval), values, timestamps);
                batch.Push();

                CheckColumn(ts, values, timestamps);
            }
        }
    }
}
