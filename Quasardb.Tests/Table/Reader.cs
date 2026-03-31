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

        static DateTime[] MakeTimestamps(int count)
        {
            var r = new DateTime[count];
            var date = DateTime.Parse("2021-01-01T00:00:00Z");
            for (var i = 0; i < count; ++i)
                r[i] = date.AddSeconds(i);
            return r;
        }

        static byte[][] MakeBlobArray(int count)
        {
            var r = new byte[count][];
            for (var i = 0; i < count; ++i)
                r[i] = System.Text.Encoding.UTF8.GetBytes("Running ðŸƒ is faster than swimming ðŸŠ.");
            return r;
        }

        QdbTable CreateTableWithoutSymbol()
        {
            var ts = _cluster.Table(RandomGenerator.CreateUniqueAlias());
            ts.Create(new QdbColumnDefinition[] {
                new QdbBlobColumnDefinition("the_blob"),
                new QdbDoubleColumnDefinition("the_double"),
                new QdbInt64ColumnDefinition("the_int64"),
                new QdbStringColumnDefinition("the_string"),
                new QdbTimestampColumnDefinition("the_ts"),
            });
            return ts;
        }

        [TestMethod]
        public void ReaderToArrayReturnsSnapshotRows()
        {
            var ts = CreateTableWithoutSymbol();
            var timestamps = MakeTimestamps(2);
            var blobs = MakeBlobArray(2);
            var batch = _cluster.ExpWriter([ts.Alias], new QdbTableExpWriterOptions().Transactional());

            batch.Add(ts.Alias, timestamps[0], [blobs[0], 1.0, 1L, "one", timestamps[0]]);
            batch.Add(ts.Alias, timestamps[1], [blobs[1], 2.0, 2L, "two", timestamps[1]]);
            batch.Push();

            var rows = ts.Reader().ToArray();

            Assert.AreEqual(2, rows.Length);
            Assert.AreNotSame(rows[0], rows[1]);
            Assert.AreEqual(timestamps[0], rows[0].Timestamp);
            Assert.AreEqual(timestamps[1], rows[1].Timestamp);
            Assert.AreEqual(1.0, rows[0]["the_double"].DoubleValue);
            Assert.AreEqual(2.0, rows[1]["the_double"].DoubleValue);
            Assert.AreEqual("one", rows[0]["the_string"].StringValue);
            Assert.AreEqual("two", rows[1]["the_string"].StringValue);
        }

        [TestMethod]
        public void StreamReaderReturnsSnapshotRows()
        {
            var ts = CreateTableWithoutSymbol();
            var timestamps = MakeTimestamps(2);
            var blobs = MakeBlobArray(2);
            var interval = new QdbTimeInterval(
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2100, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            var batch = _cluster.ExpWriter([ts.Alias], new QdbTableExpWriterOptions().Transactional());

            batch.Add(ts.Alias, timestamps[0], [blobs[0], 1.0, 1L, "one", timestamps[0]]);
            batch.Add(ts.Alias, timestamps[1], [blobs[1], 2.0, 2L, "two", timestamps[1]]);
            batch.Push();

            using (var reader = ts.StreamReader(interval))
            {
                var first = reader.NextRow();
                var second = reader.NextRow();

                Assert.IsNotNull(first);
                Assert.IsNotNull(second);
                Assert.AreNotSame(first, second);
                Assert.AreEqual(timestamps[0], first.Timestamp);
                Assert.AreEqual(timestamps[1], second.Timestamp);
                Assert.AreEqual(1.0, first["the_double"].DoubleValue);
                Assert.AreEqual(2.0, second["the_double"].DoubleValue);
                Assert.AreEqual("one", first["the_string"].StringValue);
                Assert.AreEqual("two", second["the_string"].StringValue);
            }
        }
    }
}
