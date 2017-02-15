using System;
using System.Collections;
using System.Collections.Generic;
using Quasardb.NativeApi;

namespace Quasardb.ManagedApi
{
    partial class QdbApi
    {
        public class AggregationCollection : IEnumerable<qdb_ts_aggregation>
        {
            qdb_ts_aggregation[] _buffer;
            int _count;

            public AggregationCollection(int initialCapacity = 128)
            {
                _buffer = new qdb_ts_aggregation[initialCapacity];
                _count = 0;
            }

            public void Add(qdb_timespec begin, qdb_timespec end)
            {
                if (_count >= _buffer.Length)
                    Array.Resize(ref _buffer, _buffer.Length * 2);

                _buffer[_count].begin = begin;
                _buffer[_count].end = end;
                _count++;
            }

            public IEnumerator<qdb_ts_aggregation> GetEnumerator()
            {
                for (var i = 0; i < _count; i++)
                    yield return _buffer[i];
            }

            public qdb_ts_aggregation this[int index] => _buffer[index];
            public qdb_ts_aggregation[] Buffer => _buffer;
            public UIntPtr Count => (UIntPtr) _count;
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public void TimeSeriesInsert(string alias, qdb_ts_double_point[] points)
        {
            var error = qdb_api.qdb_ts_insert(_handle, alias, points, (UIntPtr) points.Length);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: alias);
        }

        public void TimeSeriesAggregate(string alias, qdb_ts_aggregation_type aggregationType, AggregationCollection aggregations)
        {
            var error = qdb_api.qdb_ts_aggregate(_handle, alias, aggregationType, aggregations.Buffer, aggregations.Count);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: alias);
        }
    }
}
