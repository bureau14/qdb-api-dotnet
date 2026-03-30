using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;
using Quasardb.TimeSeries.ExpWriter;

using System.Runtime.InteropServices;

namespace Quasardb.Tests.Table
{
    [TestClass]
    unsafe public class ExpWriterTests
    {
        private readonly QdbCluster _cluster = QdbTestCluster.Instance;

        private static double? ToExpectedDouble(object value)
        {
            if (value == null)
                return null;

            var result = Convert.ToDouble(value);
            return double.IsNaN(result) ? (double?)null : result;
        }

        private static void AssertNullableDouble(double? actual, double? expected)
        {
            var normalizedActual = actual.HasValue && double.IsNaN(actual.Value) ? (double?)null : actual;
            var normalizedExpected = expected.HasValue && double.IsNaN(expected.Value) ? (double?)null : expected;

            if (!normalizedActual.HasValue && !normalizedExpected.HasValue)
                return;

            Assert.AreEqual(normalizedActual, normalizedExpected);
        }

        private static void AssertNullableDouble(double? actual, object expected)
        {
            var normalizedActual = actual.HasValue && double.IsNaN(actual.Value) ? (double?)null : actual;
            var normalizedExpected = ToExpectedDouble(expected);

            if (!normalizedActual.HasValue && !normalizedExpected.HasValue)
                return;

            Assert.AreEqual(normalizedActual, normalizedExpected);
        }

        private static long? ToExpectedInt64(object value)
        {
            if (value == null)
                return null;

            var result = Convert.ToInt64(value);
            return result == long.MinValue ? (long?)null : result;
        }

        private static void AssertNullableInt64(long? actual, long? expected)
        {
            var normalizedActual = actual == long.MinValue ? (long?)null : actual;
            var normalizedExpected = expected == long.MinValue ? (long?)null : expected;

            if (!normalizedActual.HasValue && !normalizedExpected.HasValue)
                return;

            Assert.AreEqual(normalizedActual, normalizedExpected);
        }

        private static void AssertNullableInt64(long? actual, object expected)
        {
            var normalizedActual = actual == long.MinValue ? (long?)null : actual;
            var normalizedExpected = ToExpectedInt64(expected);

            if (!normalizedActual.HasValue && !normalizedExpected.HasValue)
                return;

            Assert.AreEqual(normalizedActual, normalizedExpected);
        }

        private static void AssertNullableTimestamp(DateTime? actual, DateTime? expected)
        {
            var normalizedActual = actual == DateTime.MinValue ? (DateTime?)null : actual;
            var normalizedExpected = expected == DateTime.MinValue ? (DateTime?)null : expected;

            if (!normalizedActual.HasValue && !normalizedExpected.HasValue)
                return;

            Assert.AreEqual(normalizedActual, normalizedExpected);
        }

        public static DateTime[] MakeTimestamps(int count)
        {
            var random = new Random();
            var r = new DateTime[count];

            DateTime date = DateTime.Parse("2021-01-01T00:00:00Z");
            for (var i = 0; i < count; ++i)
            {
                r[i] = date.AddSeconds(i);
            }
            return r;
        }

        public static byte[][] MakeBlobArray(int count)
        {
            var r = new byte[count][];

            for (var i = 0; i < count; ++i)
            {
                r[i] = System.Text.Encoding.UTF8.GetBytes("Running 🏃 is faster than swimming 🏊.");
            }
            return r;
        }

        public static double[] MakeDoubleArray(int count)
        {
            var r = new double[count];

            for (var i = 0; i < count; ++i)
            {
                r[i] = (double)i;
            }
            return r;
        }

        public static long[] MakeInt64Array(int count)
        {
            var r = new long[count];

            for (var i = 0; i < count; ++i)
            {
                r[i] = (long)i;
            }
            return r;
        }

