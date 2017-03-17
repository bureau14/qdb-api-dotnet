using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quasardb.Tests.Entry.Stream
{
    [TestClass]
    public class Read
    {
        private System.IO.Stream _stream;
        private static readonly byte[] _content = Encoding.ASCII.GetBytes("Hello World!");

        [TestInitialize]
        public void Initiliaze()
        {
            _stream = QdbTestCluster.CreateAndOpenStream();
            _stream.Write(_content, 0, _content.Length);
            _stream.Position = 0;
        }

        [TestCleanup]
        public void Cleanup()
        {
            _stream.Close();
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void ThrowsObjectDisposedException()
        {
            _stream.Close();
            _stream.Read(new byte[2], 0, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNullException_WhenBufferIsNull()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            _stream.Read(null, 0, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowsArgumentOutOfRangeException_WhenOffsetIsNegative()
        {
            _stream.Read(new byte[2], -1, 4);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowsArgumentOutOfRangeException_WhenCountIsNegative()
        {
            _stream.Read(new byte[2], 4, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ThrowsArgumentException_WhenSumOfOffsetAndLengthIsToBig()
        {
            _stream.Read(new byte[2], 1, 2);
        }

        [TestMethod]
        public void ReturnsNumberOfBytes_Section()
        {
            var result = _stream.Read(new byte[2], 0, 2);
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void ReturnsNumberOfBytes_Whole()
        {
            var result = _stream.Read(new byte[100], 0, 100);
            Assert.AreEqual(_content.Length, result);
        }

        [TestMethod]
        public void FillsBuffer_Section()
        {
            var buffer = new byte[2];
            _stream.Read(buffer, 0, 2);
            Assert.AreEqual(_content[0], buffer[0]);
            Assert.AreEqual(_content[1], buffer[1]);
        }
        [TestMethod]
        public void FillsBuffer_Whole()
        {
            var buffer = new byte[100];
            var len = _stream.Read(buffer, 0, 100);
            Array.Resize(ref buffer, len);
            CollectionAssert.AreEqual(_content, buffer);
        }
    }
}
