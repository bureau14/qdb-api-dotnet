using System;
using System.Text;
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
                    case QdbValueType.DoubleArray:
                        return DoubleArray;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the result reinterpreted as a double.
        /// </summary>
        /// <exception cref="InvalidCastException">The result is not of type <see cref="QdbValueType.Double" /> </exception>
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

        private unsafe byte[] PayloadToByteArray(qdb_point_result_blob_payload payload) 
        {
            return Helper.GetBytes(new IntPtr(payload.content), (int)payload.content_size);
        }

        private unsafe byte[] PayloadToByteArray(qdb_point_result_string_payload payload) 
        {
            return Helper.GetBytes(new IntPtr(payload.content), (int)payload.content_size);
        }

        /// <summary>
        /// Gets the result reinterpreted as a byte array.
        /// </summary>
        /// <exception cref="InvalidCastException">The result is not of type <see cref="QdbValueType.Blob" /> </exception>
        public unsafe byte[] BlobValue
        {
            get
            {
                if (Type == QdbValueType.None)
                    return null;
                if (Type != QdbValueType.Blob)
                    throw new InvalidCastException();
                return PayloadToByteArray(_result.blob_payload);
            }
        }

        /// <summary>
        /// Gets the result reinterpreted as a long.
        /// </summary>
        /// <exception cref="InvalidCastException">The result is not of type <see cref="QdbValueType.Int64" /> </exception>
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
        /// Gets the result reinterpreted as a DateTime.
        /// </summary>
        /// <exception cref="InvalidCastException">The result is not of type <see cref="QdbValueType.Timestamp" /> </exception>
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
        /// Gets the result reinterpreted as a long.
        /// </summary>
        /// <exception cref="InvalidCastException">The result is not of type <see cref="QdbValueType.Count" /> </exception>
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

        /// <summary>
        /// Gets the result reinterpreted as a string.
        /// </summary>
        /// <exception cref="InvalidCastException">The result is not of type <see cref="QdbValueType.String" /> </exception>
        public unsafe string StringValue
        {
            get
            {
                Encoding enc = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
                if (Type == QdbValueType.None)
                    return null;
                if (Type != QdbValueType.String)
                    throw new InvalidCastException();
                var content = PayloadToByteArray(_result.string_payload);
                if (content == null)
                    return null;
                return enc.GetString(content);
            }
        }

        /// <summary>
        /// Gets the result reinterpreted as a double array.
        /// </summary>
        /// <exception cref="InvalidCastException">The result is not of type <see cref="QdbValueType.DoubleArray" /> </exception>
        public unsafe double[] DoubleArray
        {
            get
            {
                if (Type == QdbValueType.None)
                    return null;
                if (Type != QdbValueType.DoubleArray)
                    throw new InvalidCastException();
                var arr = new double[(int)_result.double_array_payload.array_size];
                for (var i = 0L; i < (long)_result.double_array_payload.array_size; i++)
                    arr[i] = _result.double_array_payload.content[i];
                return arr;
            }
        }

        /// <summary>
        /// Gets the result reinterpreted as a int64 array.
        /// </summary>
        /// <exception cref="InvalidCastException">The result is not of type <see cref="QdbValueType.Int64Array" /> </exception>
        public unsafe long[] Int64Array
        {
            get
            {
                if (Type == QdbValueType.None)
                    return null;
                if (Type != QdbValueType.DoubleArray)
                    throw new InvalidCastException();
                var arr = new long[(int)_result.int64_array_payload.array_size];
                for (var i = 0L; i < (long)_result.int64_array_payload.array_size; i++)
                    arr[i] = _result.int64_array_payload.content[i];
                return arr;
            }
        }

        /// <summary>
        /// Gets the result reinterpreted as an array of bytes.
        /// </summary>
        /// <exception cref="InvalidCastException">The result is not of type <see cref="QdbValueType.BlobArray" /> </exception>
        public unsafe byte[][] BlobArray
        {
            get
            {
                if (Type == QdbValueType.None)
                    return null;
                if (Type != QdbValueType.DoubleArray)
                    throw new InvalidCastException();
                var arr = new byte[(int)_result.blob_array_payload.array_size][];
                for (var i = 0L; i < (long)_result.blob_array_payload.array_size; i++)
                    arr[i] = Helper.GetBytes(new IntPtr(_result.blob_array_payload.content[i].content), (int)_result.blob_array_payload.content[i].content_size);
                return arr;
            }
        }

        /// <summary>
        /// Gets the result value reinterpreted as a DateTime Array.
        /// </summary>
        /// <exception cref="InvalidCastException">The result is not of type <see cref="QdbValueType.TimestampArray" /> </exception>
        public unsafe DateTime[] TimestampArray
        {
            get
            {
                if (Type == QdbValueType.None)
                    return null;
                if (Type != QdbValueType.DoubleArray)
                    throw new InvalidCastException();
                var arr = new DateTime[(int)_result.timestamp_array_payload.array_size];
                for (var i = 0L; i < (long)_result.timestamp_array_payload.array_size; i++)
                    arr[i] = TimeConverter.ToDateTime(_result.timestamp_array_payload.content[i]);
                return arr;
            }
        }

        /// <summary>
        /// Gets the result reinterpreted as a string array.
        /// </summary>
        /// <exception cref="InvalidCastException">The result is not of type <see cref="QdbValueType.StringArray" /> </exception>
        public unsafe string[] StringArray
        {
            get
            {
                if (Type == QdbValueType.None)
                    return null;
                if (Type != QdbValueType.DoubleArray)
                    throw new InvalidCastException();
                var arr = new string[(int)_result.string_array_payload.array_size];
                Encoding enc = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
                for (var i = 0L; i < (long)_result.string_array_payload.array_size; i++)
                {
                    var content = Helper.GetBytes(new IntPtr(_result.string_array_payload.content[i].data), (int)_result.string_array_payload.content[i].length);
                    arr[i] = (content == null ? null : enc.GetString(content));
                }
                return arr;
            }
        }
    }
}
