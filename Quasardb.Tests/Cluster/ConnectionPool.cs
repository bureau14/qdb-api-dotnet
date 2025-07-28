using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasardb.Tests.Cluster
{
    [TestClass]
    public class ConnectionPool
    {
        public static QdbConnectionPool CreatePool(int size)
        {
            var factory = new QdbConnectionFactory(DaemonRunner.ClusterUrl);

            if (DaemonRunner.UseSecurity)
            {
                factory.WithSecurity(DaemonRunner.ClusterPublicKey, DaemonRunner.UserName, DaemonRunner.UserPrivateKey);
            }
            return new QdbConnectionPool(factory, size);
        }

        [TestMethod]
        public void CantTakeMoreThanLimit()
        {
            var pool = CreatePool(2);
            var cs1 = pool.Get();
            var cs2 = pool.Get();

            var limit = new TimeSpan(0, 0, 1);
            QdbCluster cs3;

            // This one won't work
            Assert.IsFalse(pool.TryGet(out cs3, limit));

            var content = Encoding.UTF8.GetBytes($"Hello world!!!!");

            var alias1 = RandomGenerator.CreateUniqueAlias();
            cs1.Blob(alias1).Put(content);
            var alias2 = RandomGenerator.CreateUniqueAlias();
            cs2.Blob(alias2).Put(content);

            pool.Return(cs1);

            // After returning one connection we can get it
            Assert.IsTrue(pool.TryGet(out cs3, limit));
            CollectionAssert.AreEqual(cs3.Blob(alias1).Get(), content);
        }

        [TestMethod]
        public void PoolSize()
        {
            var pool = CreatePool(2);
            Assert.AreEqual(pool.Size(), 2);
        }
    }
}
