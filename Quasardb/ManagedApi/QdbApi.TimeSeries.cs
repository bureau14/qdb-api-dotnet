using Quasardb.Native;
using Quasardb.TimeSeries;

namespace Quasardb.ManagedApi
{
    partial class QdbApi
    {
        private static string MakeTsAlias(string series, string column)
        {
            return series + "." + column;
        }

        public void TsDoubleInsert(string alias, string column, InteropableList<qdb_ts_double_point> points)
        {
            var error = qdb_api.qdb_ts_double_insert(_handle, MakeTsAlias(alias, column), points.Buffer, points.Count);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: alias);
        }

        public void TimeSeriesAggregate(string alias, string column, qdb_ts_aggregation_type aggregationType, InteropableList<qdb_ts_aggregation> aggregations)
        {
            var error = qdb_api.qdb_ts_aggregate(_handle, MakeTsAlias(alias, column), aggregationType, aggregations.Buffer, aggregations.Count);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: alias);
        }

        public unsafe QdbDoublePointResponse TsDoubleGetPoints(string alias, string column, InteropableList<qdb_ts_range> ranges)
        {
            var points = new QdbDoublePointResponse(_handle);
            var error = qdb_api.qdb_ts_double_get_range(_handle, MakeTsAlias(alias, column), ranges.Buffer, ranges.Count, out points.Pointer, out points.Size);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: alias);
            return points;
        }

        public void TsBlobInsert(string alias, string column, InteropableList<qdb_ts_blob_point> points)
        {
            var error = qdb_api.qdb_ts_blob_insert(_handle, MakeTsAlias(alias, column), points.Buffer, points.Count);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: alias);
        }

        public unsafe QdbBlobPointResponse TsBlobGetPoints(string alias, string column, InteropableList<qdb_ts_range> ranges)
        {
            var points = new QdbBlobPointResponse(_handle);
            var error = qdb_api.qdb_ts_blob_get_range(_handle, MakeTsAlias(alias, column), ranges.Buffer, ranges.Count, out points.Pointer, out points.Size);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: alias);
            return points;
        }
    }
}
