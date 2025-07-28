using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Quasardb.Exceptions;

namespace Quasardb.Tests.Entry.Timestamp
{
    [TestClass]
    public class Get
    {
        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var ts = QdbTestCluster.CreateEmptyTimestamp();

            try
            {
                ts.Get();
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(ts.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ThrowsIncompatibleType()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.CreateEmptyTimestamp(alias);

            QdbTestCluster.CreateBlob(alias);

            try
            {
                ts.Get();
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(ts.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ReturnsValue_AfterPut()
        {
            var ts = QdbTestCluster.CreateEmptyTimestamp();

            var dt = new DateTime(2020, 1, 1);
            ts.Put(dt);
            var value = ts.Get();

            Assert.AreEqual(dt, value);
        }

        [TestMethod]
        public void ReturnsNewValue_AfterUpdate()
        {
            var ts = QdbTestCluster.CreateEmptyTimestamp();

            ts.Put(new DateTime(2020, 1, 1));
            ts.Update(new DateTime(2021, 1, 1));
            var value = ts.Get();

            Assert.AreEqual(new DateTime(2021, 1, 1), value);
        }
    }
}
