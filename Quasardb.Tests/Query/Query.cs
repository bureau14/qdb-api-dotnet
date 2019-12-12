using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.Query;
using Quasardb.TimeSeries;

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
                new QdbTimestampColumnDefinition("the_ts"),
            });
            return ts;
        }

        public QdbBlobPointCollection InsertBlobPoints(QdbTable ts, DateTime time, int count)
        {
            Random random = new Random();
            var r = new QdbBlobPointCollection(count);

            var column = ts.BlobColumns["the_blob"];
            for (int i = 0; i < count; ++i)
            {
                var value = new byte[32];
                random.NextBytes(value);
                column.Insert(time, value);
                r.Add(time, value);
                time = time.AddSeconds(1);
            }
            return r;
        }

        public QdbDoublePointCollection InsertDoublePoints(QdbTable ts, DateTime time, int count)
        {
            Random random = new Random();
            var r = new QdbDoublePointCollection(count);

            var column = ts.DoubleColumns["the_double"];
            for (int i = 0; i < count; ++i)
            {
                var value = random.NextDouble();
                column.Insert(time, value);
                r.Add(time, value);
                time = time.AddSeconds(1);
            }
            return r;
        }

        public QdbInt64PointCollection InsertInt64Points(QdbTable ts, DateTime time, int count)
        {
            Random random = new Random();
            var r = new QdbInt64PointCollection(count);

            var column = ts.Int64Columns["the_int64"];
            for (int i = 0; i < count; ++i)
            {
                var value = random.Next();
                column.Insert(time, value);
                r.Add(time, value);
                time = time.AddSeconds(1);
            }
            return r;
        }

        public QdbTimestampPointCollection InsertTimestampPoints(QdbTable ts, DateTime time, int count)
        {
            Random random = new Random();
            var r = new QdbTimestampPointCollection(count);

            var column = ts.TimestampColumns["the_ts"];
            for (int i = 0; i < count; ++i)
            {
                var value = DateTime.Today.AddSeconds(random.NextDouble());
                column.Insert(time, value);
                r.Add(time, value);
                time = time.AddSeconds(1);
            }
            return r;
        }

        private void CheckColumns(QdbColumnNameCollection columns)
        {
            Assert.AreEqual(6, columns.Count);
            Assert.AreEqual("$timestamp", columns[0]);
            Assert.AreEqual("$table", columns[1]);
            Assert.AreEqual("the_blob", columns[2]);
            Assert.AreEqual("the_double", columns[3]);
            Assert.AreEqual("the_int64", columns[4]);
            Assert.AreEqual("the_ts", columns[5]);
        }

        #region Query failure tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
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
        public void ReturnsInsertedDataWithStarSelect()
        {
            var startTime = DateTime.Now;
            QdbTable ts = CreateTable();
            var insertedDoubleData = InsertDoublePoints(ts, startTime, 10);
            try
            {
                var results = _cluster.Query("select * from " + ts.Alias);
                CheckColumns(results.ColumnNames);

                var rows = results.Rows;
                Assert.AreEqual(10L, rows.Count);
                for (int i = 0; i < 10L; ++i)
                {
                    var row = rows[i];
                    Assert.AreEqual(null, row["the_blob"].Value);
                    Assert.AreEqual(insertedDoubleData[i].Value, row["the_double"].Value);
                    Assert.AreEqual(null, row["the_int64"].Value);
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
                for (int i = 0; i < 10L; ++i)
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
                for (int i = 0; i < 10L; ++i)
                {
                    var row = rows[i];
                    Assert.AreEqual(startTime.AddSeconds(i), row[0].TimestampValue);
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
            var insertedBlobData = InsertBlobPoints(ts, startTime, 10);
            var insertedDoubleData = InsertDoublePoints(ts, startTime, 10);
            var insertedInt64Data = InsertInt64Points(ts, startTime, 10);
            var insertedTimestampData = InsertTimestampPoints(ts, startTime, 10);
            try
            {
                var results = _cluster.Query("select * from " + ts.Alias);
                CheckColumns(results.ColumnNames);

                var rows = results.Rows;
                Assert.AreEqual(10L, rows.Count);
                for (int i = 0; i < 10L; ++i)
                {
                    var row = rows[i];
                    CollectionAssert.AreEqual(insertedBlobData[i].Value, row["the_blob"].BlobValue);
                    Assert.AreEqual(insertedDoubleData[i].Value, row["the_double"].Value);
                    Assert.AreEqual(insertedInt64Data[i].Value, row["the_int64"].Value);
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
        public void ReturnsInsertedMultiDataWithStarSelect_WithNulls()
        {
            var startTime = DateTime.Now;
            QdbTable ts = CreateTable();
            var insertedBlobData = InsertBlobPoints(ts, startTime, 9);
            var insertedDoubleData = InsertDoublePoints(ts, startTime, 9);
            var insertedInt64Data = InsertInt64Points(ts, startTime, 9);
            var insertedTimestampData = InsertTimestampPoints(ts, startTime, 9);

            ts.BlobColumns["the_blob"].Insert(startTime.AddSeconds(9), new byte[] { 10 });
            insertedBlobData.Add(startTime.AddSeconds(9), new byte[] { 10 });
            insertedDoubleData.Add(startTime.AddSeconds(9), null);
            insertedInt64Data.Add(startTime.AddSeconds(9), null);
            insertedTimestampData.Add(startTime.AddSeconds(9), null);
            try
            {
                var results = _cluster.Query("select * from " + ts.Alias);
                CheckColumns(results.ColumnNames);

                var rows = results.Rows;
                Assert.AreEqual(10L, rows.Count);
                for (int i = 0; i < 10L; ++i)
                {
                    var row = rows[i];
                    CollectionAssert.AreEqual(insertedBlobData[i].Value, row["the_blob"].BlobValue);
                    Assert.AreEqual(insertedDoubleData[i].Value, row["the_double"].Value);
                    Assert.AreEqual(insertedInt64Data[i].Value, row["the_int64"].Value);
                    Assert.AreEqual(insertedTimestampData[i].Value, row["the_ts"].Value);
                    Assert.AreEqual(ts.Alias, row["$table"].StringValue);
                }
            }
            finally
            {
                ts.Remove();
            }
        }

        #endregion
    }
}
