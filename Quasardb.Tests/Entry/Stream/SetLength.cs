using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quasardb.Tests.Entry.Stream
{
    [TestClass]
    public class SetLength
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
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowsArgumentOutOfRangeException_WhenLengthIsNegative()
        {
            _stream.SetLength(-1);
        }

        [TestMethod]
        public void NoError_WhenNewLengthIsEqual()
        {
            _stream.Write(new byte[]{1,2}, 0, 2);

            _stream.SetLength(2);
        }

        [TestMethod]
        public void NoError_WhenNewLengthIsSmaller()
        {
            _stream.Write(new byte[] { 1, 2 }, 0, 2);

            _stream.SetLength(1);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void ThrowsNotSupportedException_WhenNewLengthIsBigger()
        {
            _stream.Write(new byte[] { 1, 2 }, 0, 2);

            _stream.SetLength(3);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void ThrowObjectDisposedException()
        {
            _stream.Close();
            _stream.SetLength(0);
        }

    }
}
