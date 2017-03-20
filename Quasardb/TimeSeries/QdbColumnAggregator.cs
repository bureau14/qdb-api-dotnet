using System;
using System.Collections.Generic;
using Quasardb.ManagedApi;
using Quasardb.Native;

namespace Quasardb.TimeSeries
{
    class QdbColumnAggregator
    {
        public struct SingleResult
        {
            readonly qdb_ts_aggregation _aggregation;

            public SingleResult(qdb_ts_aggregation aggregation)
            {
                _aggregation = aggregation;
            }

            public long ToLong()
            {
                return double.IsNaN(_aggregation.result.value) ? 0 : (long) _aggregation.result.value;
            }

            public double ToDouble()
            {
                return _aggregation.result.value;
            }

            public QdbDoublePoint ToDoublePoint()
            {
                return double.IsNaN(_aggregation.result.value) ? null : _aggregation.result.ToManaged();
            }
        }

        public struct MultipleResults
        {
            readonly IEnumerable<qdb_ts_aggregation> _aggregations;
  
            public MultipleResults(IEnumerable<qdb_ts_aggregation> aggregations)
            {
                _aggregations = aggregations;
            }

            public IEnumerable<long> ToLong()
            {
                foreach (var agg in _aggregations)
                    yield return double.IsNaN(agg.result.value) ? 0 : (long)agg.result.value;
            }

            public IEnumerable<double> ToDouble()
            {
                foreach (var agg in _aggregations)
                    yield return agg.result.value;
            }

            public IEnumerable<QdbDoublePoint> ToDoublePoint()
            {
                foreach (var agg in _aggregations)
                    yield return double.IsNaN(agg.result.value) ? null : agg.result.ToManaged();
            }
        }

        readonly QdbColumn _column;

        public QdbColumnAggregator(QdbColumn column)
        {
            _column = column;
        }

        public SingleResult Aggregate(QdbTimeInterval interval, qdb_ts_aggregation_type mode)
        {
            var aggregations = new InteropableList<qdb_ts_aggregation>(1);
            aggregations.Add(MakeAggregation(interval));
            _column.Series.Api.TimeSeriesAggregate(_column.Series.Alias, _column.Name, mode, aggregations);
            return new SingleResult(aggregations[0]);
        }

        public MultipleResults Aggregate(IEnumerable<QdbTimeInterval> intervals, qdb_ts_aggregation_type mode)
        {
            var aggregations = new InteropableList<qdb_ts_aggregation>(Helpers.GetCountOrDefault(intervals));
            foreach (var interval in intervals)
                aggregations.Add(MakeAggregation(interval));

            _column.Series.Api.TimeSeriesAggregate(_column.Series.Alias, _column.Name, mode, aggregations);

            return new MultipleResults(aggregations);
        }

        static qdb_ts_aggregation MakeAggregation(QdbTimeInterval interval)
        {
            return new qdb_ts_aggregation { range = interval.ToNative() };
        }
    }
}