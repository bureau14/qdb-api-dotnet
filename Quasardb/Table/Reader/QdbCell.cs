using System;
using System.Runtime.InteropServices;
using Quasardb.Exceptions;
using Quasardb.Native;

using qdb_int_t = System.Int64;
using size_t = System.UIntPtr;
using qdb_size_t = System.UIntPtr;
using pointer_t = System.IntPtr;

// ReSharper disable InconsistentNaming

namespace Quasardb.TimeSeries.Reader
{
    /// <summary>
    /// A variadic structure holding the type as well as the value.
    /// </summary>
    public class QdbCell
    {
        private readonly IntPtr _table;
        private readonly string _alias;
        private readonly qdb_ts_column_info _column;
        private readonly qdb_size_t _index;

        internal QdbCell(IntPtr table, string alias, qdb_ts_column_info column, qdb_size_t index)
        {
            _table = table;
            _alias = alias;
            _column = column;
            _index = index;
        }

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        /// <seealso cref="QdbColumnType"/>
        public QdbColumnType Type => (QdbColumnType)_column.type;

        /// <summary>
        /// Gets the value.
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
                        return StringValue;
                    case QdbColumnType.Symbol:
                        return SymbolValue;
                    case QdbColumnType.Timestamp:
                        return TimestampValue;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <exception cref="InvalidCastException">The value is not of type <see cref="QdbColumnType.Double" /> </exception>
        public double? DoubleValue
        {
            get
            {
                if (Type != QdbColumnType.Double)
                    throw new InvalidCastException();
                var err = qdb_api.qdb_ts_row_get_double(
                    _table, _index,
                    out double value);
                if (err == qdb_error.qdb_e_element_not_found)
                    return null;
                QdbExceptionThrower.ThrowIfNeeded(err, alias: _alias, column: _column.name);
                return value;
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <exception cref="InvalidCastException">The value is not of type <see cref="QdbColumnType.Blob" /> </exception>
        public byte[] BlobValue
        {
            get
            {
                if (Type != QdbColumnType.Blob)
                    throw new InvalidCastException();
                var err = qdb_api.qdb_ts_row_get_blob_no_copy(
                    _table, _index,
                    out pointer_t content, out size_t length);
                if (err == qdb_error.qdb_e_element_not_found)
                    return null;
                QdbExceptionThrower.ThrowIfNeeded(err, alias: _alias, column: _column.name);
                return Helper.GetBytes(content, (int)length);
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <exception cref="InvalidCastException">The value is not of type <see cref="QdbColumnType.Int64" /> </exception>
        public long? Int64Value
        {
            get
            {
                if (Type != QdbColumnType.Int64)
                    throw new InvalidCastException();
                var err = qdb_api.qdb_ts_row_get_int64(
                    _table, _index,
                    out qdb_int_t value);
                if (err == qdb_error.qdb_e_element_not_found)
                    return null;
                QdbExceptionThrower.ThrowIfNeeded(err, alias: _alias, column: _column.name);
                return value;
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <exception cref="InvalidCastException">The value is not of type <see cref="QdbColumnType.String" /> </exception>
        public string StringValue
        {
            get
            {
                if (Type != QdbColumnType.String)
                    throw new InvalidCastException();

                var err = qdb_api.qdb_ts_row_get_string_no_copy(
                    _table, _index,
                    out pointer_t content, out size_t length);
                if (err == qdb_error.qdb_e_element_not_found)
                    return null;
                QdbExceptionThrower.ThrowIfNeeded(err, alias: _alias, column: _column.name);
                // TODO: limited to 32-bit
                var value = Helper.GetBytes(content, (int)length);
                if (value == null)
                    return null;
                return System.Text.Encoding.UTF8.GetString(value);
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <exception cref="InvalidCastException">The value is not of type <see cref="QdbColumnType.Symbol" /> </exception>
        public string SymbolValue
        {
            get
            {
                if (Type != QdbColumnType.Symbol)
                    throw new InvalidCastException();

                var err = qdb_api.qdb_ts_row_get_symbol_no_copy(
                    _table, _index,
                    out pointer_t content, out size_t length);
                if (err == qdb_error.qdb_e_element_not_found)
                    return null;
                QdbExceptionThrower.ThrowIfNeeded(err, alias: _alias, column: _column.name);
                // TODO: limited to 32-bit
                var value = new byte[(int)length];
                Marshal.Copy(content, value, 0, (int)length);
                return System.Text.Encoding.UTF8.GetString(value);
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <exception cref="InvalidCastException">The value is not of type <see cref="QdbColumnType.Timestamp" /> </exception>
        public DateTime? TimestampValue
        {
            get
            {
                if (Type != QdbColumnType.Timestamp)
                    throw new InvalidCastException();
                var err = qdb_api.qdb_ts_row_get_timestamp(
                    _table, _index,
                    out qdb_timespec value);
                if (err == qdb_error.qdb_e_element_not_found)
                    return null;
                QdbExceptionThrower.ThrowIfNeeded(err, alias: _alias, column: _column.name);
                return TimeConverter.ToDateTime(value);
            }
        }
    }
}
