using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;
using QuasardbTests.Helpers;

namespace QuasardbTests
{
    [TestClass]
    public class QdbIntegerTests
    {
        QdbInteger _integer;
       
        [TestInitialize]
        public void Initialize()
        {
            _integer = QdbTestCluster.CreateEmptyInteger();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void Add_NotFound()
        {
            _integer.Add(22);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void Add_Incompatible()
        {
            QdbTestCluster.CreateBlob(_integer.Alias);
            _integer.Add(22);
        }

        [TestMethod]
        public void Put_Add_Get()
        {
            _integer.Put(1934);
            Assert.AreEqual(1956, _integer.Add(22));
            Assert.AreEqual(1956, _integer.Get());
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void Get_NotFound()
        {
            _integer.Get();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void Get_Incompatible()
        {
            QdbTestCluster.CreateBlob(_integer.Alias);
            _integer.Get();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void Put_IncompatibleType()
        {
            QdbTestCluster.CreateBlob(_integer.Alias);
            _integer.Put(1934);
        }

        [TestMethod]
        public void Put_Get()
        {
            _integer.Put(42);
            Assert.AreEqual(42, _integer.Get());
        }

        [TestMethod]
        public void Put_Update_Get()
        {
            _integer.Put(1934);
            _integer.Update(1956);
            Assert.AreEqual(1956, _integer.Get());
        }

        [TestMethod]
        [ExpectedException(typeof(QdbIncompatibleTypeException))]
        public void Update_Incompatible()
        {
            QdbTestCluster.CreateBlob(_integer.Alias);
            _integer.Update(1956);
        }
    }
}
