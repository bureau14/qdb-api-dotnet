using System.Collections.Generic;
using Quasardb.Exceptions;
using Quasardb.Native;

namespace Quasardb.TimeSeries
{
    class QdbColumnAggregator
    {
        public struct SingleBlobResult
        {
            readonly qdb_ts_blob_aggregation _aggregation;

            public SingleBlobResult(qdb_ts_blob_aggregation aggregation)
            {
                _aggregation = aggregation;
            }

            public long ToLong()
            {
                return (long) _aggregation.count;
            }

            public QdbBlobPoint ToBlobPoint()
            {
                return ((int)_aggregation.result.content_size == 0)
                           ? null
                           : _aggregation.result.ToManaged();
            }
        }

        public struct SingleDoubleResult
        {
            readonly qdb_ts_double_aggregation _aggregation;

            public SingleDoubleResult(qdb_ts_double_aggregation aggregation)
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

        public struct MultipleBlobResults
        {
            readonly IEnumerable<qdb_ts_blob_aggregation> _aggregations;

            public MultipleBlobResults(IEnumerable<qdb_ts_blob_aggregation> aggregations)
            {
                _aggregations = aggregations;
            }

            public IEnumerable<long> ToLong()
            {
                foreach (var agg in _aggregations)
                    yield return (long)agg.count;
            }

            public IEnumerable<QdbBlobPoint> ToBlobPoint()
            {
                foreach (var agg in _aggregations)
                    yield return ((int)agg.result.content_size == 0)
                        ? null
                        : agg.result.ToManaged();
            }
        }

        public struct MultipleDoubleResults
        {
            readonly IEnumerable<qdb_ts_double_aggregation> _aggregations;

            public MultipleDoubleResults(IEnumerable<qdb_ts_double_aggregation> aggregations)
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

        public SingleBlobResult BlobAggregate(qdb_ts_aggregation_type mode)
        {
            return BlobAggregate(mode, QdbTimeInterval.Everything);
        }

        public SingleBlobResult BlobAggregate(qdb_ts_aggregation_type mode, QdbTimeInterval interval)
        {
            var aggregations = new InteropableList<qdb_ts_blob_aggregation>(1);
            aggregations.Add(MakeBlobAggregation(mode, interval));
            var error = qdb_api.qdb_ts_blob_aggregate(_column.Handle, _column.Series.Alias, _column.Name, aggregations.Buffer, aggregations.Count);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: _column.Series.Alias, column: _column.Name);
            return new SingleBlobResult(aggregations[0]);
        }

        public MultipleBlobResults BlobAggregate(qdb_ts_aggregation_type mode, IEnumerable<QdbTimeInterval> intervals)
        {
            var aggregations = new InteropableList<qdb_ts_blob_aggregation>(Helpers.GetCountOrDefault(intervals));
            foreach (var interval in intervals)
                aggregations.Add(MakeBlobAggregation(mode, interval));

            var error = qdb_api.qdb_ts_blob_aggregate(_column.Handle, _column.Series.Alias, _column.Name, aggregations.Buffer, aggregations.Count);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: _column.Series.Alias);

            return new MultipleBlobResults(aggregations);
        }

        public SingleDoubleResult DoubleAggregate(qdb_ts_aggregation_type mode)
        {
            return DoubleAggregate(mode, QdbTimeInterval.Everything);
        }

        public SingleDoubleResult DoubleAggregate(qdb_ts_aggregation_type mode, QdbTimeInterval interval)
        {
            var aggregations = new InteropableList<qdb_ts_double_aggregation>(1);
            aggregations.Add(MakeDoubleAggregation(mode, interval));
            var error = qdb_api.qdb_ts_double_aggregate(_column.Handle, _column.Series.Alias, _column.Name, aggregations.Buffer, aggregations.Count);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: _column.Series.Alias, column: _column.Name);
            return new SingleDoubleResult(aggregations[0]);
        }

        public MultipleDoubleResults DoubleAggregate(qdb_ts_aggregation_type mode, IEnumerable<QdbTimeInterval> intervals)
        {
            var aggregations = new InteropableList<qdb_ts_double_aggregation>(Helpers.GetCountOrDefault(intervals));
            foreach (var interval in intervals)
                aggregations.Add(MakeDoubleAggregation(mode, interval));

            var error = qdb_api.qdb_ts_double_aggregate(_column.Handle, _column.Series.Alias, _column.Name, aggregations.Buffer, aggregations.Count);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: _column.Series.Alias);

            return new MultipleDoubleResults(aggregations);
        }

        static qdb_ts_blob_aggregation MakeBlobAggregation(qdb_ts_aggregation_type mode, QdbTimeInterval interval)
        {
          return new qdb_ts_blob_aggregation{type = mode,
                                            range = interval.ToNative()};
        }

        static qdb_ts_double_aggregation MakeDoubleAggregation(qdb_ts_aggregation_type mode, QdbTimeInterval interval)
        {
          return new qdb_ts_double_aggregation{type = mode,
                                               range = interval.ToNative()};
        }
    }
}