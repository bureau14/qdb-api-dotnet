using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Quasardb.Exceptions;
using Quasardb.Native;
using Quasardb.TimeSeries.Reader;

namespace Quasardb.TimeSeries
{
    /// <summary>
    /// A collection of columns
    /// </summary>
    public class QdbColumnCollection : IEnumerable<QdbColumn>
    {
        internal readonly QdbTable _series;

        internal QdbColumnCollection(QdbTable series)
        {
            _series = series;
        }

        /// <inheritdoc />
        public IEnumerator<QdbColumn> GetEnumerator()
        {
            var handle = _series.Handle;
            var alias = _series.Alias;

            using (var columns = new qdb_buffer<qdb_ts_column_info_ex>(handle))
            {
                var err = qdb_api.qdb_ts_list_columns_ex(handle, alias, out columns.Pointer, out columns.Size);
                QdbExceptionThrower.ThrowIfNeeded(err, alias: alias);

                foreach (var column in columns)
                    yield return MakeColumn(column.type, column.name, column.symtable);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        QdbColumn MakeColumn(qdb_ts_column_type type, string name, string symtable)
        {
            if (type == qdb_ts_column_type.qdb_ts_column_string || type == qdb_ts_column_type.qdb_ts_column_symbol)
            {
                return new QdbColumn(_series, name, type, symtable);
            }

            return new QdbColumn(_series, name, type);
        }
    }

    /// <summary>
    /// A table
    /// </summary>
    public class QdbTable : QdbEntry
    {
        internal QdbTable(qdb_handle handle, string alias) : base(handle, alias)
        {
            Columns = new QdbColumnCollection(this);
        }

        /// <summary>
        /// The columns of the table.
        /// </summary>
        public QdbColumnCollection Columns { get; }

        /// <summary>
        /// Returns the shard size of a table.
        /// </summary>
        public unsafe TimeSpan ShardSize
        {
            get
            {
                qdb_ts_metadata* metadata;
                var err = qdb_api.qdb_ts_get_metadata(Handle, Alias, out metadata);
                QdbExceptionThrower.ThrowIfNeeded(err, alias: Alias);
                qdb_duration shardSize = metadata->shard_size;

                qdb_api.qdb_release(Handle, new IntPtr(metadata));
                return TimeSpan.FromMilliseconds((double)shardSize);
            }
        }

        internal InteropableList<qdb_ts_column_info_ex> GetColumnDefinitions()
        {
            using (var columns = new qdb_buffer<qdb_ts_column_info_ex>(Handle))
            {
                var err = qdb_api.qdb_ts_list_columns_ex(Handle, Alias, out columns.Pointer, out columns.Size);
                QdbExceptionThrower.ThrowIfNeeded(err, alias: Alias);

                var columnDefinitions = new InteropableList<qdb_ts_column_info_ex>((int)columns.Size);
                foreach (var def in columns)
                {
                    columnDefinitions.Add(def);
                }
                return columnDefinitions;
            }
        }

        /// <summary>
        /// Creates the time-series.
        /// </summary>
        /// <param name="columnDefinitions">The description of the columns</param>
        /// <exception cref="QdbAliasAlreadyExistsException">If the time-series already exists.</exception>
        /// <exception cref="QdbIncompatibleTypeException">If the alias matches with an entry of another type.</exception>
        public void Create(params QdbColumnDefinition[] columnDefinitions)
        {
            Create((IEnumerable<QdbColumnDefinition>)columnDefinitions);
        }

        /// <summary>
        /// Creates the time-series.
        /// </summary>
        /// <param name="shardSize">The size of a single shard (bucket)</param>
        /// <param name="columnDefinitions">The description of the columns</param>
        /// <exception cref="QdbAliasAlreadyExistsException">If the time-series already exists.</exception>
        /// <exception cref="QdbIncompatibleTypeException">If the alias matches with an entry of another type.</exception>
        /// <exception cref="QdbInvalidArgumentException">If shard size is less than one millisecond or greater than maximum allowed length.</exception>
        public void Create(TimeSpan shardSize, params QdbColumnDefinition[] columnDefinitions)
        {
            Create(shardSize, (IEnumerable<QdbColumnDefinition>)columnDefinitions);
        }

        /// <summary>
        /// Creates the time-series.
        /// </summary>
        /// <param name="columnDefinitions">The description of the columns</param>
        /// <exception cref="QdbAliasAlreadyExistsException">If the time-series already exists.</exception>
        /// <exception cref="QdbIncompatibleTypeException">If the alias matches with an entry of another type.</exception>
        public void Create(IEnumerable<QdbColumnDefinition> columnDefinitions)
        {
            Create(TimeSpan.FromMilliseconds(
                       (double)qdb_duration.qdb_d_default_shard_size),
                   columnDefinitions);
        }

        /// <summary>
        /// Creates the time-series.
        /// </summary>
        /// <param name="shardSize">The size of a single shard (bucket)</param>
        /// <param name="columnDefinitions">The description of the columns</param>
        /// <exception cref="QdbAliasAlreadyExistsException">If the time-series already exists.</exception>
        /// <exception cref="QdbIncompatibleTypeException">If the alias matches with an entry of another type.</exception>
        /// <exception cref="QdbInvalidArgumentException">If shard size is less than one millisecond or greater than maximum allowed length.</exception>
        public void Create(TimeSpan shardSize, IEnumerable<QdbColumnDefinition> columnDefinitions)
        {
            var columns = NormalizeCreateColumns(columnDefinitions);

            var err = qdb_api.qdb_ts_create_ex(
                Handle, Alias,
                (ulong)(shardSize.TotalMilliseconds *
                        (double)qdb_duration.qdb_d_millisecond),
                columns.Buffer, columns.Count, 0);
            QdbExceptionThrower.ThrowIfNeeded(err, alias: Alias);
        }

        private static InteropableList<qdb_ts_column_info_ex> NormalizeCreateColumns(IEnumerable<QdbColumnDefinition> columnDefinitions)
        {
            var definitions = columnDefinitions ?? Array.Empty<QdbColumnDefinition>();
            var count = Helpers.GetCountOrDefault(definitions);
            var userColumns = new List<qdb_ts_column_info_ex>(count);
            QdbColumnDefinition timestampDefinition = null;

            foreach (var def in definitions)
            {
                if (def == null)
                {
                    throw new ArgumentException("Column definitions cannot contain null values.", nameof(columnDefinitions));
                }

                if (def.Name == "$timestamp")
                {
                    if (timestampDefinition != null)
                    {
                        throw new ArgumentException("The $timestamp column can only be specified once.", nameof(columnDefinitions));
                    }

                    if (def.Type != qdb_ts_column_type.qdb_ts_column_timestamp)
                    {
                        throw new ArgumentException("The $timestamp column must have TIMESTAMP type.", nameof(columnDefinitions));
                    }

                    if (!string.IsNullOrEmpty(def.Symtable))
                    {
                        throw new ArgumentException("The $timestamp column cannot define symbols.", nameof(columnDefinitions));
                    }

                    timestampDefinition = def;
                    continue;
                }

                userColumns.Add(ToNativeColumnInfo(def));
            }

            var columns = new InteropableList<qdb_ts_column_info_ex>(userColumns.Count + 1);
            columns.Add(ToNativeColumnInfo(timestampDefinition ?? new QdbTimestampColumnDefinition("$timestamp")));

            foreach (var column in userColumns)
            {
                columns.Add(column);
            }

            return columns;
        }

        private static qdb_ts_column_info_ex ToNativeColumnInfo(QdbColumnDefinition definition)
        {
            return new qdb_ts_column_info_ex
            {
                name = definition.Name,
                type = definition.Type,
                symtable = definition.Symtable
            };
        }

        /// <summary>
        /// Appends columns to an existing table.
        /// </summary>
        /// <param name="columnDefinitions">The description of the columns</param>
        /// <exception cref="QdbInvalidArgumentException">If columns list is empty.</exception>
        public void InsertColumns(params QdbColumnDefinition[] columnDefinitions)
        {
            InsertColumns((IEnumerable<QdbColumnDefinition>)columnDefinitions);
        }

        /// <summary>
        /// Appends columns to an existing table.
        /// </summary>
        /// <param name="columnDefinitions">The description of the columns</param>
        /// <exception cref="QdbInvalidArgumentException">If columns list is empty.</exception>
        public void InsertColumns(IEnumerable<QdbColumnDefinition> columnDefinitions)
        {
            var count = Helpers.GetCountOrDefault(columnDefinitions);
            var columns = new InteropableList<qdb_ts_column_info_ex>(count);

            foreach (var def in columnDefinitions)
            {
                columns.Add(new qdb_ts_column_info_ex
                {
                    name = def.Name,
                    type = def.Type,
                    symtable = def.Symtable
                });
            }

            var err = qdb_api.qdb_ts_insert_columns_ex(
                Handle, Alias,
                columns.Buffer, columns.Count);
            QdbExceptionThrower.ThrowIfNeeded(err, alias: Alias);
        }

        /// <summary>
        /// Trim the table, so that it uses approximately the provided size.
        /// </summary>
        /// <param name="size">The desired disk usage size after the operation, in bytes</param>
        /// <exception cref="QdbInvalidArgumentException">If size is negative.</exception>
        public void ExpireBySize(long size)
        {
            if (size < 0)
                throw new QdbInvalidArgumentException();

            var err = qdb_api.qdb_ts_expire_by_size(
                Handle, Alias, (ulong)size);
            QdbExceptionThrower.ThrowIfNeeded(err, alias: Alias);
        }

        #region Reader

        /// <summary>
        /// Initialize a local table for reading from this table.
        /// </summary>
        /// <returns>A <see cref="QdbTableReader"/> for reading from this table</returns>
        /// <seealso cref="QdbTableReader"/>
        public QdbTableReader Reader()
        {
            return Reader(null, QdbTimeInterval.Everything);
        }

        /// <summary>
        /// Initialize a local table for reading from this table.
        /// </summary>
        /// <param name="interval">The time interval to read</param>
        /// <returns>A <see cref="QdbTableReader"/> for reading from this table</returns>
        /// <seealso cref="QdbTableReader"/>
        public QdbTableReader Reader(QdbTimeInterval interval)
        {
            return Reader(null, new[] { interval });
        }

        /// <summary>
        /// Initialize a local table for reading from this table.
        /// </summary>
        /// <param name="intervals">The time intervals to read</param>
        /// <exception cref="QdbInvalidArgumentException">If interval list is empty.</exception>
        /// <returns>A <see cref="QdbTableReader"/> for reading from this table</returns>
        /// <seealso cref="QdbTableReader"/>
        public QdbTableReader Reader(IEnumerable<QdbTimeInterval> intervals)
        {
            return Reader(null, intervals);
        }

        /// <summary>
        /// Initialize a local table for reading from this table.
        /// </summary>
        /// <param name="columnDefinitions">The description of the columns</param>
        /// <returns>A <see cref="QdbTableReader"/> for reading from this table</returns>
        /// <seealso cref="QdbTableReader"/>
        public QdbTableReader Reader(IEnumerable<QdbColumnDefinition> columnDefinitions)
        {
            return Reader(columnDefinitions, QdbTimeInterval.Everything);
        }

        /// <summary>
        /// Initialize a local table for reading from this table.
        /// </summary>
        /// <param name="columnDefinitions">The description of the columns</param>
        /// <param name="interval">The time interval to read</param>
        /// <returns>A <see cref="QdbTableReader"/> for reading from this table</returns>
        /// <seealso cref="QdbTableReader"/>
        public QdbTableReader Reader(IEnumerable<QdbColumnDefinition> columnDefinitions, QdbTimeInterval interval)
        {
            return Reader(columnDefinitions, new[] { interval });
        }

        /// <summary>
        /// Initialize a local table for reading from this table.
        /// </summary>
        /// <param name="columnDefinitions">The description of the columns</param>
        /// <param name="intervals">The time intervals to read</param>
        /// <exception cref="QdbInvalidArgumentException">If interval list is empty.</exception>
        /// <returns>A <see cref="QdbTableReader"/> for reading from this table</returns>
        /// <seealso cref="QdbTableReader"/>
        public QdbTableReader Reader(IEnumerable<QdbColumnDefinition> columnDefinitions, IEnumerable<QdbTimeInterval> intervals)
        {
            InteropableList<qdb_ts_column_info> columns;
            if (columnDefinitions == null)
            {
                var defs = GetColumnDefinitions();
                columns = new InteropableList<qdb_ts_column_info>((int)defs.Count);
                foreach (var def in defs)
                {
                    columns.Add(new qdb_ts_column_info
                    {
                        name = def.name,
                        type = def.type,
                    });
                }
            }
            else
            {
                var count = Helpers.GetCountOrDefault(columnDefinitions);
                columns = new InteropableList<qdb_ts_column_info>(count);
                foreach (var def in columnDefinitions)
                {
                    columns.Add(new qdb_ts_column_info
                    {
                        name = def.Name,
                        type = def.Type,
                    });
                }
            }

            var err = qdb_api.qdb_ts_local_table_init(
                Handle, Alias,
                columns.Buffer, columns.Count,
                out IntPtr table);
            QdbExceptionThrower.ThrowIfNeeded(err, alias: Alias);

            try
            {
                var ranges = new InteropableList<qdb_ts_range>(Helpers.GetCountOrDefault(intervals));
                foreach (var interval in intervals)
                    ranges.Add(interval.ToNative());

                err = qdb_api.qdb_ts_table_get_ranges(
                    table, ranges.Buffer, ranges.Count);
                QdbExceptionThrower.ThrowIfNeeded(err, alias: Alias);
            }
            catch
            {
                qdb_api.qdb_release(Handle, table);
                throw;
            }

            return new QdbTableReader(Handle, Alias, table, columns);
        }

        #endregion

        #region StreamReader

        /// <summary>
        /// Initialize a local table for reading from this table.
        /// </summary>
        /// <returns>A <see cref="QdbTableStreamReader"/> for reading from this table</returns>
        /// <seealso cref="QdbTableStreamReader"/>
        public QdbTableStreamReader StreamReader()
        {
            return StreamReader(null, QdbTimeInterval.Everything);
        }

        /// <summary>
        /// Initialize a local table for reading from this table.
        /// </summary>
        /// <param name="interval">The time interval to read</param>
        /// <returns>A <see cref="QdbTableStreamReader"/> for reading from this table</returns>
        /// <seealso cref="QdbTableStreamReader"/>
        public QdbTableStreamReader StreamReader(QdbTimeInterval interval)
        {
            return StreamReader(null, new[] { interval });
        }

        /// <summary>
        /// Initialize a local table for reading from this table.
        /// </summary>
        /// <param name="intervals">The time intervals to read</param>
        /// <exception cref="QdbInvalidArgumentException">If interval list is empty.</exception>
        /// <returns>A <see cref="QdbTableStreamReader"/> for reading from this table</returns>
        /// <seealso cref="QdbTableStreamReader"/>
        public QdbTableStreamReader StreamReader(IEnumerable<QdbTimeInterval> intervals)
        {
            return StreamReader(null, intervals);
        }

        /// <summary>
        /// Initialize a local table for reading from this table.
        /// </summary>
        /// <param name="columnDefinitions">The description of the columns</param>
        /// <returns>A <see cref="QdbTableStreamReader"/> for reading from this table</returns>
        /// <seealso cref="QdbTableStreamReader"/>
        public QdbTableStreamReader StreamReader(IEnumerable<QdbColumnDefinition> columnDefinitions)
        {
            return StreamReader(columnDefinitions, QdbTimeInterval.Everything);
        }

        /// <summary>
        /// Initialize a local table for reading from this table.
        /// </summary>
        /// <param name="columnDefinitions">The description of the columns</param>
        /// <param name="interval">The time interval to read</param>
        /// <returns>A <see cref="QdbTableStreamReader"/> for reading from this table</returns>
        /// <seealso cref="QdbTableStreamReader"/>
        public QdbTableStreamReader StreamReader(IEnumerable<QdbColumnDefinition> columnDefinitions, QdbTimeInterval interval)
        {
            return StreamReader(columnDefinitions, new[] { interval });
        }

        /// <summary>
        /// Initialize a local table for reading from this table.
        /// </summary>
        /// <param name="columnDefinitions">The description of the columns</param>
        /// <param name="intervals">The time intervals to read</param>
        /// <exception cref="QdbInvalidArgumentException">If interval list is empty.</exception>
        /// <returns>A <see cref="QdbTableStreamReader"/> for reading from this table</returns>
        /// <seealso cref="QdbTableStreamReader"/>
        public QdbTableStreamReader StreamReader(IEnumerable<QdbColumnDefinition> columnDefinitions, IEnumerable<QdbTimeInterval> intervals)
        {
            InteropableList<qdb_ts_column_info> columns;
            if (columnDefinitions == null)
            {
                var defs = GetColumnDefinitions();
                columns = new InteropableList<qdb_ts_column_info>((int)defs.Count);
                foreach (var def in defs)
                {
                    columns.Add(new qdb_ts_column_info
                    {
                        name = def.name,
                        type = def.type,
                    });
                }
            }
            else
            {
                var count = Helpers.GetCountOrDefault(columnDefinitions);
                columns = new InteropableList<qdb_ts_column_info>(count);
                foreach (var def in columnDefinitions)
                {
                    columns.Add(new qdb_ts_column_info
                    {
                        name = def.Name,
                        type = def.Type,
                    });
                }
            }

            var err = qdb_api.qdb_ts_local_table_init(
                Handle, Alias,
                columns.Buffer, columns.Count,
                out IntPtr table);
            QdbExceptionThrower.ThrowIfNeeded(err, alias: Alias);

            try
            {
                var ranges = new InteropableList<qdb_ts_range>(Helpers.GetCountOrDefault(intervals));
                foreach (var interval in intervals)
                    ranges.Add(interval.ToNative());

                err = qdb_api.qdb_ts_table_stream_ranges(
                    table, ranges.Buffer, ranges.Count);
                QdbExceptionThrower.ThrowIfNeeded(err, alias: Alias);
            }
            catch
            {
                qdb_api.qdb_release(Handle, table);
                throw;
            }

            return new QdbTableStreamReader(Handle, Alias, table, columns);
        }

        #endregion

    }
}
