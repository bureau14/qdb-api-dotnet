using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.TimeSeries;
using Quasardb.TimeSeries.ExpWriter;
using Quasardb.TimeSeries.Reader;
using System.Linq;
using System.Collections.Generic;

namespace Quasardb.Tests.Table
{
    [TestClass]
    public class BulkReaderTests
    {
        readonly QdbCluster _cluster = QdbTestCluster.Instance;

        DateTime[] MakeTimestamps(int count)
        {
            var r = new DateTime[count];
            var date = DateTime.Parse("2021-01-01T00:00:00Z");
            for (int i = 0; i < count; ++i)
                r[i] = date.AddSeconds(i);
            return r;
        }

        byte[][] MakeBlobArray(int count)
        {
            var r = new byte[count][];
            for (int i = 0; i < count; ++i)
                r[i] = System.Text.Encoding.UTF8.GetBytes("Running 🏃 is faster than swimming 🏊.");
            return r;
        }

        double[] MakeDoubleArray(int count)
        {
            var r = new double[count];
            for (int i = 0; i < count; ++i) r[i] = i;
            return r;
        }

        long[] MakeInt64Array(int count)
        {
            var r = new long[count];
            for (int i = 0; i < count; ++i) r[i] = i;
            return r;
        }

        string[] MakeStringArray(int count)
        {
            var r = new string[count];
            for (int i = 0; i < count; ++i) r[i] = i.ToString();
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
        public void Ok_BulkReaderSingleTable()
        {
            var ts = CreateTableWithoutSymbol();
            var count = 3;
            var blobs = MakeBlobArray(count);
            var doubles = MakeDoubleArray(count);
            var ints = MakeInt64Array(count);
            var strings = MakeStringArray(count);
            var timestamps = MakeTimestamps(count);

            var batch = _cluster.ExpWriter(new string[] { ts.Alias }, new QdbTableExpWriterOptions().Transactional());
            for (int i = 0; i < count; i++)
                batch.Add(ts.Alias, timestamps[i], new object[] { blobs[i], doubles[i], ints[i], strings[i], timestamps[i] });
            batch.Push();

            var reader = _cluster.BulkReader(new string[] { "the_blob", "the_double", "the_int64", "the_string", "the_ts" },
                new QdbBulkReaderTable[] { new QdbBulkReaderTable(ts.Alias, null) });

            int idx = 0;
            foreach (var row in reader)
            {
                Assert.AreEqual(ts.Alias, row[0].StringValue);
                Assert.AreEqual(timestamps[idx], row.Timestamp);
                CollectionAssert.AreEqual(blobs[idx], row[1].BlobValue);
                Assert.AreEqual(doubles[idx], row[2].DoubleValue);
                Assert.AreEqual(ints[idx], row[3].Int64Value);
                Assert.AreEqual(strings[idx], row[4].StringValue);
                Assert.AreEqual(timestamps[idx], row[5].TimestampValue);
                idx++;
            }
            Assert.AreEqual(count, idx);
        }

        [TestMethod]
        public void Ok_BulkReaderMultiTable()
        {
            var ts1 = CreateTableWithoutSymbol();
            var ts2 = CreateTableWithoutSymbol();
            var count = 3;
            var blobs = MakeBlobArray(count);
            var doubles = MakeDoubleArray(count);
            var ints = MakeInt64Array(count);
            var strings = MakeStringArray(count);
            var timestamps = MakeTimestamps(count);

            var batch = _cluster.ExpWriter(new string[] { ts1.Alias, ts2.Alias }, new QdbTableExpWriterOptions().Transactional());
            for (int i = 0; i < count; i++)
            {
                batch.Add(ts1.Alias, timestamps[i], new object[] { blobs[i], doubles[i], ints[i], strings[i], timestamps[i] });
                batch.Add(ts2.Alias, timestamps[i], new object[] { blobs[i], doubles[i], ints[i], strings[i], timestamps[i] });
            }
            batch.Push();

            var reader = _cluster.BulkReader(new string[] { "the_blob", "the_double", "the_int64", "the_string", "the_ts" },
                new QdbBulkReaderTable[] { new QdbBulkReaderTable(ts1.Alias, null), new QdbBulkReaderTable(ts2.Alias, null) });

            var aliases = new List<string>();
            foreach (var row in reader)
            {
                aliases.Add(row[0].StringValue);
            }

            var expected = Enumerable.Repeat(ts1.Alias, count).Concat(Enumerable.Repeat(ts2.Alias, count)).ToArray();
            CollectionAssert.AreEqual(expected, aliases);
        }
    }
}
