using System;
using System.Runtime.InteropServices;
using Quasardb.Native;

namespace Quasardb.TimeSeries.Reader
{
    /// <summary>
    /// Represents a single cell in a bulk-read operation from a QuasarDB time series table.
    /// </summary>
    public unsafe class QdbBulkCell
    {
        readonly qdb_exp_batch_push_column* _column;
        readonly long _index;

        /// <summary>
        /// Initializes a new instance of the <see cref="QdbBulkCell"/> class.
        /// </summary>
        /// <param name="column">The native column pointer.</param>
        /// <param name="index">The index of the cell in the column.</param>
        internal QdbBulkCell(qdb_exp_batch_push_column* column, long index)
        {
            _column = column;
            _index = index;
        }

        /// <summary>
        /// Gets the type of the column.
        /// </summary>
        public QdbColumnType Type => (QdbColumnType)_column->data_type;

        /// <summary>
        /// Gets the value of the cell as an object.
        /// </summary>
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

        /// <summary>
        /// Gets the value of the cell as a nullable double.
        /// </summary>
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

        /// <summary>
        /// Gets the value of the cell as a byte array.
        /// </summary>
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

        /// <summary>
        /// Gets the value of the cell as a nullable 64-bit integer.
        /// </summary>
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

        /// <summary>
        /// Gets the value of the cell as a string.
        /// </summary>
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

        /// <summary>
        /// Gets the value of the cell as a nullable <see cref="DateTime"/>.
        /// </summary>
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
