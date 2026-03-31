using System;
using System.Collections.Generic;
using Quasardb.Exceptions;
using Quasardb.Native;

namespace Quasardb.TimeSeries
{
    /// <summary>
    /// A column of a table
    /// </summary>
    public class QdbColumn
    {
        internal QdbColumn(QdbTable series, string name, qdb_ts_column_type type)
        {
            Series = series;
            Name = name;
            Type = ToColumnType(type);
        }

        internal QdbColumn(QdbTable series, string name, qdb_ts_column_type type, string symtable)
        {
            Series = series;
            Name = name;
            Type = ToColumnType(type);
            Symtable = symtable;
        }

        /// <summary>
        /// The parent of the column
        /// </summary>
        public QdbTable Series { get; }

        /// <summary>
        /// The name of the column
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The type of the column.
        /// </summary>
        public QdbColumnType Type { get; }

        /// <summary>
        /// The symtable name of the column
        /// </summary>
        public string Symtable { get; }

        internal qdb_handle Handle => Series.Handle;

        private static QdbColumnType ToColumnType(qdb_ts_column_type type)
        {
            switch (type)
            {
                case qdb_ts_column_type.qdb_ts_column_double:
                    return QdbColumnType.Double;
                case qdb_ts_column_type.qdb_ts_column_blob:
                    return QdbColumnType.Blob;
                case qdb_ts_column_type.qdb_ts_column_int64:
                    return QdbColumnType.Int64;
                case qdb_ts_column_type.qdb_ts_column_timestamp:
                    return QdbColumnType.Timestamp;
                case qdb_ts_column_type.qdb_ts_column_string:
                    return QdbColumnType.String;
                case qdb_ts_column_type.qdb_ts_column_symbol:
                    return QdbColumnType.Symbol;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported column type.");
            }
        }
    }
}
