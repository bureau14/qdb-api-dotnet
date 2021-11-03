﻿using System;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;
using Quasardb.TimeSeries.ExpWriter;

namespace Quasardb.Tests.Table
{
    [TestClass]
    public class ExpWriter
    {
        private readonly QdbCluster _cluster = QdbTestCluster.Instance;

        public DateTime[] MakeTimestamps(int count)
        {
            Random random = new Random();
            var r = new DateTime[count];

            DateTime date = DateTime.Parse("2021-01-01T00:00:00Z");
            for (int i = 0; i < count; ++i)
            {
                r[i] = date.AddSeconds(i);
            }
            return r;
        }

        public byte[][] MakeBlobArray(int count)
        {
            Random random = new Random();
            var r = new byte[count][];

            for (int i = 0; i < count; ++i)
            {
                var value = new byte[32];
                random.NextBytes(value);
                r[i] = value;
            }
            return r;
        }

        public double[] MakeDoubleArray(int count)
        {
            Random random = new Random();
            var r = new double[count];

            for (int i = 0; i < count; ++i)
            {
                r[i] = (double)i;
            }
            return r;
        }

        public long[] MakeInt64Array(int count)
        {
            Random random = new Random();
            var r = new long[count];

            for (int i = 0; i < count; ++i)
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

        public string[] MakeStringArray(int count)
        {
            Random random = new Random();
            var r = new string[count];

            for (int i = 0; i < count; ++i)
            {
                r[i] = RandomString(32, random);
            }
            return r;
        }

        public DateTime[] CreateTimestampPoints(int count)
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

        public QdbTableExpWriter Insert(string ts,
            byte[][] blobs,
            double[] doubles,
            long[] int64s,
            string[] strings,
            DateTime[] timestamps)
        {
            var batch = _cluster.ExpWriter(ts);
            batch.SetTimestamps(timestamps);
            batch.SetBlobColumn(0, blobs);
            batch.SetDoubleColumn(1, doubles);
            batch.SetInt64Column(2, int64s);
            batch.SetStringColumn(3, strings);
            batch.SetTimestampColumn(4, timestamps);
            batch.SetStringColumn(5, strings);
            return batch;
        }

        public void CheckTables(QdbTable ts,
            byte[][] blobs,
            double[] doubles,
            long[] ints,
            string[] strings,
            DateTime[] timestamps)
        {
            var blob_arr = ts.BlobColumns["the_blob"].Points().ToArray();
            var double_arr = ts.DoubleColumns["the_double"].Points().ToArray();
            var int_arr = ts.Int64Columns["the_int64"].Points().ToArray();
            var string_arr = ts.StringColumns["the_string"].Points().ToArray();
            var ts_arr = ts.TimestampColumns["the_ts"].Points().ToArray();
            var symbol_arr = ts.SymbolColumns["the_symbol"].Points().ToArray();
            for (int idx = 0; idx < timestamps.Length; idx++)
            {
                Assert.AreEqual(blob_arr[idx].Time, timestamps[idx]);
                CollectionAssert.AreEqual(blob_arr[idx].Value, blobs[idx]);
                Assert.AreEqual(double_arr[idx].Value, doubles[idx]);
                Assert.AreEqual(int_arr[idx].Value, ints[idx]);
                Assert.AreEqual(string_arr[idx].Value, strings[idx]);
                Assert.AreEqual(ts_arr[idx].Value, timestamps[idx]);
                Assert.AreEqual(symbol_arr[idx].Value, strings[idx]);
            }
        }

        [TestMethod]
        public void Ok_BulkRowInsert()
        {
            QdbTable ts = CreateTable();

            var blobs      = MakeBlobArray(10);
            var doubles    = MakeDoubleArray(10);
            var int64s     = MakeInt64Array(10);
            var strings    = MakeStringArray(10);
            var timestamps = MakeTimestamps(10);

            var batch = Insert(ts.Alias, blobs, doubles, int64s, strings, timestamps);

            batch.Push();

            CheckTables(ts, blobs, doubles, int64s, strings, timestamps);
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

            var batch = Insert(ts.Alias, blobs, doubles, int64s, strings, timestamps);

            batch.PushFast();

            CheckTables(ts, blobs, doubles, int64s, strings, timestamps);
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

            var batch = Insert(ts.Alias, blobs, doubles, int64s, strings, timestamps);

            batch.PushAsync();

            // Wait for push_async to complete
            // Ideally we could be able to get the proper flush interval
            Thread.Sleep(8 * 1000);

            CheckTables(ts, blobs, doubles, int64s, strings, timestamps);
        }
    }
}
