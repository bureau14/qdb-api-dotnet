using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbStreamTests
{
    [TestClass]
    public class Seek
    {
        private Stream _stream;

        [TestInitialize]
        public void Initiliaze()
        {
            _stream = QdbTestCluster.CreateAndOpenStream();
            _stream.Write(new byte[10], 0, 10);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _stream.Close();
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException), "Cannot access a closed Stream.")]
        public void ThrowsObjectDisposedException()
        {
            _stream.Close();
            _stream.Seek(0, SeekOrigin.Begin);
        }

        [TestMethod]
        public void Underrange_BeginMode()
        {
            var result = _stream.Seek(-5, SeekOrigin.Begin);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Underrange_CurrentMode()
        {
            var result = _stream.Seek(-15, SeekOrigin.Current);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Underrange_EndMode()
        {
            var result = _stream.Seek(-15, SeekOrigin.End);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Overrange_BeginMode()
        {
            var result = _stream.Seek(15, SeekOrigin.Begin);
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void Overrange_CurrentMode()
        {
            var result = _stream.Seek(5, SeekOrigin.Current);
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void Overrange_EndMode()
        {
            var result = _stream.Seek(5, SeekOrigin.End);
            Assert.AreEqual(10, result);
        }
        
        [TestMethod]
        public void MoveToTheMiddle_BeginMode()
        {
            var result = _stream.Seek(5, SeekOrigin.Begin);
            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void MoveToTheMiddle_CurrentMode()
        {
            _stream.Seek(-3, SeekOrigin.Current);
            var result = _stream.Seek(-2, SeekOrigin.Current);
            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void MoveToTheMiddle_EndMode()
        {
            var result = _stream.Seek(-5, SeekOrigin.End);
            Assert.AreEqual(5, result);
        }
    }
}
