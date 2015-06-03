using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb;
using Quasardb.Exceptions;

namespace QuasardbTests
{
    [TestClass]
    public class QdbIntegerTests
    {
        QdbInteger _integer;
       
        [TestInitialize]
        public void Initialize()
        {
            var cluster = new QdbCluster(DaemonRunner.ClusterUrl);
            var alias = Utils.CreateUniqueAlias();
            _integer = cluster.Integer(alias);
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
