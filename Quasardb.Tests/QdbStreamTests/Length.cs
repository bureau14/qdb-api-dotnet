using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Tests.Helpers;

namespace Quasardb.Tests.QdbStreamTests
{
    [TestClass]
    public class Length
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
        public void ReturnsZero_OnNewStream()
        {
            Assert.AreEqual(0, _stream.Length);
        }

        [TestMethod]
        public void ReturnsOne_AfterWriteByte()
        {
            _stream.WriteByte(42);
            Assert.AreEqual(1, _stream.Length);
        }

        [TestMethod]
        public void ReturnsTwo_AfterWrite()
        {
            _stream.Write(new byte[10], 2, 2);
            Assert.AreEqual(2, _stream.Length);
        }

        [TestMethod]
        public void ReturnsZero_AfterWrite()
        {
            _stream.Write(new byte[10], 2, 0);
            Assert.AreEqual(0, _stream.Length);
        }

        [TestMethod]
        public void ReturnsZero_AfterRemove()
        {
            var stream = QdbTestCluster.CreateStream();
            stream.Remove();
            _stream = stream.Open(QdbStreamMode.Append);

            Assert.AreEqual(0, _stream.Length);
        }
    }
}