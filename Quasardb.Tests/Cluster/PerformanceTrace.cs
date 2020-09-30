using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quasardb.Tests.Cluster
{
    [TestClass]
    public class PerformanceTrace
    {
        private readonly QdbCluster _cluster = QdbTestCluster.Instance;

        [TestMethod]
        public void ReturnNoLabelsByDefault()
        {
            _cluster.Integer("quarante-deux").Put(42);
            var traces = _cluster.GetPerformanceTraces();
            Assert.AreEqual(0, traces.Length);
        }

        [TestMethod]
        public void ReturnNoLabelsWhenEnabled()
        {
            _cluster.EnablePerformanceTraces();
            var traces = _cluster.GetPerformanceTraces();
            Assert.AreEqual(0, traces.Length);
        }

        [TestMethod]
        public void ReturnWriteEntryLabels()
        {
            _cluster.EnablePerformanceTraces();

            _cluster.Integer("quarante-deux").Put(42);

            var traces = _cluster.GetPerformanceTraces();
            Assert.AreEqual(1, traces.Length);

            Assert.AreEqual("beep", traces[0].name);
            Assert.AreEqual("yo", traces[0].measures[0].label);
            Assert.AreEqual("amigo", traces[0].measures[1].label);
            Assert.AreEqual("wassup", traces[0].measures[2].label);
            Assert.AreEqual("eik", traces[0].measures[3].label);
            Assert.AreEqual("boo", traces[0].measures[4].label);
            Assert.AreEqual("oh", traces[0].measures[5].label);
        }

        [TestMethod]
        public void ReturnReadEntryLabels()
        {
            _cluster.Integer("quarante-deux").Put(42);

            _cluster.EnablePerformanceTraces();

            _cluster.Integer("quarante-deux").Get();

            var traces = _cluster.GetPerformanceTraces();
            Assert.AreEqual(1, traces.Length);

            Assert.AreEqual("beep", traces[0].name);
            Assert.AreEqual("yo", traces[0].measures[0].label);
            Assert.AreEqual("amigo", traces[0].measures[1].label);
            Assert.AreEqual("wassup", traces[0].measures[2].label);
            Assert.AreEqual("eik", traces[0].measures[3].label);
            Assert.AreEqual("boo", traces[0].measures[4].label);
            Assert.AreEqual("oh", traces[0].measures[5].label);
        }
    }
}
