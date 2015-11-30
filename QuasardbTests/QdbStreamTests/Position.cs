using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbStreamTests
{
    [TestClass]
    public class Position
    {
        private Stream _stream;

        [TestInitialize]
        public void Initiliaze()
        {
            _stream = QdbTestCluster.CreateAndOpenStream();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _stream.Close();
        }

        [TestMethod]
        public void ReturnsZero_OnEmptyStream()
        {
            Assert.AreEqual(0, _stream.Position);
        }

        [TestMethod]
        public void ReturnsOne_AfterWriteByte()
        {
            _stream.WriteByte(42);
            Assert.AreEqual(1, _stream.Position);
        }

        [TestMethod]
        public void ReturnsOne_AfterSeek()
        {
            _stream.Write(new byte[10], 0, 10);
            _stream.Seek(1, SeekOrigin.Begin);
            Assert.AreEqual(1, _stream.Position);
        }

        [TestMethod]
        public void ReturnsOne_AfterSet()
        {
            _stream.Write(new byte[10], 0, 10);
            _stream.Position = 1;
            Assert.AreEqual(1, _stream.Position);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void ThrowObjectDisposedException_Getter()
        {
            _stream.Close();
            // ReSharper disable once UnusedVariable
            var position = _stream.Position;
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void ThrowObjectDisposedException_Setter()
        {
            _stream.Close();
            _stream.Position = 0;
        }

        [TestMethod]
        public void Underrange()
        {
            _stream.Write(new byte[10], 0, 10);
            _stream.Position = -5;
            Assert.AreEqual(0, _stream.Position);
        }

        [TestMethod]
        public void Overrange()
        {
            _stream.Write(new byte[10], 0, 10);
            _stream.Position = 15;
            Assert.AreEqual(10, _stream.Position);
        }
    }
}
