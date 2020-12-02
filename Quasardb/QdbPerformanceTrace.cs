using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Quasardb.Exceptions;
using Quasardb.Native;

// ReSharper disable InconsistentNaming

namespace Quasardb
{
    /// A measure of the time taken by a single operation.
    public struct QdbPerformanceMeasure {
            /// The name of the measured operation.
            public string label;
            /// The number of nanoseconds elapsed since the first measurement.
            public long elapsed;
    }

    /// A collection of continuous performance measurements.
    public sealed class QdbPerformanceTrace {
        /// The trace name.
        public string name;
        /// The measurements.
        public QdbPerformanceMeasure[] measures;

        internal unsafe QdbPerformanceTrace(qdb_perf_profile profile) {
            name = profile.name.ToString();
            
            measures = new QdbPerformanceMeasure[(int)profile.count];
            for (var i = 0; i < (int)profile.count; ++i)
            {
                var p = new IntPtr(profile.measurements + i);
                var m = (qdb_perf_measurement)Marshal.PtrToStructure(p, typeof(qdb_perf_measurement));
                measures[i].label = m.label.ToString();
                measures[i].elapsed = m.elapsed;
            }
        }

        static internal unsafe QdbPerformanceTrace[] CreateTraces(qdb_perf_profile* profiles, int count)
        {
            var traces = new QdbPerformanceTrace[count];
            for (var i = 0; i < count; ++i)
            {
                var p = new IntPtr(profiles + i);
                var t = (qdb_perf_profile)Marshal.PtrToStructure(p, typeof(qdb_perf_profile));
                traces[i] = new QdbPerformanceTrace(t);
            }
            return traces;
        }
    }
}
