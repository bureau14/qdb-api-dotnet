﻿using System;
using System.Collections;
using System.Collections.Generic;
using Quasardb.Exceptions;
using Quasardb.Native;

namespace Quasardb.TimeSeries
{
    /// <summary>
    /// A collection of columns
    /// </summary>
    public class QdbColumnCollection : IEnumerable<QdbColumn>
    {
        internal readonly QdbTimeSeries _series;

        internal QdbColumnCollection(QdbTimeSeries series)
        {
            _series = series;
        }

        /// <inheritdoc />
        public IEnumerator<QdbColumn> GetEnumerator()
        {
            var handle = _series.Handle;
            var alias = _series.Alias;
            var names = new QdbStringCollection(handle);
            var types = new QdbColumnTypeCollection(handle);

            var err = qdb_api.qdb_ts_list_columns(handle, alias, out names.Pointer, out types.Pointer,
                out names.Size);
            types.Size = names.Size;
            QdbExceptionThrower.ThrowIfNeeded(err, alias: alias);

            for (ulong i = 0; i < (ulong)types.Size; i++)
            {
                yield return MakeColumn(types[i], names[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        QdbColumn MakeColumn(qdb_ts_column_type type, string name)
        {
            switch (type)
            {
                case qdb_ts_column_type.qdb_ts_column_double:
                    return new QdbDoubleColumn(_series, name);
                case qdb_ts_column_type.qdb_ts_column_blob:
                    return new QdbBlobColumn(_series, name);
                default:
                    return new QdbUnknownColumn(_series, name, type);
            }
        }
    }

    /// <summary>
    /// A collection of columns contains double-precision floating point values.
    /// </summary>
    public class QdbDoubleColumnCollection : IEnumerable<QdbDoubleColumn>
    {
        internal readonly QdbTimeSeries _series;

        internal QdbDoubleColumnCollection(QdbTimeSeries series)
        {
            _series = series;
        }

        /// <summary>
        /// Gets the columns with the specified name
        /// </summary>
        /// <param name="name">The name of the column</param>
        public QdbDoubleColumn this[string name] => new QdbDoubleColumn(_series, name);

        /// <inheritdoc />
        public IEnumerator<QdbDoubleColumn> GetEnumerator()
        {
            foreach (var col in new QdbColumnCollection(_series))
            {
                if (col is QdbDoubleColumn doubleColumn)
                    yield return doubleColumn;
            }
        }

       IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// A collection of columns of blobs
    /// </summary>
    public class QdbBlobColumnCollection : IEnumerable<QdbBlobColumn>
    {
        readonly QdbTimeSeries _series;

        internal QdbBlobColumnCollection(QdbTimeSeries series)
        {
            _series = series;
        }

        /// <summary>
        /// Gets the columns with the specified name
        /// </summary>
        /// <param name="name">The name of the column</param>
        public QdbBlobColumn this[string name] => new QdbBlobColumn(_series, name);

        /// <inheritdoc />
        public IEnumerator<QdbBlobColumn> GetEnumerator()
        {
            foreach (var col in new QdbColumnCollection(_series))
            {
                if (col is QdbBlobColumn blobColumn)
                    yield return blobColumn;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// A time series
    /// </summary>
    public class QdbTimeSeries : QdbEntry
    {
        internal QdbTimeSeries(qdb_handle handle, string alias) : base(handle, alias)
        {
            DoubleColumns = new QdbDoubleColumnCollection(this);
            Columns = new QdbColumnCollection(this);
            BlobColumns = new QdbBlobColumnCollection(this);
        }

        /// <summary>
        /// The columns of the time series that contains blobs.
        /// </summary>
        public QdbBlobColumnCollection BlobColumns { get; }

        /// <summary>
        /// The columns of the time series.
        /// </summary>
        public QdbColumnCollection Columns { get; }

        /// <summary>
        /// The columns of the time series that contains double-precision floating-point values.
        /// </summary>
        public QdbDoubleColumnCollection DoubleColumns { get; }

        /// <summary>
        /// Creates the time-series.
        /// </summary>
        /// <param name="columnDefinitions">The description of the columns</param>
        /// <exception cref="QdbAliasAlreadyExistsException">If the time-series already exists.</exception>
        /// <exception cref="QdbIncompatibleTypeException">If the alias matches with an entry of another type.</exception>
        /// <exception cref="QdbInvalidArgumentException">If columns list is empty.</exception>
        public void Create(params QdbColumnDefinition[] columnDefinitions)
        {
            var count = columnDefinitions.Length;
            var names = new string[count];
            var types = new qdb_ts_column_type[count];

            for (var i = 0; i < count; i++)
            {
                names[i] = columnDefinitions[i].Name;
                types[i] = columnDefinitions[i].Type;
            }

            var err = qdb_api.qdb_ts_create(Handle, Alias, names, types, (UIntPtr) count);
            QdbExceptionThrower.ThrowIfNeeded(err, alias: Alias);
        }
    }
}
