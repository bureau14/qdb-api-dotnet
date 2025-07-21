using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Quasardb.Exceptions;
using Quasardb.Native;

using qdb_bulk_reader_table_data = Quasardb.Native.qdb_exp_batch_push_table_data;

namespace Quasardb.TimeSeries.Reader
{
    public unsafe class QdbBulkRow : IEnumerable<QdbBulkCell>
    {
        readonly qdb_bulk_reader_table_data* _data;
        readonly string[] _columnNames;
        readonly qdb_exp_batch_push_column* _columns;

        internal QdbBulkRow(qdb_bulk_reader_table_data* data)
        {
            _data = data;
            _columns = data->columns;
            _columnNames = new string[(long)data->column_count];
            for (int i = 0; i < _columnNames.Length; i++)
            {
                _columnNames[i] = Marshal.PtrToStringAnsi(_columns[i].name);
            }
        }

        internal long RowIndex { get; set; }

        public DateTime Timestamp
        {
            get
            {
                qdb_timespec ts = _data->timestamps[RowIndex];
                return TimeConverter.ToDateTime(ts);
            }
        }

        public long Count => _columnNames.LongLength;

        public QdbBulkCell this[long index]
        {
            get
            {
                if (index < 0 || index >= _columnNames.LongLength)
                    throw new ArgumentOutOfRangeException();
                return new QdbBulkCell(&_columns[index], RowIndex);
            }
        }

        public QdbBulkCell this[string name]
        {
            get
            {
                long idx = IndexOf(name);
                if (idx != -1) return this[idx];
                throw new QdbColumnNotFoundException(null, name);
            }
        }

        internal long IndexOf(string column)
        {
            for (int i = 0; i < _columnNames.Length; i++)
                if (_columnNames[i] == column)
                    return i;
            return -1;
        }

        public IEnumerator<QdbBulkCell> GetEnumerator()
        {
            for (var i = 0L; i < _columnNames.LongLength; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
