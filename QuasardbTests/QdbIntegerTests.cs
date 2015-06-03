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
    }
}
