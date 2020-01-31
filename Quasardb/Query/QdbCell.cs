using System;
using System.Runtime.InteropServices;
using Quasardb.Native;

// ReSharper disable InconsistentNaming

namespace Quasardb.Query
{
    /// <summary>
    /// A variadic structure holding the result type as well as the result value.
    /// </summary>
    public class QdbCell
    {
        private readonly qdb_point_result _result;

        internal QdbCell(qdb_point_result result)
        {
            _result = result;
        }

        /// <summary>
        /// Gets the type of result value.
        /// </summary>
        /// <seealso cref="QdbValueType"/>
        public QdbValueType Type => (QdbValueType)_result.type;

        /// <summary>
        /// Gets the result value.
        /// </summary>
        public object Value
        {
            get
            {
                switch (Type)
                {
                    case QdbValueType.None:
                        return null;
                    case QdbValueType.Double:
                        return DoubleValue;
                    case QdbValueType.Blob:
                        return BlobValue;
                    case QdbValueType.Int64:
                        return Int64Value;
                    case QdbValueType.Timestamp:
                        return TimestampValue;
                    case QdbValueType.Count:
                        return CountValue;
                    case QdbValueType.String:
                        return StringValue;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the result value.
        /// </summary>
        /// <exception cref="InvalidCastException">The result value is not of type <see cref="QdbValueType.Double" /> </exception>
        public double? DoubleValue
        {
            get
            {
                if (Type == QdbValueType.None)
                    return null;
                if (Type != QdbValueType.Double)
                    throw new InvalidCastException();
                return _result.double_payload.value;
            }
        }

        /// <summary>
        /// Gets the result value.
        /// </summary>
        /// <exception cref="InvalidCastException">The result value is not of type <see cref="QdbValueType.Int64" /> </exception>
        public long? Int64Value
        {
            get
            {
                if (Type == QdbValueType.None)
                    return null;
                if (Type != QdbValueType.Int64)
                    throw new InvalidCastException();
                return _result.int64_payload.value;
            }
        }

        /// <summary>
        /// Gets the result value.
        /// </summary>
        /// <exception cref="InvalidCastException">The result value is not of type <see cref="QdbValueType.Blob" /> </exception>
        public unsafe byte[] BlobValue
        {
            get
            {
                if (Type == QdbValueType.None)
                    return null;
                if (Type != QdbValueType.Blob)
                    throw new InvalidCastException();
                // TODO: limited to 32-bit
                var content = new byte[(int)_result.blob_payload.content_size];
                Marshal.Copy(new IntPtr(_result.blob_payload.content), content, 0, (int)_result.blob_payload.content_size);
                return content;
            }
        }

        /// <summary>
        /// Gets the result value reinterpreted as a string.
        /// </summary>
        /// <exception cref="InvalidCastException">The result value is not of type <see cref="QdbValueType.String" /> </exception>
        public unsafe string StringValue
        {
            get
            {
                if (Type == QdbValueType.None)
                    return null;
                if (Type != QdbValueType.String)
                    throw new InvalidCastException();
                // TODO: limited to 32-bit
                var content = new byte[(int)_result.string_payload.content_size];
                Marshal.Copy(new IntPtr(_result.string_payload.content), content, 0, (int)_result.string_payload.content_size);
                return content != null ? System.Text.Encoding.UTF8.GetString(content) : null;
            }
        }

        /// <summary>
        /// Gets the result value.
        /// </summary>
        /// <exception cref="InvalidCastException">The result value is not of type <see cref="QdbValueType.Timestamp" /> </exception>
        public DateTime? TimestampValue
        {
            get
            {
                if (Type == QdbValueType.None)
                    return null;
                if (Type != QdbValueType.Timestamp)
                    throw new InvalidCastException();
                return TimeConverter.ToDateTime(_result.timestamp_payload.value);
            }
        }

        /// <summary>
        /// Gets the result value.
        /// </summary>
        /// <exception cref="InvalidCastException">The result value is not of type <see cref="QdbValueType.Count" /> </exception>
        public long? CountValue
        {
            get
            {
                if (Type == QdbValueType.None)
                    return null;
                if (Type != QdbValueType.Count)
                    throw new InvalidCastException();
                return (long)_result.count_payload.value.ToUInt64();
            }
        }
    }
}
