using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.TimeSeries;
using Quasardb.TimeSeries.ExpWriter;

namespace Quasardb.Tests.Table
{
    [TestClass]
    public class ReaderTests
    {
        readonly QdbCluster _cluster = QdbTestCluster.Instance;

        [TestMethod]
        public void ReaderToArrayReturnsSnapshotRows()
        {
            var ts = TableTestHelper.CreateTableWithoutSymbol(_cluster);
            var timestamps = TableTestHelper.MakeTimestamps(2);
            var blobs = TableTestHelper.MakeBlobArray(2);
            var doubles = new[] { 1.0, 2.0 };
            var int64s = new long[] { 1L, 2L };
            var strings = new[] { "one", "two" };

            TableTestHelper.InsertRowsWithoutSymbol(_cluster, ts, blobs, doubles, int64s, strings, timestamps);

            var rows = ts.Reader().ToArray();

            Assert.AreEqual(2, rows.Length);
            Assert.AreNotSame(rows[0], rows[1]);

            CollectionAssert.AreEqual(blobs[0], rows[0]["the_blob"].BlobValue);
            CollectionAssert.AreEqual(blobs[1], rows[1]["the_blob"].BlobValue);
            Assert.AreEqual(timestamps[0], rows[0].Timestamp);
            Assert.AreEqual(timestamps[1], rows[1].Timestamp);
            Assert.AreEqual(doubles[0], rows[0]["the_double"].DoubleValue);
            Assert.AreEqual(doubles[1], rows[1]["the_double"].DoubleValue);
            Assert.AreEqual(int64s[0], rows[0]["the_int64"].Int64Value);
            Assert.AreEqual(int64s[1], rows[1]["the_int64"].Int64Value);
            Assert.AreEqual(strings[0], rows[0]["the_string"].StringValue);
            Assert.AreEqual(strings[1], rows[1]["the_string"].StringValue);
            Assert.AreEqual(timestamps[0], rows[0]["the_ts"].TimestampValue);
            Assert.AreEqual(timestamps[1], rows[1]["the_ts"].TimestampValue);
        }

        [TestMethod]
        public void StreamReaderReturnsSnapshotRows()
        {
            var ts = TableTestHelper.CreateTableWithoutSymbol(_cluster);
            var timestamps = TableTestHelper.MakeTimestamps(2);
            var blobs = TableTestHelper.MakeBlobArray(2);
            var doubles = new[] { 1.0, 2.0 };
            var int64s = new long[] { 1L, 2L };
            var strings = new[] { "one", "two" };
            var interval = new QdbTimeInterval(
                new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            TableTestHelper.InsertRowsWithoutSymbol(_cluster, ts, blobs, doubles, int64s, strings, timestamps);

            using (var reader = ts.StreamReader(interval))
            {
                var first = reader.NextRow();
                var second = reader.NextRow();
                var end = reader.NextRow();

                Assert.IsNotNull(first);
                Assert.IsNotNull(second);
                Assert.IsNull(end);
                Assert.AreNotSame(first, second);

                CollectionAssert.AreEqual(blobs[0], first["the_blob"].BlobValue);
                CollectionAssert.AreEqual(blobs[1], second["the_blob"].BlobValue);
                Assert.AreEqual(timestamps[0], first.Timestamp);
                Assert.AreEqual(timestamps[1], second.Timestamp);
                Assert.AreEqual(doubles[0], first["the_double"].DoubleValue);
                Assert.AreEqual(doubles[1], second["the_double"].DoubleValue);
                Assert.AreEqual(int64s[0], first["the_int64"].Int64Value);
                Assert.AreEqual(int64s[1], second["the_int64"].Int64Value);
                Assert.AreEqual(strings[0], first["the_string"].StringValue);
                Assert.AreEqual(strings[1], second["the_string"].StringValue);
                Assert.AreEqual(timestamps[0], first["the_ts"].TimestampValue);
                Assert.AreEqual(timestamps[1], second["the_ts"].TimestampValue);
            }
        }
    }
}
