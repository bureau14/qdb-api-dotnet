using System;
using System.Runtime.InteropServices;
using Quasardb.Native;

namespace Quasardb.TimeSeries.Reader
{
    public unsafe class QdbBulkCell
    {
        readonly qdb_exp_batch_push_column* _column;
        readonly long _index;

        internal QdbBulkCell(qdb_exp_batch_push_column* column, long index)
        {
            _column = column;
            _index = index;
        }

        public QdbColumnType Type => (QdbColumnType)_column->data_type;

        public object Value
        {
            get
            {
                switch (Type)
                {
                    case QdbColumnType.Double:
                        return DoubleValue;
                    case QdbColumnType.Blob:
                        return BlobValue;
                    case QdbColumnType.Int64:
                        return Int64Value;
                    case QdbColumnType.String:
                    case QdbColumnType.Symbol:
                        return StringValue;
                    case QdbColumnType.Timestamp:
                        return TimestampValue;
                }
                return null;
            }
        }

        public double? DoubleValue
        {
            get
            {
                if (Type != QdbColumnType.Double)
                    throw new InvalidCastException();
                double v = _column->data.doubles[_index];
                return double.IsNaN(v) ? (double?)null : v;
            }
        }

        public byte[] BlobValue
        {
            get
            {
                if (Type != QdbColumnType.Blob)
                    throw new InvalidCastException();
                qdb_blob blob = _column->data.blobs[_index];
                if (blob.content == null)
                    return null;
                return Helper.GetBytes(new IntPtr(blob.content), blob.content_size);
            }
        }

        public long? Int64Value
        {
            get
            {
                if (Type != QdbColumnType.Int64)
                    throw new InvalidCastException();
                long v = _column->data.ints[_index];
                return v == unchecked((long)0x8000000000000000) ? (long?)null : v;
            }
        }

        public string StringValue
        {
            get
            {
                if (Type != QdbColumnType.String && Type != QdbColumnType.Symbol)
                    throw new InvalidCastException();
                qdb_sized_string str = _column->data.strings[_index];
                if (str.data == null)
                    return null;
                return Marshal.PtrToStringAnsi(new IntPtr(str.data), (int)str.length);
            }
        }

        public DateTime? TimestampValue
        {
            get
            {
                if (Type != QdbColumnType.Timestamp)
                    throw new InvalidCastException();
                qdb_timespec ts = _column->data.timestamps[_index];
                if (ts.tv_sec == long.MinValue && ts.tv_nsec == long.MinValue)
                    return null;
                return TimeConverter.ToDateTime(ts);
            }
        }
    }
}
