﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;

namespace Quasardb.Tests.Cluster
{
    [TestClass]
    public class PerformanceTrace
    {
        private readonly QdbCluster _cluster = QdbTestCluster.Instance;

        [TestMethod]
        public void DisabledByDefault()
        {
            var traces = _cluster.GetPerformanceTraces();
            Assert.AreEqual(0, traces.Length);
        }

        [TestMethod]
        public void ReturnNoLabelsWhenEnabled()
        {
            System.Console.Write(_cluster.GetConfiguration("qdb://127.0.0.1:2836"));

            _cluster.EnablePerformanceTraces();

            var traces = _cluster.GetPerformanceTraces();
            Assert.AreEqual(0, traces.Length);

            _cluster.DisablePerformanceTraces();
        }

        [TestMethod]
        public void ReturnWriteEntryLabels()
        {
            
            var key = _cluster.Integer("quarante-deux");

            _cluster.EnablePerformanceTraces();
            key.Put(42);

            var traces = _cluster.GetPerformanceTraces();
            Assert.AreEqual(1, traces.Length);

            Assert.AreEqual("beep", traces[0].name);
            Assert.AreEqual("yo", traces[0].measures[0].label);
            Assert.AreEqual("amigo", traces[0].measures[1].label);
            Assert.AreEqual("wassup", traces[0].measures[2].label);
            Assert.AreEqual("eik", traces[0].measures[3].label);
            Assert.AreEqual("boo", traces[0].measures[4].label);
            Assert.AreEqual("oh", traces[0].measures[5].label);

            _cluster.DisablePerformanceTraces();
            key.Remove();
        }

        [TestMethod]
        public void ReturnReadEntryLabels()
        {
            var key = _cluster.Integer("quarante-deux");
            key.Put(42);

            _cluster.EnablePerformanceTraces();
            key.Get();

            var traces = _cluster.GetPerformanceTraces();
            Assert.AreEqual(1, traces.Length);

            Assert.AreEqual("beep", traces[0].name);
            Assert.AreEqual("yo", traces[0].measures[0].label);
            Assert.AreEqual("amigo", traces[0].measures[1].label);
            Assert.AreEqual("wassup", traces[0].measures[2].label);
            Assert.AreEqual("eik", traces[0].measures[3].label);
            Assert.AreEqual("boo", traces[0].measures[4].label);
            Assert.AreEqual("oh", traces[0].measures[5].label);
            
            _cluster.DisablePerformanceTraces();
            key.Remove();
        }
    }
}
