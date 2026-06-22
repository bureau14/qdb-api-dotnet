using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.TimeSeries;
using Quasardb.TimeSeries.ExpWriter;
using Quasardb.TimeSeries.Reader;

namespace Quasardb.Tests.Table
{
    [TestClass]
    public class BulkReaderTests
    {
        readonly QdbCluster _cluster = QdbTestCluster.Instance;

        [TestMethod]
        public void Ok_BulkReaderSingleTable()
        {
            var ts = TableTestHelper.CreateTableWithoutSymbol(_cluster);
            var count = 10000;
            var blobs = TableTestHelper.MakeBlobArray(count);
            var doubles = TableTestHelper.MakeDoubleArray(count);
            var ints = TableTestHelper.MakeInt64Array(count);
            var strings = TableTestHelper.MakeStringArray(count);
            var timestamps = TableTestHelper.MakeTimestamps(count);

            TableTestHelper.InsertRowsWithoutSymbol(_cluster, ts, blobs, doubles, ints, strings, timestamps);

            var reader = _cluster.BulkReader(["the_blob", "the_double", "the_int64", "the_string", "the_ts"],
                new QdbBulkReaderTable[] { new(ts.Alias, null) });
            reader.rowsToGet = count / 4;

            var idx = 0;
            foreach (var row in reader)
            {
                Assert.AreEqual(5, row.Count);
                Assert.ThrowsException<Quasardb.Exceptions.QdbColumnNotFoundException>(() =>
                {
                    var _ = row["$table"];
                });
                Assert.AreEqual(timestamps[idx], row.Timestamp);
                CollectionAssert.AreEqual(blobs[idx], row[0].BlobValue);
                Assert.AreEqual(doubles[idx], row[1].DoubleValue);
                Assert.AreEqual(ints[idx], row[2].Int64Value);
                Assert.AreEqual(strings[idx], row[3].StringValue);
                Assert.AreEqual(timestamps[idx], row[4].TimestampValue);
                idx++;
            }

            Assert.AreEqual(count, idx);
        }

        [TestMethod]
        public void Ok_BulkReaderSingleTableAllData()
        {
            var ts = TableTestHelper.CreateTableWithoutSymbol(_cluster);
            var count = 10000;
            var blobs = TableTestHelper.MakeBlobArray(count);
            var doubles = TableTestHelper.MakeDoubleArray(count);
            var ints = TableTestHelper.MakeInt64Array(count);
            var strings = TableTestHelper.MakeStringArray(count);
            var timestamps = TableTestHelper.MakeTimestamps(count);

            TableTestHelper.InsertRowsWithoutSymbol(_cluster, ts, blobs, doubles, ints, strings, timestamps);

            var reader = _cluster.BulkReader(["the_blob", "the_double", "the_int64", "the_string", "the_ts"],
                new QdbBulkReaderTable[] { new(ts.Alias, null) });

            var idx = 0;
            foreach (var row in reader)
            {
                Assert.AreEqual(5, row.Count);
                Assert.AreEqual(timestamps[idx], row.Timestamp);
                CollectionAssert.AreEqual(blobs[idx], row[0].BlobValue);
                Assert.AreEqual(doubles[idx], row[1].DoubleValue);
                Assert.AreEqual(ints[idx], row[2].Int64Value);
                Assert.AreEqual(strings[idx], row[3].StringValue);
                Assert.AreEqual(timestamps[idx], row[4].TimestampValue);
                idx++;
            }

            Assert.AreEqual(count, idx);
        }

        [TestMethod]
        public void Ok_BulkReaderMultiTable()
        {
            var ts1 = TableTestHelper.CreateTableWithoutSymbol(_cluster);
            var ts2 = TableTestHelper.CreateTableWithoutSymbol(_cluster);

            var count = 10000;
            var blobs = TableTestHelper.MakeBlobArray(count);
            var doubles = TableTestHelper.MakeDoubleArray(count);
            var ints = TableTestHelper.MakeInt64Array(count);
            var strings = TableTestHelper.MakeStringArray(count);
            var timestamps = TableTestHelper.MakeTimestamps(count * 2);

            var batch = _cluster.ExpWriter([ts1.Alias, ts2.Alias], new QdbTableExpWriterOptions().Transactional());
            for (var i = 0; i < count; i++)
            {
                batch.Add(ts1.Alias, timestamps[i], [blobs[i], doubles[i], ints[i], strings[i], timestamps[i]]);
                batch.Add(ts2.Alias, timestamps[i + count], [blobs[i], doubles[i], ints[i], strings[i], timestamps[i]]);
            }
            batch.Push();

            var reader = _cluster.BulkReader(["$table", "the_blob", "the_double", "the_int64", "the_string", "the_ts"],
                new QdbBulkReaderTable[] { new(ts1.Alias, null), new(ts2.Alias, null) });
            reader.rowsToGet = count / 4;

            var aliases = new List<string>();
            var idx = 0;
            foreach (var row in reader)
            {
                Assert.AreEqual(6, row.Count);
                aliases.Add(row[0].StringValue);
                Assert.AreEqual(timestamps[idx], row.Timestamp);
                CollectionAssert.AreEqual(blobs[idx % count], row[1].BlobValue);
                Assert.AreEqual(doubles[idx % count], row[2].DoubleValue);
                Assert.AreEqual(ints[idx % count], row[3].Int64Value);
                Assert.AreEqual(strings[idx % count], row[4].StringValue);
                Assert.AreEqual(timestamps[idx % count], row[5].TimestampValue);
                idx++;
            }

            Assert.AreEqual(count * 2, idx);

            var expected = Enumerable.Repeat(ts1.Alias, count).Concat(Enumerable.Repeat(ts2.Alias, count)).ToArray();
            CollectionAssert.AreEqual(expected, aliases);
        }
    }
}
