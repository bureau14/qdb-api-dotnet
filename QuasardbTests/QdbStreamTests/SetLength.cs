using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuasardbTests.Helpers;

namespace QuasardbTests.QdbStreamTests
{
    [TestClass]
    public class SetLength
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
        [ExpectedException(typeof(NotSupportedException))]
        public void ThrowNotSupportedException()
        {
            _stream.SetLength(0);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException), "Cannot access a closed Stream.")]
        public void ThrowObjectDisposedException()
        {
            _stream.Close();
            _stream.SetLength(0);
        }

    }
}
