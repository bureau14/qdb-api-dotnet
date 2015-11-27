using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests
{
    [TestClass]
    public class QdbStreamTest
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

        #region Open()

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void GivenRandomAlias_WhenOpen_ThenAliasNotFound()
        {
            QdbTestCluster.Instance.Stream("toto").Open(QdbStreamMode.Open);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void GivenExistingAlias_WhenOpen_ThenIncompatibleType()
        {
            var blob = QdbTestCluster.CreateBlob();
            QdbTestCluster.Instance.Stream(blob.Alias).Open(QdbStreamMode.Open);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void GivenExistingAlias_WhenAppend_ThenIncompatibleType()
        {
            var blob = QdbTestCluster.CreateBlob();
            QdbTestCluster.Instance.Stream(blob.Alias).Open(QdbStreamMode.Append);
        }

        #endregion

        #region Length

        [TestMethod]
        public void GivenEmptyStream_ThenLengthIsZero()
        {
            Assert.AreEqual(0, _stream.Length);
        }

        [TestMethod]
        public void GivenEmptyStream_WhenWriteByte_ThenLengthIsOne()
        {
            _stream.WriteByte(42);
            Assert.AreEqual(1, _stream.Length);
        }

        [TestMethod]
        public void GivenEmptyStream_WhenWriteTwoBytes_ThenLengthIsTwo()
        {
            _stream.Write(new byte[10], 2, 2);
            Assert.AreEqual(2, _stream.Length);
        }

        [TestMethod]
        public void GivenEmptyStream_WhenWriteZeroBytes_ThenLengthIsZero()
        {
            _stream.Write(new byte[10], 2, 0);
            Assert.AreEqual(0, _stream.Length);
        }

        #endregion

        #region Write()

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

        #endregion
    }
}
