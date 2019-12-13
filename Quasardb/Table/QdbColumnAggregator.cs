using System;
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

            public long Count()
            {
                return (long)_aggregation.count;
            }

            public QdbBlobPoint AsPoint()
            {
                return (long)_aggregation.count == 0
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

            public long Count()
            {
                return (long)_aggregation.count;
            }

            public double? Value()
            {
                if (double.IsNaN(_aggregation.result.value))
                    return null;
                return _aggregation.result.value;
            }

            public QdbDoublePoint AsPoint()
            {
                return (long)_aggregation.count == 0 ? null : _aggregation.result.ToManaged();
            }
        }

        public struct SingleInt64Result
        {
            readonly qdb_ts_int64_aggregation _aggregation;

            public SingleInt64Result(qdb_ts_int64_aggregation aggregation)
            {
                _aggregation = aggregation;
            }

            public long Count()
            {
                return (long)_aggregation.count;
            }

            public long? Value()
            {
                if (_aggregation.result.value == long.MinValue)
                    return null;
                return _aggregation.result.value;
            }

            public double? ExactResult()
            {
                if (double.IsNaN(_aggregation.exact_result))
                    return null;
                return _aggregation.exact_result;
            }

            public QdbInt64Point AsPoint()
            {
                return (long)_aggregation.count == 0 ? null : _aggregation.result.ToManaged();
            }
        }

        public struct SingleTimestampResult
        {
            readonly qdb_ts_timestamp_aggregation _aggregation;

            public SingleTimestampResult(qdb_ts_timestamp_aggregation aggregation)
            {
                _aggregation = aggregation;
            }

            public long Count()
            {
                return (long)_aggregation.count;
            }

            public DateTime? Value()
            {
                if (TimeConverter.IsNull(_aggregation.result.value))
                    return null;
                return TimeConverter.ToDateTime(_aggregation.result.value);
            }

            public QdbTimestampPoint AsPoint()
            {
                return (long)_aggregation.count == 0 ? null : _aggregation.result.ToManaged();
            }
        }

        public struct MultipleBlobResults
        {
            readonly IEnumerable<qdb_ts_blob_aggregation> _aggregations;

            public MultipleBlobResults(IEnumerable<qdb_ts_blob_aggregation> aggregations)
            {
                _aggregations = aggregations;
            }

            public IEnumerable<long> Count()
            {
                foreach (var agg in _aggregations)
                    yield return (long)agg.count;
            }

            public IEnumerable<QdbBlobPoint> AsPoint()
            {
                foreach (var agg in _aggregations)
                    yield return (long)agg.count == 0
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

            public IEnumerable<long> Count()
            {
                foreach (var agg in _aggregations)
                    yield return (long)agg.count;
            }

            public IEnumerable<double?> Value()
            {
                foreach (var agg in _aggregations)
                {
                    if (double.IsNaN(agg.result.value))
                        yield return null;
                    else
                        yield return agg.result.value;
                }
            }

            public IEnumerable<QdbDoublePoint> AsPoint()
            {
                foreach (var agg in _aggregations)
                    yield return (long)agg.count == 0
                        ? null
                        : agg.result.ToManaged();
            }
        }

        public struct MultipleInt64Results
        {
            readonly IEnumerable<qdb_ts_int64_aggregation> _aggregations;

            public MultipleInt64Results(IEnumerable<qdb_ts_int64_aggregation> aggregations)
            {
                _aggregations = aggregations;
            }

            public IEnumerable<long> Count()
            {
                foreach (var agg in _aggregations)
                    yield return (long)agg.count;
            }

            public IEnumerable<long?> Value()
            {
                foreach (var agg in _aggregations)
                {
                    if (agg.result.value == long.MinValue)
                        yield return null;
                    else
                        yield return agg.result.value;
                }
            }

            public IEnumerable<double?> ExactResult()
            {
                foreach (var agg in _aggregations)
                {
                    if (double.IsNaN(agg.exact_result))
                        yield return null;
                    else
                        yield return agg.exact_result;
                }
            }

            public IEnumerable<QdbInt64Point> AsPoint()
            {
                foreach (var agg in _aggregations)
                    yield return (long)agg.count == 0
                        ? null
                        : agg.result.ToManaged();
            }
        }

        public struct MultipleTimestampResults
        {
            readonly IEnumerable<qdb_ts_timestamp_aggregation> _aggregations;

            public MultipleTimestampResults(IEnumerable<qdb_ts_timestamp_aggregation> aggregations)
            {
                _aggregations = aggregations;
            }

            public IEnumerable<long> Count()
            {
                foreach (var agg in _aggregations)
                    yield return (long)agg.count;
            }

            public IEnumerable<DateTime?> Value()
            {
                foreach (var agg in _aggregations)
                {
                    if (TimeConverter.IsNull(agg.result.value))
                        yield return null;
                    else
                        yield return TimeConverter.ToDateTime(agg.result.value);
                }
            }

            public IEnumerable<QdbTimestampPoint> AsPoint()
            {
                foreach (var agg in _aggregations)
                    yield return (long)agg.count == 0
                        ? null
                        : agg.result.ToManaged();
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
            var aggregations = new InteropableList<qdb_ts_blob_aggregation>(1) { MakeBlobAggregation(mode, interval) };
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
            var aggregations = new InteropableList<qdb_ts_double_aggregation>(1) { MakeDoubleAggregation(mode, interval) };
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

        public SingleInt64Result Int64Aggregate(qdb_ts_aggregation_type mode)
        {
            return Int64Aggregate(mode, QdbTimeInterval.Everything);
        }

        public SingleInt64Result Int64Aggregate(qdb_ts_aggregation_type mode, QdbTimeInterval interval)
        {
            var aggregations = new InteropableList<qdb_ts_int64_aggregation>(1) { MakeInt64Aggregation(mode, interval) };
            var error = qdb_api.qdb_ts_int64_aggregate(_column.Handle, _column.Series.Alias, _column.Name, aggregations.Buffer, aggregations.Count);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: _column.Series.Alias, column: _column.Name);
            return new SingleInt64Result(aggregations[0]);
        }

        public MultipleInt64Results Int64Aggregate(qdb_ts_aggregation_type mode, IEnumerable<QdbTimeInterval> intervals)
        {
            var aggregations = new InteropableList<qdb_ts_int64_aggregation>(Helpers.GetCountOrDefault(intervals));
            foreach (var interval in intervals)
                aggregations.Add(MakeInt64Aggregation(mode, interval));

            var error = qdb_api.qdb_ts_int64_aggregate(_column.Handle, _column.Series.Alias, _column.Name, aggregations.Buffer, aggregations.Count);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: _column.Series.Alias);

            return new MultipleInt64Results(aggregations);
        }

        public SingleTimestampResult TimestampAggregate(qdb_ts_aggregation_type mode)
        {
            return TimestampAggregate(mode, QdbTimeInterval.Everything);
        }

        public SingleTimestampResult TimestampAggregate(qdb_ts_aggregation_type mode, QdbTimeInterval interval)
        {
            var aggregations = new InteropableList<qdb_ts_timestamp_aggregation>(1) { MakeTimestampAggregation(mode, interval) };
            var error = qdb_api.qdb_ts_timestamp_aggregate(_column.Handle, _column.Series.Alias, _column.Name, aggregations.Buffer, aggregations.Count);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: _column.Series.Alias, column: _column.Name);
            return new SingleTimestampResult(aggregations[0]);
        }

        public MultipleTimestampResults TimestampAggregate(qdb_ts_aggregation_type mode, IEnumerable<QdbTimeInterval> intervals)
        {
            var aggregations = new InteropableList<qdb_ts_timestamp_aggregation>(Helpers.GetCountOrDefault(intervals));
            foreach (var interval in intervals)
                aggregations.Add(MakeTimestampAggregation(mode, interval));

            var error = qdb_api.qdb_ts_timestamp_aggregate(_column.Handle, _column.Series.Alias, _column.Name, aggregations.Buffer, aggregations.Count);
            QdbExceptionThrower.ThrowIfNeeded(error, alias: _column.Series.Alias);

            return new MultipleTimestampResults(aggregations);
        }

        static qdb_ts_blob_aggregation MakeBlobAggregation(qdb_ts_aggregation_type mode, QdbTimeInterval interval)
        {
            return new qdb_ts_blob_aggregation
            {
                type = mode,
                range = interval.ToNative()
            };
        }

        static qdb_ts_double_aggregation MakeDoubleAggregation(qdb_ts_aggregation_type mode, QdbTimeInterval interval)
        {
            return new qdb_ts_double_aggregation
            {
                type = mode,
                range = interval.ToNative()
            };
        }

        static qdb_ts_int64_aggregation MakeInt64Aggregation(qdb_ts_aggregation_type mode, QdbTimeInterval interval)
        {
            return new qdb_ts_int64_aggregation
            {
                type = mode,
                range = interval.ToNative()
            };
        }

        static qdb_ts_timestamp_aggregation MakeTimestampAggregation(qdb_ts_aggregation_type mode, QdbTimeInterval interval)
        {
            return new qdb_ts_timestamp_aggregation
            {
                type = mode,
                range = interval.ToNative()
            };
        }
    }
}