using System;
using Quasardb.TimeSeries;
using Quasardb.TimeSeries.ExpWriter;

namespace Quasardb.Tests.Table
{
    internal static class TableTestHelper
    {
        internal static DateTime[] MakeTimestamps(int count)
        {
            var r = new DateTime[count];
            var date = DateTime.Parse("2021-01-01T00:00:00Z");
            for (var i = 0; i < count; ++i)
                r[i] = date.AddSeconds(i);
            return r;
        }

        internal static byte[][] MakeBlobArray(int count)
        {
            var r = new byte[count][];
            for (var i = 0; i < count; ++i)
                r[i] = System.Text.Encoding.UTF8.GetBytes("Running is faster than swimming.");
            return r;
        }

        internal static double[] MakeDoubleArray(int count)
        {
            var r = new double[count];
            for (var i = 0; i < count; ++i)
                r[i] = i;
            return r;
        }

        internal static long[] MakeInt64Array(int count)
        {
            var r = new long[count];
            for (var i = 0; i < count; ++i)
                r[i] = i;
            return r;
        }

        internal static string[] MakeStringArray(int count)
        {
            var r = new string[count];
            for (var i = 0; i < count; ++i)
                r[i] = i.ToString();
            return r;
        }

        internal static QdbTable CreateTableWithoutSymbol(QdbCluster cluster)
        {
            var ts = cluster.Table(RandomGenerator.CreateUniqueAlias());
            ts.Create(new QdbColumnDefinition[] {
                new QdbBlobColumnDefinition("the_blob"),
                new QdbDoubleColumnDefinition("the_double"),
                new QdbInt64ColumnDefinition("the_int64"),
                new QdbStringColumnDefinition("the_string"),
                new QdbTimestampColumnDefinition("the_ts"),
            });
            return ts;
        }

        internal static void InsertRowsWithoutSymbol(QdbCluster cluster, QdbTable ts,
            byte[][] blobs, double[] doubles, long[] int64s, string[] strings, DateTime[] timestamps)
        {
            var batch = cluster.ExpWriter([ts.Alias], new QdbTableExpWriterOptions().Transactional());

            for (var i = 0; i < timestamps.Length; ++i)
                batch.Add(ts.Alias, timestamps[i], [blobs[i], doubles[i], int64s[i], strings[i], timestamps[i]]);

            batch.Push();
        }
    }
}
