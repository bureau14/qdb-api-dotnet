using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

// import-start
using Quasardb;
using Quasardb.TimeSeries;
using Quasardb.TimeSeries.ExpWriter;
// import-end

namespace Quasardb.Tests.Tutorial
{
    [TestClass]
    public class Tutorial
    {
        [TestMethod]
        public void OnInsecure()
        {
            // connect-start
            var c = new QdbCluster("qdb://127.0.0.1:2836");
            // connect-end
            Assert.IsNotNull(c);

            // create-table-start
            // First we acquire a reference to a table which not exist
            var alias = RandomGenerator.CreateUniqueAlias(); //= "stocks";
            var ts = c.Table(alias);

            // Initialize our column definitions
            var columns = new QdbColumnDefinition[]{
                    new QdbDoubleColumnDefinition("open"),
                    new QdbDoubleColumnDefinition("close"),
                    new QdbInt64ColumnDefinition("volume")};

            // Now create the table with the default shard size
            ts.Create(columns);
            // create-table-end

            // tags-start
            ts.AttachTag(c.Tag("nasdaq"));
            // tags-end

            // batch-insert-start
            // We initialize a batch writer.
            var writer = c.ExpWriter(new string[] { ts.Alias }, new QdbTableExpWriterOptions().Transactional());

            // Insert the first row: to start a new row, we must provide it with a mandatory
            // timestamp that all values for this row will share. QuasarDB will use this timestamp
            // as its primary index.
            writer.Add(ts.Alias, new DateTime(2019, 02, 01), new object[] { 3.40, 3.50, 10000L });

            // We tell the batch writer to start a new row before we can set the values for the
            // next row.
            writer.Add(ts.Alias, new DateTime(2019, 02, 02), new object[] { 3.50, 3.55, 7500L });

            // Now that we're done, we push the buffer as one single operation.
            writer.Push();
            // batch-insert-end

            // bulk-read-start
            // We can initialize a bulk reader based directly from our table.
            var reader = ts.Reader();
            ts.Reader(new QdbTimeInterval(new DateTime(2019, 02, 01), new DateTime(2019, 02, 02)));

            // The bulk reader is exposed as a regular .Net Enumerable
            foreach (var row in reader)
            {
                // Each row has a timestamp which you can access as a Timespec:
                Console.WriteLine($"row timestamp: {row.Timestamp}");

                // Note that the offsets of the values array align with the offsets we used
                // when creating the table, i.e. 0 means "open", 1 means "close" and 2 means
                // "volume":
                var openValue = row[0].DoubleValue;
                var closeValue = row[1].DoubleValue;
                var volumeValue = row[2].Int64Value;
            }
            // bulk-read-end

            // column-insert-start
            // Prepare some data to be inserted
            var opens = new QdbDoublePointCollection { { new DateTime(2019, 02, 01), 3.40 }, { new DateTime(2019, 02, 02), 3.50 } };
            var closes = new QdbDoublePointCollection { { new DateTime(2019, 02, 01), 3.50 }, { new DateTime(2019, 02, 02), 3.55 } };
            var volumes = new QdbInt64PointCollection { { new DateTime(2019, 02, 01), 10000 }, { new DateTime(2019, 02, 02), 7500 } };

            var columnWriter = c.ExpWriter(new string[] { ts.Alias }, new QdbTableExpWriterOptions().Transactional());
            for (var i = 0; i < opens.Count; ++i)
            {
                columnWriter.Add(ts.Alias, opens[i].Time, new object[] { opens[i].Value, closes[i].Value, volumes[i].Value });
            }
            columnWriter.Push();
            // column-insert-end

            // column-get-start
            var range = new QdbTimeInterval(new DateTime(2019, 02, 01), new DateTime(2019, 02, 02));
            var resultPoints = ts.Reader(new QdbColumnDefinition[]
            {
                new QdbDoubleColumnDefinition("open")
            }, range);
            // column-get-end
            var ptEnum = resultPoints.GetEnumerator();
            ptEnum.MoveNext();
            Assert.AreEqual(ptEnum.Current.Timestamp, opens[0].Time);
            Assert.AreEqual(ptEnum.Current[0].DoubleValue, opens[0].Value);

            // query-start
            // Execute the query
            var r = c.Query("SELECT SUM(volume) FROM " + alias);

            // The rows are exposed as a regular .Net Enumerable
            var columnNames = r.ColumnNames;
            var rows = r.Rows;
            foreach (var row in rows)
            {
                Console.WriteLine($"{columnNames[0]}: {row[0].Value}");
            }

            // Since we only expect one row, we also access it like this:
            var aggregateResult = rows[0]["SUM(volume)"].Int64Value;
            Console.Write($"SUM(volume): {aggregateResult}");
            // query-end

            // drop-table-start
            // Use the earlier reference of the table we acquired to remove it:
            ts.Remove();
            // drop-table-end
        }
        [TestMethod]
        public void OnSecure()
        {
            var secureClusterURI = DaemonRunner.SecureClusterUrl;
            var clusterPublicKey = DaemonRunner.ClusterPublicKey;
            var userName = DaemonRunner.UserName;
            var userPrivateKey = DaemonRunner.UserPrivateKey;
            // secure-connect-start
            var sc = new QdbCluster(secureClusterURI, clusterPublicKey, userName, userPrivateKey);
            // secure-connect-end
            Assert.IsNotNull(sc);
        }
    }
}
