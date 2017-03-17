using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quasardb.Tests.Entry.Stream
{
    [TestClass]
    public class Write
    {
        private System.IO.Stream _stream;

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
        [ExpectedException(typeof(ArgumentException), "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.")]
        public void ThrowsArgumentException_WhenSumOfOffsetAndLengthIsToBig()
        {
            _stream.Write(new byte[2], 1, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Buffer cannot be null.Parameter name: buffer.")]
        public void ThrowsArgumentNullException_WhenBufferIsNull()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            _stream.Write(null, 0, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Non-negative number required.Parameter name: offset.")]
        public void ThrowsArgumentOutOfRangeException_WhenOffsetIsNegative()
        {
            _stream.Write(new byte[2], -1, 4);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Non-negative number required.Parameter name: count.")]
        public void ThrowsArgumentOutOfRangeException_WhenCountIsNegative()
        {
            _stream.Write(new byte[2], 4, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException), "Cannot access a closed Stream.")]
        public void ThrowsObjectDisposedException()
        {
            _stream.Close();
            _stream.Write(new byte[2], 0, 2);
        }
    }
}
