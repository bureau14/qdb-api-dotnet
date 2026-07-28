using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.Query;
using Quasardb.TimeSeries;
using Quasardb.TimeSeries.ExpWriter;
using System;
using System.Linq;
using System.Text;

namespace Quasardb.Tests.Query
{
    [TestClass]
    public class Query
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
            });
            return ts;
        }

        public QdbBlobPointCollection InsertBlobPoints(QdbTable ts, DateTime time, int count)
        {
            if (ts == null) throw new ArgumentNullException(nameof(ts));

            var random = new Random();
            var r = new QdbBlobPointCollection(count);
            var writer = _cluster.ExpWriter(new string[] { ts.Alias }, new QdbTableExpWriterOptions().Transactional());
            for (var i = 0; i < count; ++i)
            {
                var value = new byte[32];
                random.NextBytes(value);
                writer.Add(ts.Alias, time, new object[] { value, null, null, null, null });
                r.Add(time, value);
                time = time.AddSeconds(1);
            }
            writer.Push();
            return r;
        }

        private static QdbBlobPointCollection CreateBlobPoints(DateTime time, int count)
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

        public QdbDoublePointCollection InsertDoublePoints(QdbTable ts, DateTime time, int count)
        {
            if (ts == null) throw new ArgumentNullException(nameof(ts));

            var random = new Random();
            var r = new QdbDoublePointCollection(count);
            var writer = _cluster.ExpWriter(new string[] { ts.Alias }, new QdbTableExpWriterOptions().Transactional());
            for (var i = 0; i < count; ++i)
            {
                var value = random.NextDouble();
                writer.Add(ts.Alias, time, new object[] { null, value, null, null, null });
                r.Add(time, value);
                time = time.AddSeconds(1);
            }
            writer.Push();
            return r;
        }

        private static QdbDoublePointCollection CreateDoublePoints(DateTime time, int count)
        {
            var random = new Random();
            var r = new QdbDoublePointCollection(count);
            for (var i = 0; i < count; ++i)
            {
                var value = random.NextDouble();
                r.Add(time, value);
                time = time.AddSeconds(1);
            }
            return r;
        }

        public QdbInt64PointCollection InsertInt64Points(QdbTable ts, DateTime time, int count)
        {
            if (ts == null) throw new ArgumentNullException(nameof(ts));

            var random = new Random();
            var r = new QdbInt64PointCollection(count);
            var writer = _cluster.ExpWriter(new string[] { ts.Alias }, new QdbTableExpWriterOptions().Transactional());
            for (var i = 0; i < count; ++i)
            {
                var value = random.Next();
                writer.Add(ts.Alias, time, new object[] { null, null, value, null, null });
                r.Add(time, value);
                time = time.AddSeconds(1);
            }
            writer.Push();
            return r;
        }

        private static QdbInt64PointCollection CreateInt64Points(DateTime time, int count)
        {
            var random = new Random();
            var r = new QdbInt64PointCollection(count);
            for (var i = 0; i < count; ++i)
            {
                var value = random.Next();
                r.Add(time, value);
                time = time.AddSeconds(1);
            }
            return r;
        }

        public static string GenerateRandomAlphanumericString(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            var random = new Random();
            var randomString = new string(Enumerable.Repeat(chars, length)
                                                    .Select(s => s[random.Next(s.Length)]).ToArray());
            return randomString;
        }

        public QdbStringPointCollection InsertStringPoints(QdbTable ts, DateTime time, int count)
        {
            if (ts == null) throw new ArgumentNullException(nameof(ts));

            var r = new QdbStringPointCollection(count);
            var writer = _cluster.ExpWriter(new string[] { ts.Alias }, new QdbTableExpWriterOptions().Transactional());
            for (var i = 0; i < count; ++i)
            {
                var value = GenerateRandomAlphanumericString(32);

                writer.Add(ts.Alias, time, new object[] { null, null, null, value, null });
                r.Add(time, value);
                time = time.AddSeconds(1);
            }
            writer.Push();
            return r;
        }

        private static QdbStringPointCollection CreateStringPoints(DateTime time, int count)
        {
            var r = new QdbStringPointCollection(count);
            for (var i = 0; i < count; ++i)
            {
                var value = GenerateRandomAlphanumericString(32);
                r.Add(time, value);
                time = time.AddSeconds(1);
            }
            return r;
        }

        public QdbStringPointCollection InsertInvalidStringPoints(QdbTable ts, DateTime time, int count)
        {
            if (ts == null) throw new ArgumentNullException(nameof(ts));

            var r = new QdbStringPointCollection(count);
            var writer = _cluster.ExpWriter(new string[] { ts.Alias }, new QdbTableExpWriterOptions().Transactional());

            byte[] bytes = { (byte)'\xfe', (byte)'\xfe', (byte)'\xff', (byte)'\xff' };
            var value = Encoding.UTF8.GetString(bytes);

            for (var i = 0; i < count; ++i)
            {
                writer.Add(ts.Alias, time, new object[] { null, null, null, value, null });
                r.Add(time, value);
                time = time.AddSeconds(1);
            }
            writer.Push();
            return r;
        }

        public QdbTimestampPointCollection InsertTimestampPoints(QdbTable ts, DateTime time, int count)
        {
            if (ts == null) throw new ArgumentNullException(nameof(ts));

            var random = new Random();
            var r = new QdbTimestampPointCollection(count);
            var writer = _cluster.ExpWriter(new string[] { ts.Alias }, new QdbTableExpWriterOptions().Transactional());
            for (var i = 0; i < count; ++i)
            {
                var value = DateTime.Today.AddSeconds(random.NextDouble());
                writer.Add(ts.Alias, time, new object[] { null, null, null, null, value });
                r.Add(time, value);
                time = time.AddSeconds(1);
            }
            writer.Push();
            return r;
        }

        private static QdbTimestampPointCollection CreateTimestampPoints(DateTime time, int count)
        {
            var random = new Random();
            var r = new QdbTimestampPointCollection(count);
            for (var i = 0; i < count; ++i)
            {
                var value = DateTime.Today.AddSeconds(random.NextDouble());
                r.Add(time, value);
                time = time.AddSeconds(1);
            }
            return r;
        }

        private void InsertRows(
            QdbTable ts,
            QdbBlobPointCollection blobs,
            QdbDoublePointCollection doubles,
            QdbInt64PointCollection int64s,
            QdbStringPointCollection strings,
            QdbTimestampPointCollection timestamps)
        {
            if (ts == null) throw new ArgumentNullException(nameof(ts));
            if (blobs == null) throw new ArgumentNullException(nameof(blobs));
            if (doubles == null) throw new ArgumentNullException(nameof(doubles));
            if (int64s == null) throw new ArgumentNullException(nameof(int64s));
            if (strings == null) throw new ArgumentNullException(nameof(strings));
            if (timestamps == null) throw new ArgumentNullException(nameof(timestamps));

            var writer = _cluster.ExpWriter(new string[] { ts.Alias }, new QdbTableExpWriterOptions().Transactional());

            for (var i = 0; i < blobs.Count; ++i)
            {
                writer.Add(ts.Alias, blobs[i].Time, new object[]
                {
                    blobs[i].Value,
                    doubles[i].Value,
                    int64s[i].Value,
                    strings[i].Value,
                    timestamps[i].Value
                });
            }

            writer.Push();
        }

        private static void CheckColumns(QdbColumnNameCollection columns)
        {
            Assert.AreEqual(7, columns.Count);
            Assert.AreEqual("$timestamp", columns[0]);
            Assert.AreEqual("$table", columns[1]);
            Assert.AreEqual("the_blob", columns[2]);
            Assert.AreEqual("the_double", columns[3]);
            Assert.AreEqual("the_int64", columns[4]);
            Assert.AreEqual("the_string", columns[5]);
            Assert.AreEqual("the_ts", columns[6]);
        }

        #region Query failure tests

        [TestMethod]
        [ExpectedException(typeof(QdbQueryException))]
        public void ThrowsForNullQuery()
        {
            _cluster.Query(null);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbQueryException))]
        public void ThrowsForEmptyQuery()
        {
            _cluster.Query("");
        }

        [TestMethod]
        [ExpectedException(typeof(QdbQueryException))]
        public void ThrowsForInvalidQuery()
        {
            _cluster.Query("select * from");
        }

        [TestMethod]
        [ExpectedException(typeof(QdbQueryException))]
        public void ThrowsWhenTsDoesntExist()
        {
            _cluster.Query("select * from this_ts_doesnt_exist in range(2017, +10d)");
        }

        [TestMethod]
        [ExpectedException(typeof(QdbQueryException))]
        public void ThrowsWhenUntagged()
        {
            _cluster.Query("select * from find(tag='this_tag_doesnt_exist') in range(2017, +10d)");
        }

        [TestMethod]
        [ExpectedException(typeof(QdbQueryException))]
        public void ThrowsWhenColumnNotFound()
        {
            var ts = _cluster.Table(RandomGenerator.CreateUniqueAlias());
            ts.Create(new QdbColumnDefinition[] { });
            try
            {
                _cluster.Query("select this_column_doesnt_exist from " + ts.Alias + " in range(2017, +10d)");
            }
            finally
            {
                ts.Remove();
            }
        }

        #endregion

        #region Data tests

        [TestMethod]
        public void ReturnsEmptyResult()
        {
            QdbTable ts = CreateTable();
            try
            {
                var results = _cluster.Query("select * from " + ts.Alias + " in range(2016, +1y)");
                CheckColumns(results.ColumnNames);

                var rows = results.Rows;
                Assert.AreEqual(0, rows.Count);
            }
            finally
            {
                ts.Remove();
            }
        }

        [TestMethod]
        public void ReturnsEmptyResultWhenNoResultsAreAvailable()
        {
            QdbTable ts = CreateTable();
            try
            {
                var results = _cluster.Query("DROP TABLE " + ts.Alias);

                var rows = results.Rows;
                Assert.AreEqual(0, rows.Count);
            }
            finally
            {
            }
        }

        [TestMethod]
        public void ReturnsNullValues()
        {
            QdbTable ts = CreateTable();
            for (var i = 0; i < 10L; ++i)
            {
                _cluster.Query($"INSERT INTO {ts.Alias} ($timestamp, the_blob, the_double, the_int64, the_string, the_ts) VALUES (now(), NULL, NULL, NULL, NULL, NULL)");
            }
            try
            {
                var results = _cluster.Query("select * from " + ts.Alias);
                CheckColumns(results.ColumnNames);

                var rows = results.Rows;
                Assert.AreEqual(10L, rows.Count);
                for (var i = 0; i < 10L; ++i)
                {
                    var row = rows[i];
                    Assert.AreEqual(null, row["the_blob"].Value);
                    Assert.AreEqual(null, row["the_double"].Value);
                    Assert.AreEqual(null, row["the_int64"].Value);
                    Assert.AreEqual(null, row["the_string"].Value);
                    Assert.AreEqual(null, row["the_ts"].Value);
                    Assert.AreEqual(ts.Alias, row["$table"].StringValue);
                }
            }
            finally
            {
                ts.Remove();
            }
        }

        [TestMethod]
        public void ReturnsInsertedDataWithColumnSelect()
        {
            var startTime = DateTime.Now;
            QdbTable ts = CreateTable();
            var insertedDoubleData = InsertDoublePoints(ts, startTime, 10);
            try
            {
                var results = _cluster.Query("select the_double from " + ts.Alias);
                Assert.AreEqual(1L, results.ColumnNames.Count);
                Assert.AreEqual("the_double", results.ColumnNames[0]);

                var rows = results.Rows;
                Assert.AreEqual(10L, rows.Count);
                for (var i = 0; i < 10L; ++i)
                {
                    var row = rows[i];
                    Assert.AreEqual(insertedDoubleData[i].Value, row[0].Value);
                }
            }
            finally
            {
                ts.Remove();
            }
        }

        [TestMethod]
        public void ReturnsInsertedDataWithSpecificSelect()
        {
            var startTime = DateTime.Now;
            QdbTable ts = CreateTable();
            var insertedDoubleData = InsertDoublePoints(ts, startTime, 10);
            try
            {
                var results = _cluster.Query("select $timestamp, $table, the_double from " + ts.Alias);
                Assert.AreEqual(3, results.ColumnNames.Count);
                Assert.AreEqual("$timestamp", results.ColumnNames[0]);
                Assert.AreEqual("$table", results.ColumnNames[1]);
                Assert.AreEqual("the_double", results.ColumnNames[2]);

                var rows = results.Rows;
                Assert.AreEqual(10L, rows.Count);
                for (var i = 0; i < 10L; ++i)
                {
                    var row = rows[i];
                    Assert.AreEqual(startTime.AddSeconds(i).ToLocalTime(), row[0].TimestampValue);
                    Assert.AreEqual(ts.Alias, row[1].StringValue);
                    Assert.AreEqual(insertedDoubleData[i].Value, row[2].Value);
                }
            }
            finally
            {
                ts.Remove();
            }
        }

        [TestMethod]
        public void ReturnsInsertedDataWithCountSelect()
        {
            var startTime = DateTime.Now;
            QdbTable ts = CreateTable();
            var insertedDoubleData = InsertDoublePoints(ts, startTime, 10);
            try
            {
                var results = _cluster.Query("select count(the_double) from " + ts.Alias);
                Assert.AreEqual(1L, results.ColumnNames.Count);
                Assert.AreEqual("count(the_double)", results.ColumnNames[0]);

                var rows = results.Rows;
                Assert.AreEqual(1L, rows.Count);
                Assert.AreEqual(10L, rows[0]["count(the_double)"].Value);
            }
            finally
            {
                ts.Remove();
            }
        }

        [TestMethod]
        public void ReturnsInsertedDataWithSumSelect()
        {
            var startTime = DateTime.Now;
            QdbTable ts = CreateTable();
            var insertedDoubleData = InsertDoublePoints(ts, startTime, 10);
            try
            {
                var results = _cluster.Query("select sum(the_double) from " + ts.Alias);
                Assert.AreEqual(1L, results.ColumnNames.Count);
                Assert.AreEqual("sum(the_double)", results.ColumnNames[0]);

                var rows = results.Rows;
                Assert.AreEqual(1L, rows.Count);
                Assert.AreEqual(insertedDoubleData.Select(x => x.Value).Sum(), rows[0]["sum(the_double)"].Value);
            }
            finally
            {
                ts.Remove();
            }
        }

        [TestMethod]
        public void ReturnsInsertedMultiDataWithStarSelect()
        {
            var startTime = DateTime.Now;
            QdbTable ts = CreateTable();
            var insertedBlobData = CreateBlobPoints(startTime, 10);
            var insertedDoubleData = CreateDoublePoints(startTime, 10);
            var insertedInt64Data = CreateInt64Points(startTime, 10);
            var insertedStringData = CreateStringPoints(startTime, 10);
            var insertedTimestampData = CreateTimestampPoints(startTime, 10);
            InsertRows(ts, insertedBlobData, insertedDoubleData, insertedInt64Data, insertedStringData, insertedTimestampData);
            try
            {
                var results = _cluster.Query("select * from " + ts.Alias);
                CheckColumns(results.ColumnNames);

                var rows = results.Rows;
                Assert.AreEqual(10L, rows.Count);
                for (var i = 0; i < 10L; ++i)
                {
                    var row = rows[i];
                    CollectionAssert.AreEqual(insertedBlobData[i].Value, row["the_blob"].BlobValue);
                    Assert.AreEqual(insertedDoubleData[i].Value, row["the_double"].Value);
                    Assert.AreEqual(insertedInt64Data[i].Value, row["the_int64"].Value);
                    Assert.AreEqual(insertedStringData[i].Value, row["the_string"].Value);
                    Assert.AreEqual(insertedTimestampData[i].Value, row["the_ts"].Value);
                    Assert.AreEqual(ts.Alias, row["$table"].StringValue);
                }
            }
            finally
            {
                ts.Remove();
            }
        }

        [TestMethod]
        public void ReturnsArrayAccum()
        {
            string alias = RandomGenerator.CreateUniqueAlias();
            _cluster.Query(String.Format("CREATE TABLE {0} ($timestamp TIMESTAMP, col1 DOUBLE)", alias));
            _cluster.Query(String.Format("INSERT INTO {0} ($timestamp, col1) values (2017-01-01, 10), (2017-01-01, 20), (2017-01-01, null), (2017-01-01, 21)", alias));

            var results = _cluster.Query(String.Format("SELECT $timestamp, array_accum(col1) FROM {0} in range(2017, +1d) group by 1day", alias));
            var rows = results.Rows;
            Assert.AreEqual(2, results.ColumnNames.Count);
            Assert.AreEqual("$timestamp", results.ColumnNames[0]);
            Assert.AreEqual("array_accum(col1)", results.ColumnNames[1]);

            double[] values = (double[])results.Rows[0]["array_accum(col1)"].Value;
            Assert.AreEqual(10, values[0]);
            Assert.AreEqual(20, values[1]);
            Assert.AreEqual(Double.NaN, values[2]);
            Assert.AreEqual(21, values[3]);
        }

        #endregion
    }
}
