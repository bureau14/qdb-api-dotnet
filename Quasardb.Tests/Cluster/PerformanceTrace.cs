using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Quasardb.Tests.Cluster
{
    [TestClass]
    public class PerformanceTrace
    {
        public TestContext TestContext { get; set; }

        private readonly QdbCluster _cluster = QdbTestCluster.Instance;
        private readonly string[] _to_check = {
                    "received",
                    "deserialization_starts",
                    "deserialization_ends",
                    "entering_chord",
                    "dispatch",
                    "deserialization_starts",
                    "deserialization_ends",
                    "processing_starts",
                    "entry_trimming_starts",
                    "entry_trimming_ends",
                    "content_writing_starts",
                    "content_writing_ends",
                    "content_reading_starts",
                    "content_reading_ends",
                    "entry_writing_starts",
                    "entry_writing_ends",
                    "serialization_starts",
                    "serialization_ends",
                    "processing_ends"
                    };

        [TestMethod]
        public void DisabledByDefault()
        {
            var traces = _cluster.GetPerformanceTraces();
            Assert.AreEqual(0, traces.Length);
        }

        [TestMethod]
        public void ReturnNoLabelsWhenEnabled()
        {
            _cluster.EnablePerformanceTraces();
            try
            {
                var traces = _cluster.GetPerformanceTraces();
                Assert.AreEqual(0, traces.Length);
            }
            finally
            {
                _cluster.DisablePerformanceTraces();
            }
        }

        [TestMethod]
        public void ReturnWriteEntryLabels()
        {
            var key = _cluster.Integer("quarante-deux");

            _cluster.EnablePerformanceTraces();
            key.Put(42);
            try
            {
                var traces = _cluster.GetPerformanceTraces();
                Assert.AreEqual(1, traces.Length);

                var trace = traces[0];
                Assert.AreEqual("integer.put", traces[0].name);

                var labels = trace.measures.Select(m => m.label).ToArray();

                StringCollection toCheck = new StringCollection();
                toCheck.AddRange(_to_check);
                foreach (var l in labels)
                {
                    TestContext.WriteLine("check: '" + l + "'");
                    CollectionAssert.Contains(toCheck, l, l);
                }

                traces = _cluster.GetPerformanceTraces();
                Assert.AreEqual(0, traces.Length);
            }
            finally
            {
                _cluster.DisablePerformanceTraces();
                key.Remove();
            }
        }

        [TestMethod]
        public void ReturnReadEntryLabels()
        {
            var key = _cluster.Integer("quarante-deux");
            key.Put(42);
            try
            {
                _cluster.EnablePerformanceTraces();
                key.Get();

                var traces = _cluster.GetPerformanceTraces();
                Assert.AreEqual(1, traces.Length);

                var trace = traces[0];
                Assert.AreEqual("common.get", trace.name);

                var labels = trace.measures.Select(m => m.label).ToArray();

                StringCollection toCheck = new StringCollection();
                toCheck.AddRange(_to_check);
                foreach (var l in labels)
                {
                    TestContext.WriteLine("check: '" + l + "'");
                    CollectionAssert.Contains(toCheck, l, l);
                }

                traces = _cluster.GetPerformanceTraces();
                Assert.AreEqual(0, traces.Length);
            }
            finally
            {
                _cluster.DisablePerformanceTraces();
                key.Remove();
            }
        }
    }
}
