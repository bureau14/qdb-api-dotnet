using System;
using Quasardb.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quasardb.Tests.Query
{
    [TestClass]
    public class Continuous
    {
        private readonly QdbCluster _cluster = QdbTestCluster.Instance;

        [TestMethod, Timeout(60000)]
        public void ReceiveFullUpdate()
        {
            var waitHandle = new System.Threading.AutoResetEvent(false);
            var alias = RandomGenerator.CreateUniqueAlias();
            _cluster.Query(String.Format("CREATE TABLE {0}(col int64)", alias));

            var latest = 0;

            _cluster.ContinuousQuery(String.Format("SELECT $timestamp, col FROM {0}", alias), QdbContinuousQuery.Mode.Full, TimeSpan.FromMilliseconds(100), (result) =>
            {
                if (result == null) return 0;

                var columns = result.ColumnNames;
                if (columns == null) return 0;

                if (columns.Count != 2) return 0;
                if (columns[0] != "$timestamp") return 0;
                if (columns[1] != "col") return 0;

                var rows = result.Rows;
                if (rows.Count == 0) return 0;
                if (rows.Count > 0 && latest > 1) return 0;

                {
                    var row = rows[0];
                    if (row == null) return 0;
                    if (row.Count == 1) return 0;
                    if ((DateTime)row["$timestamp"].Value != new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc)) return 0;
                    if ((long)row["col"].Value != 1) return 0;
                }

                if (latest == 1)
                {
                    var row = rows[1];
                    if (row == null) return 0;
                    if (row.Count == 1) return 0;
                    if ((DateTime)row["$timestamp"].Value != new DateTime(2021, 1, 2, 0, 0, 0, DateTimeKind.Utc)) return 0;
                    if ((long)row["col"].Value != 2) return 0;
                }
                latest += 1;
                waitHandle.Set();
                return 0;
            });

            _cluster.Query(String.Format("INSERT INTO {0}($timestamp, col) VALUES(2021-01-01, 1)", alias));

            waitHandle.WaitOne();
            waitHandle.Reset();

            _cluster.Query(String.Format("INSERT INTO {0}($timestamp, col) VALUES(2021-01-02, 2)", alias));

            waitHandle.WaitOne();
            waitHandle.Reset();
        }

        [TestMethod, Timeout(60000)]
        public void ReceiveNewValuesOnlyUpdate()
        {
            var waitHandle = new System.Threading.AutoResetEvent(false);
            var alias = RandomGenerator.CreateUniqueAlias();
            _cluster.Query(String.Format("CREATE TABLE {0}(col int64)", alias));

            var latest = 0;

            _cluster.ContinuousQuery(String.Format("SELECT $timestamp, col FROM {0}", alias), QdbContinuousQuery.Mode.NewValuesOnly, TimeSpan.FromMilliseconds(100), (result) =>
            {
                if (result == null) return 0;

                var columns = result.ColumnNames;
                if (columns == null) return 0;

                if (columns.Count != 2) return 0;
                if (columns[0] != "$timestamp") return 0;
                if (columns[1] != "col") return 0;

                var rows = result.Rows;
                if (rows.Count == 0) return 0;
                if (rows.Count > 0 && latest > 1) return 0;

                var row = rows[0];
                if (row == null) return 0;
                if (row.Count == 1) return 0;
                if (latest == 0)
                {
                    if ((DateTime)row["$timestamp"].Value != new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc)) return 0;
                    if ((long)row["col"].Value != 1) return 0;
                }
                else if (latest == 1)
                {
                    if ((DateTime)row["$timestamp"].Value != new DateTime(2021, 1, 2, 0, 0, 0, DateTimeKind.Utc)) return 0;
                    if ((long)row["col"].Value != 2) return 0;
                }
                latest += 1;
                waitHandle.Set();
                return 0;
            });

            _cluster.Query(String.Format("INSERT INTO {0}($timestamp, col) VALUES(2021-01-01, 1)", alias));

            waitHandle.WaitOne();
            waitHandle.Reset();

            _cluster.Query(String.Format("INSERT INTO {0}($timestamp, col) VALUES(2021-01-02, 2)", alias));

            waitHandle.WaitOne();
            waitHandle.Reset();
        }
    }
}
