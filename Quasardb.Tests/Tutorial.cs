using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

// import-start
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
            ts.Remove();
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