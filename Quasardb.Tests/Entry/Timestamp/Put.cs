using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Quasardb.Exceptions;

namespace Quasardb.Tests.Entry.Timestamp
{
    [TestClass]
    public class Put
    {
        [TestMethod]
        public void ThrowsAliasAlreadyExists()
        {
            var ts = QdbTestCluster.CreateEmptyTimestamp();

            ts.Put(new DateTime(2020, 1, 1));

            try
            {
                ts.Put(new DateTime(2021, 1, 1));
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasAlreadyExistsException e)
            {
                Assert.AreEqual(ts.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ThrowsIncompatibleType()
        {
            var ts = QdbTestCluster.CreateEmptyTimestamp();

            QdbTestCluster.CreateBlob(ts.Alias);

            try
            {
                ts.Put(new DateTime(2020, 1, 1));
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(ts.Alias, e.Alias);
            }
        }
    }
}
