using System;
using System.Collections.Generic;
using System.Text;
using Quasardb.Native;

namespace Quasardb.TimeSeries
{
    /// <summary>
    /// Describes a time-series column, required to create the column
    /// </summary>
    public abstract class QdbColumnDefinition
    {
        /// <summary>
        /// The name of the new column
        /// </summary>
        public string Name { get; }

        internal qdb_ts_column_type Type { get; }

        internal string Symtable { get; }


        internal QdbColumnDefinition(string name, qdb_ts_column_type type)
        {
            Name = name;
            Type = type;
        }
        
        internal QdbColumnDefinition(string name, qdb_ts_column_type type, string symtable)
        {
            Name     = name;
            Type     = type;
            Symtable = symtable;
        }
    }

    /// <summary>
    /// Describes a time-series column that contains blobs
    /// </summary>
    public class QdbBlobColumnDefinition : QdbColumnDefinition
    {
        /// <summary>
        /// Creates a column description with the specified name.
        /// </summary>
        /// <param name="name">The name of the column</param>
        public QdbBlobColumnDefinition(string name) : base(name, qdb_ts_column_type.qdb_ts_column_blob)
        {
        }
    }

    /// <summary>
    /// Describes a time-series column that contains double precision floating point values.
    /// </summary>
    public class QdbDoubleColumnDefinition : QdbColumnDefinition
    {
        /// <summary>
        /// Creates a column description with the specified name.
        /// </summary>
        /// <param name="name">The name of the column</param>
        public QdbDoubleColumnDefinition(string name) : base(name, qdb_ts_column_type.qdb_ts_column_double)
        {
        }
    }

    /// <summary>
    /// Describes a time-series column that contains int64 point values.
    /// </summary>
    public class QdbInt64ColumnDefinition : QdbColumnDefinition
    {
        /// <summary>
        /// Creates a column description with the specified name.
        /// </summary>
        /// <param name="name">The name of the column</param>
        public QdbInt64ColumnDefinition(string name) : base(name, qdb_ts_column_type.qdb_ts_column_int64)
        {
        }
    }

    /// <summary>
    /// Describes a time-series column that contains string point values.
    /// </summary>
    public class QdbStringColumnDefinition : QdbColumnDefinition
    {
        /// <summary>
        /// Creates a column description with the specified name.
        /// </summary>
        /// <param name="name">The name of the column</param>
        public QdbStringColumnDefinition(string name) : base(name, qdb_ts_column_type.qdb_ts_column_string)
        {
        }
    }

    /// <summary>
    /// Describes a time-series column that contains timestamp point values.
    /// </summary>
    public class QdbTimestampColumnDefinition : QdbColumnDefinition
    {
        /// <summary>
        /// Creates a column description with the specified name.
        /// </summary>
        /// <param name="name">The name of the column</param>
        public QdbTimestampColumnDefinition(string name) : base(name, qdb_ts_column_type.qdb_ts_column_timestamp)
        {
        }
    }
    
    /// <summary>
    /// Describes a time-series column that contains symbol point values.
    /// </summary>
    public class QdbSymbolColumnDefinition : QdbColumnDefinition
    {
        /// <summary>
        /// Creates a column description with the specified name.
        /// </summary>
        /// <param name="name">The name of the column</param>
        /// <param name="symtable">The symbol table name</param>
        public QdbSymbolColumnDefinition(string name, string symtable) : base(name, qdb_ts_column_type.qdb_ts_column_symbol, symtable)
        {
        }
    }
}
