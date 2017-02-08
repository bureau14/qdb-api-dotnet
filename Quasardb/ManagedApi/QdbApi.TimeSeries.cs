using System;
using Quasardb.Exceptions;
using Quasardb.NativeApi;

namespace Quasardb.ManagedApi
{
    partial class QdbApi
    {
        public void TimeSeriesInsert(string alias, qdb_ts_double_point[] points)
        {
            var error = qdb_api.qdb_ts_insert(_handle, alias, points, (UIntPtr) points.Length);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: alias);
        }

        public qdb_ts_double_point TimeSeriesAggregate(string alias, qdb_timespec begin, qdb_timespec end, qdb_ts_aggregation aggregation)
        {
            qdb_ts_double_point result;
            var error = qdb_api.qdb_ts_aggregate(_handle, alias, ref begin, ref end, aggregation, out result);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: alias);
            return result;
        }
    }
}
