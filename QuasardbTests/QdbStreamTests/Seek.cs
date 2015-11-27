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
        public void MoveToTheMiddle_BeginMode()
        {
            var result = _stream.Seek(5, SeekOrigin.Begin);
            Assert.AreEqual(5, result);
        }
    }
}
