using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

// import-start
using Quasardb;
using Quasardb.TimeSeries;
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
            // First we acquire a reference to a table (which may or may not yet exist)
            var ts = c.Table("stocks");

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
            // We initialize a writer our batch writer.
            var writer = ts.Writer();
            // Alternatively we could select specific columns
            // var writer = ts.Writer(new QdbColumnDefinition[]{
            //    new QdbDoubleColumnDefinition("open")
            //});

            // Insert the first row: to start a new row, we must provide it with a mandatory
            // timestamp that all values for this row will share. QuasarDB will use this timestamp
            // as its primary index.
            writer.StartRow(new DateTime(2019, 02, 01));

            // We now set the values for our columns by their relative offsets: column 0 below
            // refers to the first column we provide in the columns variable above.
            writer.SetDouble(0, 3.40);
            writer.SetDouble(1, 3.50);
            writer.SetInt64(2, 10000);

            // We tell the batch writer to start a new row before we can set the values for the
            // next row.
            writer.StartRow(new DateTime(2019, 02, 02));

            writer.SetDouble(0, 3.50);
            writer.SetDouble(1, 3.55);
            writer.SetInt64(2, 7500);

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
                var closealue = row[1].DoubleValue;
                var volumeValue = row[2].Int64Value;
            }
            // bulk-read-end

            // column-insert-start
            // Prepare some data to be inserted
            var opens = new QdbDoublePointCollection { { new DateTime(2019, 02, 01), 3.40 }, { new DateTime(2019, 02, 02), 3.50 } };
            var closes = new QdbDoublePointCollection { { new DateTime(2019, 02, 01), 3.50 }, { new DateTime(2019, 02, 02), 3.55 } };
            var volumes = new QdbInt64PointCollection { { new DateTime(2019, 02, 01), 10000 }, { new DateTime(2019, 02, 02), 7500 } };

            // Retrieve the different columns from our table
            var openCol = ts.DoubleColumns["open"];
            var closeCol = ts.DoubleColumns["close"];
            var volumeCol = ts.Int64Columns["volume"];

            // Insert data for each column
            openCol.Insert(opens);
            closeCol.Insert(closes);
            volumeCol.Insert(volumes);
            // column-insert-end

            // column-get-start
            // using the same columns we used for the insertion
            // we can retrieve the points from a specific range
            var range = new QdbTimeInterval(new DateTime(2019, 02, 01), new DateTime(2019, 02, 02));

            openCol.Points(range);

            // you can now inspect the values in the enumerable Points
            var resultPoints = openCol.Points();
            // column-get-end
            var ptEnum = resultPoints.GetEnumerator();
            ptEnum.MoveNext();
            Assert.AreEqual(ptEnum.Current.Time, opens[0].Time);
            Assert.AreEqual(ptEnum.Current.Value, opens[0].Value);

            // query-start
            // Execute the query
            var r = c.Query("SELECT SUM(volume) FROM stocks");

            // The rows are exposed as a regular .Net Enumerable
            var columnNames = r.ColumnNames;
            var rows = r.Rows;
            foreach (var row in rows)
            {
                Console.WriteLine($"{columnNames[0]}: {row[0].Value}");
            }

            // Since we only expect one row, we also access it like this:
            var aggregateResult = rows[0]["sum(volume)"].Int64Value;
            Console.Write($"sum(volume): {aggregateResult}");
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