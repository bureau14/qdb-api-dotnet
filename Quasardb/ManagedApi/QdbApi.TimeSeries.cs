using System;
using Quasardb.NativeApi;

namespace Quasardb.ManagedApi
{
    partial class QdbApi
    {
        public void TimeSeriesInsert(string alias, qdb_ts_double_point[] points)
        {
            var error = qdb_api.qdb_ts_double_insert(_handle, alias, points, (UIntPtr) points.Length);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: alias);
        }

        public void TimeSeriesAggregate(string alias, qdb_ts_aggregation_type aggregationType, InteropableList<qdb_ts_aggregation> aggregations)
        {
            var error = qdb_api.qdb_ts_aggregate(_handle, alias, aggregationType, aggregations.Buffer, aggregations.Count);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: alias);
        }

        public unsafe QdbDoublePointCollection TimeSeriesGetPoints(string alias, InteropableList<qdb_ts_range> ranges)
        {
            var points = new QdbDoublePointCollection(_handle);
            var error = qdb_api.qdb_ts_double_get_range(_handle, alias, ranges.Buffer, ranges.Count, out points.Pointer, out points.Size);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: alias);
            return points;
        }
    }
}