        public static string RandomString(int length, Random r)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[r.Next(s.Length)]).ToArray());
        }

        public static string[] MakeStringArray(int count)
        {
            var random = new Random();
            var r = new string[count];

            for (var i = 0; i < count; ++i)
            {
                r[i] = RandomString(32, random);
            }
            return r;
        }

        public static DateTime[] CreateTimestampPoints(int count)
        {
            return MakeTimestamps(count);
        }

        public QdbTable CreateTable(string alias = null)
        {
            var ts = _cluster.Table(alias ?? RandomGenerator.CreateUniqueAlias());
            ts.Create(new QdbColumnDefinition[] {
                new QdbBlobColumnDefinition("the_blob"),
                new QdbDoubleColumnDefinition("the_double"),
                new QdbInt64ColumnDefinition("the_int64"),
                new QdbStringColumnDefinition("the_string"),
                new QdbTimestampColumnDefinition("the_ts"),
                new QdbSymbolColumnDefinition("the_symbol", "symtable"),
            });
            return ts;
        }

        public QdbTable CreateTableWithoutSymbol(string alias = null)
        {
            var ts = _cluster.Table(alias ?? RandomGenerator.CreateUniqueAlias());
            ts.Create(new QdbColumnDefinition[] {
                new QdbBlobColumnDefinition("the_blob"),
                new QdbDoubleColumnDefinition("the_double"),
                new QdbInt64ColumnDefinition("the_int64"),
                new QdbStringColumnDefinition("the_string"),
                new QdbTimestampColumnDefinition("the_ts"),
            });
            return ts;
        }

        public QdbTableExpWriter Insert(string ts,
            QdbTableExpWriterOptions options,
            byte[][] blobs,
            double[] doubles,
            long[] int64s,
            string[] strings,
            DateTime[] timestamps)
        {
            if (ts == null) throw new ArgumentNullException(nameof(ts));
            if (blobs == null) throw new ArgumentNullException(nameof(blobs));
            if (doubles == null) throw new ArgumentNullException(nameof(doubles));
            if (int64s == null) throw new ArgumentNullException(nameof(int64s));
            if (strings == null) throw new ArgumentNullException(nameof(strings));
            if (timestamps == null) throw new ArgumentNullException(nameof(timestamps));

            var batch = _cluster.ExpWriter([ts], options);

            for (var index = 0; index < doubles.Length; index++)
            {
                batch.Add(ts, timestamps[index], [blobs[index], doubles[index], int64s[index], strings[index], timestamps[index], strings[index]]);
            }
            return batch;
        }

        public QdbTableExpWriter InsertWithColumn(string ts,
            QdbTableExpWriterOptions options,
            byte[][] blobs,
            double[] doubles,
            long[] int64s,
            string[] strings,
            DateTime[] timestamps)
        {
            if (ts == null) throw new ArgumentNullException(nameof(ts));
            if (blobs == null) throw new ArgumentNullException(nameof(blobs));
            if (doubles == null) throw new ArgumentNullException(nameof(doubles));
            if (int64s == null) throw new ArgumentNullException(nameof(int64s));
            if (strings == null) throw new ArgumentNullException(nameof(strings));
            if (timestamps == null) throw new ArgumentNullException(nameof(timestamps));

            var batch = _cluster.ExpWriter([ts], options);

            batch.SetTimestamps(ts, timestamps.ToList());
            batch.SetColumn(ts, "the_blob", blobs.ToList());
            batch.SetColumn(ts, "the_double", doubles.ToList());
            batch.SetColumn(ts, "the_int64", int64s.ToList());
            batch.SetColumn(ts, "the_string", strings.ToList());
            batch.SetColumn(ts, "the_ts", timestamps.ToList());
            batch.SetColumn(ts, "the_symbol", strings.ToList());
            return batch;
        }

        public QdbTableExpWriter InsertWithColumnWithNull(string ts,
            QdbTableExpWriterOptions options,
            byte[][] blobs,
            double?[] doubles,
            long?[] int64s,
            string[] strings,
            DateTime[] timestamps,
            DateTime?[] timestamps_values)
        {
            if (ts == null) throw new ArgumentNullException(nameof(ts));
            if (blobs == null) throw new ArgumentNullException(nameof(blobs));
            if (doubles == null) throw new ArgumentNullException(nameof(doubles));
            if (int64s == null) throw new ArgumentNullException(nameof(int64s));
            if (strings == null) throw new ArgumentNullException(nameof(strings));
            if (timestamps == null) throw new ArgumentNullException(nameof(timestamps));

            var batch = _cluster.ExpWriter([ts], options);

            batch.SetTimestamps(ts, timestamps.ToList());
            batch.SetColumn(ts, "the_blob", blobs.ToList());
            batch.SetColumn(ts, "the_double", doubles.ToList());
            batch.SetColumn(ts, "the_int64", int64s.ToList());
            batch.SetColumn(ts, "the_string", strings.ToList());
            batch.SetColumn(ts, "the_ts", timestamps_values.ToList());
            batch.SetColumn(ts, "the_symbol", strings.ToList());
            return batch;
        }

        public QdbTableExpWriter InsertByName(string ts,
            QdbTableExpWriterOptions options,
            byte[][] blobs,
            double[] doubles,
            long[] int64s,
            string[] strings,
            DateTime[] timestamps)
        {
            if (ts == null) throw new ArgumentNullException(nameof(ts));
            if (blobs == null) throw new ArgumentNullException(nameof(blobs));
            if (doubles == null) throw new ArgumentNullException(nameof(doubles));
            if (int64s == null) throw new ArgumentNullException(nameof(int64s));
            if (strings == null) throw new ArgumentNullException(nameof(strings));
            if (timestamps == null) throw new ArgumentNullException(nameof(timestamps));

            var batch = _cluster.ExpWriter([ts], options);

            for (var index = 0; index < doubles.Length; index++)
            {
                batch.Add(ts, timestamps[index], [blobs[index], doubles[index], int64s[index], strings[index], timestamps[index], strings[index]]);
            }
            return batch;
        }

        public static void CheckTables(QdbTable ts,
            byte[][] blobs,
            double[] doubles,
            long[] int64s,
            string[] strings,
            DateTime[] timestamps,
            string[] symbols)
        {
            if (ts == null) throw new ArgumentNullException(nameof(ts));
            if (blobs == null) throw new ArgumentNullException(nameof(blobs));
            if (doubles == null) throw new ArgumentNullException(nameof(doubles));
            if (int64s == null) throw new ArgumentNullException(nameof(int64s));
            if (strings == null) throw new ArgumentNullException(nameof(strings));
            if (timestamps == null) throw new ArgumentNullException(nameof(timestamps));
            if (symbols == null) throw new ArgumentNullException(nameof(symbols));

            var rows = ts.Reader().ToArray();
            Assert.AreEqual(rows.Length, blobs.Length);
            Assert.AreEqual(rows.Length, doubles.Length);
            Assert.AreEqual(rows.Length, int64s.Length);
            Assert.AreEqual(rows.Length, strings.Length);
            Assert.AreEqual(rows.Length, timestamps.Length);
            Assert.AreEqual(rows.Length, symbols.Length);
            for (var idx = 0; idx < rows.Length; idx++)
            {
                Assert.AreEqual(rows[idx].Timestamp, timestamps[idx]);
                CollectionAssert.AreEqual(rows[idx]["the_blob"].BlobValue, blobs[idx]);
                Assert.AreEqual(rows[idx]["the_double"].DoubleValue, doubles[idx]);
                Assert.AreEqual(rows[idx]["the_int64"].Int64Value, int64s[idx]);
                Assert.AreEqual(rows[idx]["the_string"].StringValue, strings[idx]);
                Assert.AreEqual(rows[idx]["the_ts"].TimestampValue, timestamps[idx]);
                Assert.AreEqual(rows[idx]["the_symbol"].StringValue, symbols[idx]);
            }
        }

        public static void CheckTablesWithNull(QdbTable ts,
            byte[][] blobs,
            double?[] doubles,
            long?[] int64s,
            string[] strings,
            DateTime[] timestamps,
            DateTime?[] timestamp_values,
            string[] symbols)
        {
            if (ts == null) throw new ArgumentNullException(nameof(ts));
            if (blobs == null) throw new ArgumentNullException(nameof(blobs));
            if (doubles == null) throw new ArgumentNullException(nameof(doubles));
            if (int64s == null) throw new ArgumentNullException(nameof(int64s));
            if (strings == null) throw new ArgumentNullException(nameof(strings));
            if (timestamps == null) throw new ArgumentNullException(nameof(timestamps));
            if (timestamp_values == null) throw new ArgumentNullException(nameof(timestamp_values));
            if (symbols == null) throw new ArgumentNullException(nameof(symbols));

            var rows = ts.Reader().ToArray();
            Assert.AreEqual(rows.Length, blobs.Length);
            Assert.AreEqual(rows.Length, doubles.Length);
            Assert.AreEqual(rows.Length, int64s.Length);
            Assert.AreEqual(rows.Length, strings.Length);
            Assert.AreEqual(rows.Length, timestamps.Length);
            Assert.AreEqual(rows.Length, timestamp_values.Length);
            Assert.AreEqual(rows.Length, symbols.Length);
            for (var idx = 0; idx < rows.Length; idx++)
            {
                Assert.AreEqual(rows[idx].Timestamp, timestamps[idx]);
                CollectionAssert.AreEqual(rows[idx]["the_blob"].BlobValue, blobs[idx]);
                AssertNullableDouble(rows[idx]["the_double"].DoubleValue, doubles[idx]);
                AssertNullableInt64(rows[idx]["the_int64"].Int64Value, int64s[idx]);
                Assert.AreEqual(rows[idx]["the_string"].StringValue, strings[idx]);
                AssertNullableTimestamp(rows[idx]["the_ts"].TimestampValue, timestamp_values[idx]);
                Assert.AreEqual(rows[idx]["the_symbol"].StringValue, symbols[idx]);
            }
        }

        public static void CheckTablesWithoutSymbol(QdbTable ts,
            byte[][] blobs,
            double[] doubles,
            long[] int64s,
            string[] strings,
            DateTime[] timestamps)
        {
            if (ts == null) throw new ArgumentNullException(nameof(ts));
            if (blobs == null) throw new ArgumentNullException(nameof(blobs));
            if (doubles == null) throw new ArgumentNullException(nameof(doubles));
            if (int64s == null) throw new ArgumentNullException(nameof(int64s));
            if (strings == null) throw new ArgumentNullException(nameof(strings));
            if (timestamps == null) throw new ArgumentNullException(nameof(timestamps));

            var rows = ts.Reader().ToArray();
            Assert.AreEqual(rows.Length, blobs.Length);
            Assert.AreEqual(rows.Length, doubles.Length);
            Assert.AreEqual(rows.Length, int64s.Length);
            Assert.AreEqual(rows.Length, strings.Length);
            Assert.AreEqual(rows.Length, timestamps.Length);
            for (var idx = 0; idx < rows.Length; idx++)
            {
                Assert.AreEqual(rows[idx].Timestamp, timestamps[idx]);
                CollectionAssert.AreEqual(rows[idx]["the_blob"].BlobValue, blobs[idx]);
                Assert.AreEqual(rows[idx]["the_double"].DoubleValue, doubles[idx]);
                Assert.AreEqual(rows[idx]["the_int64"].Int64Value, int64s[idx]);
                Assert.AreEqual(rows[idx]["the_string"].StringValue, strings[idx]);
                Assert.AreEqual(rows[idx]["the_ts"].TimestampValue, timestamps[idx]);
            }
        }

        public static bool IsBlittable(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (type.IsArray)
            {
                var elem = type.GetElementType();
                return elem.IsValueType && IsBlittable(elem);
            }
            try
            {
                object instance = type.IsValueType ? Activator.CreateInstance(type) : null;
                if (instance != null)
                {
                    GCHandle.Alloc(instance, GCHandleType.Pinned).Free();
                    return true;
                }
                // For reference types, we cannot guarantee blittability this way, so return false.
                return false;
            }
            catch
            {
                return false;
            }
        }

        [TestMethod]
        public unsafe void Ok_AllBlittable()
        {
            //Assert.IsTrue(IsBlittable(typeof(string)));
            Assert.IsTrue(IsBlittable(typeof(byte[])));
            Assert.IsTrue(IsBlittable(typeof(Quasardb.Native.qdb_blob)));
            Assert.IsTrue(IsBlittable(typeof(Quasardb.Native.qdb_sized_string)));
            Assert.IsTrue(IsBlittable(typeof(Quasardb.Native.qdb_blob[])));
            Assert.IsTrue(IsBlittable(typeof(Quasardb.Native.qdb_sized_string[])));
            Assert.IsTrue(IsBlittable(typeof(Quasardb.Native.qdb_exp_batch_push_table)));
            Assert.IsTrue(IsBlittable(typeof(Quasardb.Native.qdb_exp_batch_push_table_data)));
            Assert.IsTrue(IsBlittable(typeof(Quasardb.Native.qdb_exp_batch_push_column)));
            //Assert.IsTrue(IsBlittable(typeof(Quasardb.Native.qdb_exp_batch_push_column_data)));
        }

        [TestMethod]
        public void Ok_ColumnInsert()
        {
            QdbTable ts = CreateTable();

            var blobs = MakeBlobArray(10);
            var doubles = MakeDoubleArray(10);
            var int64s = MakeInt64Array(10);
            var strings = MakeStringArray(10);
            var timestamps = MakeTimestamps(10);

            var batch = InsertWithColumn(ts.Alias, new QdbTableExpWriterOptions().Transactional(), blobs, doubles, int64s, strings, timestamps);

            batch.Push();

            CheckTables(ts, blobs, doubles, int64s, strings, timestamps, strings);
        }

        [TestMethod]
        public void Ok_ColumnInsertWithNull()
        {
            QdbTable ts = CreateTable();

            var blobs = new byte[5][] { System.Text.Encoding.UTF8.GetBytes("Running 🏃 is faster than swimming 🏊."), null, null, null, null };
            var doubles = new double?[5] { null, 1.1, null, null, null };
            var int64s = new long?[5] { null, null, 1, null, null };
            var strings = new string[5] { null, null, null, "Running 🏃 is faster than swimming 🏊.", null };
            var timestamps = MakeTimestamps(5);
            var timestamp_values = new DateTime?[5] { null, null, null, DateTime.Parse("2021-01-01T00:00:00Z"), null };

            var batch = InsertWithColumnWithNull(ts.Alias, new QdbTableExpWriterOptions().Transactional(), blobs, doubles, int64s, strings, timestamps, timestamp_values);

            batch.Push();

            CheckTablesWithNull(ts, blobs, doubles, int64s, strings, timestamps, timestamp_values, strings);
        }

        [TestMethod]
        public void Ok_BulkRowInsert()
        {
            QdbTable ts = CreateTable();

            var blobs = MakeBlobArray(10);
            var doubles = MakeDoubleArray(10);
            var int64s = MakeInt64Array(10);
            var strings = MakeStringArray(10);
            var timestamps = MakeTimestamps(10);

            var batch = Insert(ts.Alias, new QdbTableExpWriterOptions().Transactional(), blobs, doubles, int64s, strings, timestamps);

            batch.Push();

            CheckTables(ts, blobs, doubles, int64s, strings, timestamps, strings);
        }

        [TestMethod]
        public void Ok_BulkRowInsertWithNullValues()
        {
            QdbTable ts = CreateTable();

            var blobs = new byte[5][] { System.Text.Encoding.UTF8.GetBytes("Running 🏃 is faster than swimming 🏊."), null, null, null, null };
            var doubles = new object[5] { null, 1.1, null, null, null };
            var int64s = new object[5] { null, null, 1, null, null };
            var strings = new object[5] { null, null, null, "Running 🏃 is faster than swimming 🏊.", null };
            var timestamps = MakeTimestamps(5);

            var batch = _cluster.ExpWriter([ts.Alias], new QdbTableExpWriterOptions().Transactional());

            for (var index = 0; index < doubles.Length; index++)
            {
                batch.Add(ts.Alias, timestamps[index], [blobs[index], doubles[index], int64s[index], strings[index], timestamps[index], strings[index]]);
            }

            batch.Push();

            // convert int64s[2] value to long for tests
            int64s[2] = (long)(int)int64s[2];

            var rows = ts.Reader().ToArray();
            Assert.AreEqual(rows.Length, blobs.Length);
            Assert.AreEqual(rows.Length, doubles.Length);
            Assert.AreEqual(rows.Length, int64s.Length);
            Assert.AreEqual(rows.Length, strings.Length);
            Assert.AreEqual(rows.Length, timestamps.Length);
            for (var idx = 0; idx < rows.Length; idx++)
            {
                Assert.AreEqual(rows[idx].Timestamp, timestamps[idx]);
                CollectionAssert.AreEqual(rows[idx]["the_blob"].BlobValue, blobs[idx]);
                AssertNullableDouble(rows[idx]["the_double"].DoubleValue, doubles[idx]);
                AssertNullableInt64(rows[idx]["the_int64"].Int64Value, int64s[idx]);
                Assert.AreEqual(rows[idx]["the_string"].StringValue, strings[idx]);
                Assert.AreEqual(rows[idx]["the_ts"].TimestampValue, timestamps[idx]);
                Assert.AreEqual(rows[idx]["the_symbol"].StringValue, strings[idx]);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(QdbException))]
        public void Ok_BulkRowInsertWithWrongName()
        {
            QdbTable ts = CreateTable();

            var blobs = MakeBlobArray(10);
            var timestamps = MakeTimestamps(10);

            var batch = _cluster.ExpWriter([ts.Alias], new QdbTableExpWriterOptions().Transactional());
            batch.Add("the_wrong_name", timestamps[0], []);

            batch.Push();
        }

        [TestMethod]
        public void Ok_BulkRowInsertByName()
        {
            QdbTable ts = CreateTable();

            var blobs = MakeBlobArray(10);
            var doubles = MakeDoubleArray(10);
            var int64s = MakeInt64Array(10);
            var strings = MakeStringArray(10);
            var timestamps = MakeTimestamps(10);

            var batch = InsertByName(ts.Alias, new QdbTableExpWriterOptions().Transactional(), blobs, doubles, int64s, strings, timestamps);

            batch.Push();

            CheckTables(ts, blobs, doubles, int64s, strings, timestamps, strings);
        }

        [TestMethod]
        public void Ok_BulkRowInsertUniqueByName()
        {
            QdbTable ts = CreateTable();

            var blobs = MakeBlobArray(10);
            var doubles = MakeDoubleArray(10);
            var int64s = MakeInt64Array(10);
            var strings = MakeStringArray(10);
            var timestamps = MakeTimestamps(10);

            // push normal
            {
                var batch = InsertByName(ts.Alias, new QdbTableExpWriterOptions().Transactional(), blobs, doubles, int64s, strings, timestamps);
                batch.Push();
            }
            // push with remove duplicate on all columns
            {
                var batch = InsertByName(ts.Alias, new QdbTableExpWriterOptions().Transactional().RemoveDuplicate(), blobs, doubles, int64s, strings, timestamps);
                batch.Push();
            }

            CheckTables(ts, blobs, doubles, int64s, strings, timestamps, strings);
        }

        [TestMethod]
        public void Ok_BulkRowInsertUniqueWithAllColumns()
        {
            QdbTable ts = CreateTable();

            var blobs = MakeBlobArray(10);
            var doubles = MakeDoubleArray(10);
            var int64s = MakeInt64Array(10);
            var strings = MakeStringArray(10);
            var timestamps = MakeTimestamps(10);

            // push normal
            {
                var batch = InsertByName(ts.Alias, new QdbTableExpWriterOptions().Transactional(), blobs, doubles, int64s, strings, timestamps);
                batch.Push();
            }
            // push with remove duplicate on all columns
            {
                var deduplicate_columns = new Dictionary<string, string[]>();
                deduplicate_columns.Add(ts.Alias, ["the_blob", "the_double", "the_int64", "the_ts", "the_symbol"]);
                var batch = InsertByName(ts.Alias, new QdbTableExpWriterOptions().Transactional().RemoveDuplicate(deduplicate_columns), blobs, doubles, int64s, strings, timestamps);
                batch.Push();
            }


            CheckTables(ts, blobs, doubles, int64s, strings, timestamps, strings);
        }

        [TestMethod]
        public void Ok_BulkRowInsertUniqueWithPartialColumns()
        {
            QdbTable ts = CreateTable();

            var blobs = MakeBlobArray(10);
            var doubles = MakeDoubleArray(10);
            var int64s = MakeInt64Array(10);
            var strings = MakeStringArray(10);
            var timestamps = MakeTimestamps(10);


            // push normal
            {
                var batch = InsertByName(ts.Alias, new QdbTableExpWriterOptions().Transactional(), blobs, doubles, int64s, strings, timestamps);
                batch.Push();
            }
            // push with remove duplicate on "the_ts" column, try to replace everything else
            // spoiler alert it will not replace anything
            {
                var new_blobs = MakeBlobArray(10);
                var new_doubles = MakeDoubleArray(10);
                var new_strings = MakeStringArray(10);
                var new_int64s = MakeInt64Array(10);

                var deduplicate_columns = new Dictionary<string, string[]>();
                deduplicate_columns.Add(ts.Alias, ["the_ts"]);
                var batch = InsertByName(ts.Alias, new QdbTableExpWriterOptions().Transactional().RemoveDuplicate(deduplicate_columns), new_blobs, new_doubles, new_int64s, new_strings, timestamps);
                batch.Push();
            }

            CheckTables(ts, blobs, doubles, int64s, strings, timestamps, strings);
        }

        [TestMethod]
        public void Ok_BulkRowInsertMultiTable()
        {
            QdbTable ts1 = CreateTableWithoutSymbol();
            QdbTable ts2 = CreateTableWithoutSymbol();
            QdbTable ts3 = CreateTableWithoutSymbol();
            QdbTable ts4 = CreateTableWithoutSymbol();
            QdbTable ts5 = CreateTableWithoutSymbol();

            var count = 10000;
            var blobs = MakeBlobArray(count);
            var doubles = MakeDoubleArray(count);
            var int64s = MakeInt64Array(count);
            var strings = MakeStringArray(count);
            var timestamps = MakeTimestamps(count);

            var batch = _cluster.ExpWriter([ts1.Alias, ts2.Alias, ts3.Alias, ts4.Alias, ts5.Alias], new QdbTableExpWriterOptions().Transactional());

            for (var i = 0; i < count; i++)
            {
                batch.Add(ts1.Alias, timestamps[i], [blobs[i], doubles[i], int64s[i], strings[i], timestamps[i]]);
                batch.Add(ts2.Alias, timestamps[i], [blobs[i], doubles[i], int64s[i], strings[i], timestamps[i]]);
                batch.Add(ts3.Alias, timestamps[i], [blobs[i], doubles[i], int64s[i], strings[i], timestamps[i]]);
                batch.Add(ts4.Alias, timestamps[i], [blobs[i], doubles[i], int64s[i], strings[i], timestamps[i]]);
                batch.Add(ts5.Alias, timestamps[i], [blobs[i], doubles[i], int64s[i], strings[i], timestamps[i]]);
            }

            batch.Push();

            CheckTablesWithoutSymbol(ts1, blobs, doubles, int64s, strings, timestamps);
            CheckTablesWithoutSymbol(ts2, blobs, doubles, int64s, strings, timestamps);
            CheckTablesWithoutSymbol(ts3, blobs, doubles, int64s, strings, timestamps);
            CheckTablesWithoutSymbol(ts4, blobs, doubles, int64s, strings, timestamps);
            CheckTablesWithoutSymbol(ts5, blobs, doubles, int64s, strings, timestamps);
        }

        [TestMethod]
        public void Ok_BulkRowInsertMultiWithoutSpecifyingTablesAtCreation()
        {
            QdbTable ts1 = CreateTableWithoutSymbol();
            QdbTable ts2 = CreateTableWithoutSymbol();

            var count = 10000;
            var blobs = MakeBlobArray(count);
            var doubles = MakeDoubleArray(count);
            var int64s = MakeInt64Array(count);
            var strings = MakeStringArray(count);
            var timestamps = MakeTimestamps(count);

            var batch = _cluster.ExpWriter(new QdbTableExpWriterOptions().Transactional());

            for (var i = 0; i < count; i++)
            {
                batch.Add(ts1.Alias, timestamps[i], [blobs[i], doubles[i], int64s[i], strings[i], timestamps[i]]);
                batch.Add(ts2.Alias, timestamps[i], [blobs[i], doubles[i], int64s[i], strings[i], timestamps[i]]);
            }

            batch.Push();

            CheckTablesWithoutSymbol(ts1, blobs, doubles, int64s, strings, timestamps);
            CheckTablesWithoutSymbol(ts2, blobs, doubles, int64s, strings, timestamps);
        }

        [TestMethod]
        public void Ok_SendOneTableThenTheOther()
        {
            QdbTable ts1 = CreateTableWithoutSymbol();
            QdbTable ts2 = CreateTableWithoutSymbol();

            var count = 10000;
            var blobs = MakeBlobArray(count);
            var doubles = MakeDoubleArray(count);
            var int64s = MakeInt64Array(count);
            var strings = MakeStringArray(count);
            var timestamps = MakeTimestamps(count);

            var batch = _cluster.ExpWriter(new QdbTableExpWriterOptions().Transactional());

            for (var i = 0; i < count; i++)
            {
                batch.Add(ts1.Alias, timestamps[i], [blobs[i], doubles[i], int64s[i], strings[i], timestamps[i]]);
            }
            batch.Push();
            for (var i = 0; i < count; i++)
            {
                batch.Add(ts2.Alias, timestamps[i], [blobs[i], doubles[i], int64s[i], strings[i], timestamps[i]]);
            }
            batch.Push();
            CheckTablesWithoutSymbol(ts1, blobs, doubles, int64s, strings, timestamps);
            CheckTablesWithoutSymbol(ts2, blobs, doubles, int64s, strings, timestamps);
        }
        [TestMethod]
        public void Ok_BulkRowInsertFast()
        {
            QdbTable ts = CreateTable();
            var blobs = MakeBlobArray(10);
            var doubles = MakeDoubleArray(10);
            var int64s = MakeInt64Array(10);
            var strings = MakeStringArray(10);
            var timestamps = MakeTimestamps(10);
            var batch = Insert(ts.Alias, new QdbTableExpWriterOptions().Fast(), blobs, doubles, int64s, strings, timestamps);
            batch.Push();

            CheckTables(ts, blobs, doubles, int64s, strings, timestamps, strings);
        }

        [TestMethod]
        public void Ok_BulkRowInsertFastByName()
        {
            QdbTable ts = CreateTable();

            var blobs = MakeBlobArray(10);
            var doubles = MakeDoubleArray(10);
            var int64s = MakeInt64Array(10);
            var strings = MakeStringArray(10);
            var timestamps = MakeTimestamps(10);

            var batch = InsertByName(ts.Alias, new QdbTableExpWriterOptions().Fast(), blobs, doubles, int64s, strings, timestamps);

            batch.Push();

            CheckTables(ts, blobs, doubles, int64s, strings, timestamps, strings);
        }

        [TestMethod]
        public void Ok_BulkRowInsertASync()
        {
            QdbTable ts = CreateTable();

            var blobs = MakeBlobArray(10);
            var doubles = MakeDoubleArray(10);
            var int64s = MakeInt64Array(10);
            var strings = MakeStringArray(10);
            var timestamps = MakeTimestamps(10);

            var batch = Insert(ts.Alias, new QdbTableExpWriterOptions().Async(), blobs, doubles, int64s, strings, timestamps);

            batch.Push();

            // Wait for push_async to complete
            // Ideally we could be able to get the proper flush interval
            Thread.Sleep(8 * 1000);

            CheckTables(ts, blobs, doubles, int64s, strings, timestamps, strings);
        }

        [TestMethod]
        public void Ok_BulkRowInsertASyncByName()
        {
            QdbTable ts = CreateTable();

            var blobs = MakeBlobArray(10);
            var doubles = MakeDoubleArray(10);
            var int64s = MakeInt64Array(10);
            var strings = MakeStringArray(10);
            var timestamps = MakeTimestamps(10);

            var batch = InsertByName(ts.Alias, new QdbTableExpWriterOptions().Async(), blobs, doubles, int64s, strings, timestamps);

            batch.Push();

            // Wait for push_async to complete
            // Ideally we could be able to get the proper flush interval
            Thread.Sleep(8 * 1000);

            CheckTables(ts, blobs, doubles, int64s, strings, timestamps, strings);
        }

        [TestMethod]
        public void Ok_BulkRowInsertTruncate()
        {
            QdbTable ts = CreateTable();
            var timestamps = MakeTimestamps(10);

            {
                var blobs = MakeBlobArray(10);
                var doubles = MakeDoubleArray(10);
                var int64s = MakeInt64Array(10);
                var strings = MakeStringArray(10);

                var batch = Insert(ts.Alias, new QdbTableExpWriterOptions().Transactional(), blobs, doubles, int64s, strings, timestamps);
                batch.Push();

                CheckTables(ts, blobs, doubles, int64s, strings, timestamps, strings);
            }

            {
                var blobs = MakeBlobArray(10);
                var doubles = MakeDoubleArray(10);
                var int64s = MakeInt64Array(10);
                var strings = MakeStringArray(10);

                var begin = timestamps[0];
                var end = timestamps[timestamps.Length - 1].AddSeconds(1);
                QdbTimeInterval interval = new QdbTimeInterval(begin, end);
                var batch = Insert(ts.Alias, new QdbTableExpWriterOptions().Truncate(interval), blobs, doubles, int64s, strings, timestamps);
                batch.Push();

                CheckTables(ts, blobs, doubles, int64s, strings, timestamps, strings);
            }
        }
    }
}
