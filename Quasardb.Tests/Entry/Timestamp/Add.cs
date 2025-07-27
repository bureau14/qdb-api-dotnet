using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;

namespace Quasardb.Tests.Entry.Timestamp
{
    [TestClass]
    public class Add
    {
        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var ts = QdbTestCluster.CreateEmptyTimestamp();

            try
            {
                ts.Add(TimeSpan.FromSeconds(1));
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(ts.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ThrowIncompatibleType()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.CreateEmptyTimestamp(alias);

            QdbTestCluster.CreateBlob(alias);

            try
            {
                ts.Add(TimeSpan.FromSeconds(1));
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(ts.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ReturnsSum()
        {
            var ts = QdbTestCluster.CreateEmptyTimestamp();

            ts.Put(new DateTime(2000, 1, 1));
            var result = ts.Add(TimeSpan.FromDays(1));

            Assert.AreEqual(new DateTime(2000, 1, 2), result);
        }
    }
}
