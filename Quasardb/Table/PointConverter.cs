using System;
using System.Runtime.InteropServices;
using Quasardb.Native;

namespace Quasardb.TimeSeries
{
    static class PointConverter
    {
        public static qdb_ts_double_point ToNative(this QdbPoint<double?> pt)
        {
            return new qdb_ts_double_point
            {
                timestamp = TimeConverter.ToTimespec(pt.Time),
                value = pt.Value ?? double.NaN
            };
        }

        public static bool IsNull(qdb_ts_double_point pt)
        {
            return double.IsNaN(pt.value);
        }

        public static QdbDoublePoint ToManaged(this qdb_ts_double_point pt)
        {
            return new QdbDoublePoint(
                TimeConverter.ToDateTime(pt.timestamp),
                IsNull(pt) ? (double?)null : pt.value);
        }

        public static unsafe qdb_ts_blob_point ToNative(this QdbPoint<byte[]> pt, out GCHandle? pin)
        {
            if (pt.Value == null)
            {
                pin = null;
                return new qdb_ts_blob_point
                {
                    timestamp = TimeConverter.ToTimespec(pt.Time),
                    content = null,
                    content_size = (UIntPtr)0L
                };
            }

            pin = GCHandle.Alloc(pt.Value, GCHandleType.Pinned);
            return new qdb_ts_blob_point
            {
                timestamp = TimeConverter.ToTimespec(pt.Time),
                content = (void*)pin.Value.AddrOfPinnedObject(),
                content_size = (UIntPtr)pt.Value.Length
            };
        }

        public static bool IsNull(qdb_ts_blob_point pt)
        {
            return pt.content_size == (UIntPtr)0L;
        }

        public static unsafe QdbBlobPoint ToManaged(this qdb_ts_blob_point pt)
        {
            if (IsNull(pt))
            {
                return new QdbBlobPoint(TimeConverter.ToDateTime(pt.timestamp), null);
            }

            // TODO: limited to 32-bit
            var content = new byte[(int)pt.content_size];
            Marshal.Copy(new IntPtr(pt.content), content, 0, (int)pt.content_size);
            return new QdbBlobPoint(TimeConverter.ToDateTime(pt.timestamp), content);
        }


        public static unsafe qdb_ts_string_point ToNative(this QdbPoint<string> pt, out GCHandle? pin)
        {
            if (pt.Value == null)
            {
                pin = null;
                return new qdb_ts_string_point
                {
                    timestamp = TimeConverter.ToTimespec(pt.Time),
                    content = null,
                    content_size = (UIntPtr)0L
                };
            }

            pin = GCHandle.Alloc(pt.Value, GCHandleType.Pinned);
            return new qdb_ts_string_point
            {
                timestamp = TimeConverter.ToTimespec(pt.Time),
                content = (char*)pin.Value.AddrOfPinnedObject(),
                content_size = (UIntPtr)pt.Value.Length
            };
        }

        public static bool IsNull(qdb_ts_string_point pt)
        {
            return pt.content_size == (UIntPtr)0L;
        }

        public static unsafe QdbStringPoint ToManaged(this qdb_ts_string_point pt)
        {
            if (IsNull(pt))
            {
                return new QdbStringPoint(TimeConverter.ToDateTime(pt.timestamp), null);
            }

            // TODO: limited to 32-bit
            var content = new byte[(int)pt.content_size];
            Marshal.Copy(new IntPtr(pt.content), content, 0, (int)pt.content_size);
            return new QdbStringPoint(TimeConverter.ToDateTime(pt.timestamp), System.Text.Encoding.UTF8.GetString(content));
        }

        public static unsafe qdb_ts_symbol_point ToSymbolNative(this QdbPoint<string> pt, out GCHandle? pin)
        {
            if (pt.Value == null)
            {
                pin = null;
                return new qdb_ts_symbol_point
                {
                    timestamp = TimeConverter.ToTimespec(pt.Time),
                    content = null,
                    content_size = (UIntPtr)0L
                };
            }

            pin = GCHandle.Alloc(pt.Value, GCHandleType.Pinned);
            return new qdb_ts_symbol_point
            {
                timestamp = TimeConverter.ToTimespec(pt.Time),
                content = (char*)pin.Value.AddrOfPinnedObject(),
                content_size = (UIntPtr)pt.Value.Length
            };
        }

        public static bool IsNull(qdb_ts_symbol_point pt)
        {
            return pt.content_size == (UIntPtr)0L;
        }

        public static unsafe QdbSymbolPoint ToManaged(this qdb_ts_symbol_point pt)
        {
            if (IsNull(pt))
            {
                return new QdbSymbolPoint(TimeConverter.ToDateTime(pt.timestamp), null);
            }

            // TODO: limited to 32-bit
            var content = new byte[(int)pt.content_size];
            Marshal.Copy(new IntPtr(pt.content), content, 0, (int)pt.content_size);
            return new QdbSymbolPoint(TimeConverter.ToDateTime(pt.timestamp), System.Text.Encoding.UTF8.GetString(content));
        }

        public static qdb_ts_int64_point ToNative(this QdbPoint<long?> pt)
        {
            return new qdb_ts_int64_point
            {
                timestamp = TimeConverter.ToTimespec(pt.Time),
                value = pt.Value ?? long.MinValue
            };
        }

        public static bool IsNull(qdb_ts_int64_point pt)
        {
            return pt.value == long.MinValue;
        }

        public static QdbInt64Point ToManaged(this qdb_ts_int64_point pt)
        {
            return new QdbInt64Point(
                TimeConverter.ToDateTime(pt.timestamp),
                IsNull(pt) ? (long?)null : pt.value);
        }

        public static qdb_ts_timestamp_point ToNative(this QdbPoint<DateTime?> pt)
        {
            return new qdb_ts_timestamp_point
            {
                timestamp = TimeConverter.ToTimespec(pt.Time),
                value = TimeConverter.ToTimespec(pt.Value)
            };
        }

        public static bool IsNull(qdb_ts_timestamp_point pt)
        {
            return TimeConverter.IsNull(pt.value);
        }

        public static QdbTimestampPoint ToManaged(this qdb_ts_timestamp_point pt)
        {
            return new QdbTimestampPoint(
                TimeConverter.ToDateTime(pt.timestamp),
                IsNull(pt) ? (DateTime?)null : TimeConverter.ToDateTime(pt.value));
        }
    }
}
