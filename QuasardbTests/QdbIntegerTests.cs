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
        public void Add()
        {
            _integer.Add(22);
        }

        [TestMethod]
        public void Put_Add_Get()
        {
            _integer.Put(1934);
            var resultOfAdd = _integer.Add(22);
            var resultOfGet = _integer.Get();

            Assert.AreEqual(1956, resultOfAdd);
            Assert.AreEqual(1956, resultOfGet);
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasNotFoundException))]
        public void Get()
        {
            _integer.Get();
        }

        [TestMethod]
        [ExpectedException(typeof(QdbAliasAlreadyExistsException))]
        public void Put_Put()
        {
            _integer.Put(1934);
            _integer.Put(1956);
        }

        [TestMethod]
        public void Put_Get()
        {
            _integer.Put(42);
            var value = _integer.Get();

            Assert.AreEqual(42, value);
        }

        [TestMethod]
        public void Put_Update_Get()
        {
            _integer.Put(1934);
            _integer.Update(1956);
            var value = _integer.Get();

            Assert.AreEqual(1956, value);
        }
    }
}
