using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;

namespace Quasardb.Tests.Entry.String
{
    [TestClass]
    public class ExpiresFromNow
    {
        [TestMethod]
        public void ThrowsAliasNotFound_OnNewAlias()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var delay = TimeSpan.FromMinutes(42);

            try
            {
                s.ExpiresFromNow(delay);
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(s.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void NoError_AfterPut()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content = RandomGenerator.CreateRandomString();
            var delay = TimeSpan.FromMinutes(42);

            s.Put(content);
            s.ExpiresFromNow(delay);
        }
    }
}
