using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbStream
{
    [TestClass]
    public class Write
    {
        private Stream _stream;

        [TestInitialize]
        public void Initiliaze()
        {
            _stream = QdbTestCluster.CreateStream();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _stream.Close();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.")]
        public void WhenTheSumOfOffsetAndCountIsGreaterThanBufferLength_ThenArgumentException()
        {
            _stream.Write(new byte[2], 1, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Buffer cannot be null.Parameter name: buffer.")]
        public void WhenBufferIsNull_ThenArgumentNullException()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            _stream.Write(null, 0, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Non-negative number required.Parameter name: offset.")]
        public void WhenOffsetIsNetagive_ThenArgumentOutOfRangeException()
        {
            _stream.Write(new byte[2], -1, 4);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Non-negative number required.Parameter name: count.")]
        public void WhenCountIsNetagive_ThenArgumentOutOfRangeException()
        {
            _stream.Write(new byte[2], 4, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException), "Cannot access a closed Stream.")]
        public void GivenClosedStream_WhenWrite_ThenObjectDisposedException()
        {
            _stream.Close();
            _stream.Write(new byte[2], 0, 2);
        }
    }
}
