using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Quasardb.Exceptions;
using Quasardb.Native;

using qdb_bulk_reader_table_data = Quasardb.Native.qdb_exp_batch_push_table_data;

namespace Quasardb.TimeSeries.Reader
{
    /// <summary>
    /// Represents a single row in a bulk-read operation from a QuasarDB time series table.
    /// </summary>
    public unsafe struct QdbBulkRow : IEnumerable<QdbBulkCell>
    {
        readonly qdb_bulk_reader_table_data* _data;
        readonly string[] _columnNames;
        readonly qdb_exp_batch_push_column* _columns;

        /// <summary>
        /// Initializes a new instance of the <see cref="QdbBulkRow"/> struct.
        /// </summary>
        /// <param name="data">Pointer to native bulk table data structure.</param>
        internal QdbBulkRow(IntPtr data)
        {
            var p = (qdb_bulk_reader_table_data*)data;
            _data = p;
            _columns = p->columns;
            _columnNames = new string[(long)p->column_count];
            for (int i = 0; i < _columnNames.Length; i++)
            {
                _columnNames[i] = Marshal.PtrToStringAnsi(_columns[i].name);
            }
        }

        /// <summary>
        /// Gets or sets the index of the current row.
        /// </summary>
        internal long RowIndex { get; set; }

        /// <summary>
        /// Gets the timestamp associated with this row.
        /// </summary>
        public DateTime Timestamp
        {
            get
            {
                qdb_timespec ts = _data->timestamps[RowIndex];
                return TimeConverter.ToDateTime(ts);
            }
        }

        /// <summary>
        /// Gets the number of columns in this row.
        /// </summary>
        public long Count => _columnNames.LongLength;

        /// <summary>
        /// Gets the cell at the specified column index.
        /// </summary>
        /// <param name="index">The column index.</param>
        /// <returns>The cell at the specified column index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if index is out of bounds.</exception>
        public QdbBulkCell this[long index]
        {
            get
            {
                if (index < 0 || index >= _columnNames.LongLength)
                    throw new ArgumentOutOfRangeException();
                return new QdbBulkCell(&_columns[index], RowIndex);
            }
        }

        /// <summary>
        /// Gets the cell by column name.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The cell in the column with the specified name.</returns>
        /// <exception cref="QdbColumnNotFoundException">Thrown if column with given name is not found.</exception>
        public QdbBulkCell this[string name]
        {
            get
            {
                long idx = IndexOf(name);
                if (idx != -1) return this[idx];
                throw new QdbColumnNotFoundException(null, name);
            }
        }

        /// <summary>
        /// Returns the index of the specified column name.
        /// </summary>
        /// <param name="column">The name of the column.</param>
        /// <returns>The index of the column if found, otherwise -1.</returns>
        internal long IndexOf(string column)
        {
            for (int i = 0; i < _columnNames.Length; i++)
                if (_columnNames[i] == column)
                    return i;
            return -1;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the cells in the row.
        /// </summary>
        /// <returns>An enumerator for the cells in the row.</returns>
        public IEnumerator<QdbBulkCell> GetEnumerator()
        {
            for (var i = 0L; i < _columnNames.LongLength; i++)
                yield return this[i];
        }

        /// <summary>
        /// Returns an enumerator that iterates through the cells in the row (non-generic).
        /// </summary>
        /// <returns>An enumerator object.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
