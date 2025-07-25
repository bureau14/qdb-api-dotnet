using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;

namespace Quasardb.Tests.Entry.String
{
    [TestClass]
    public class ExpiresAt
    {
        [TestMethod]
        public void ThrowsAliasNotFound_OnNewAlias()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var expiry = new DateTime(3000, 12, 25);

            try
            {
                s.ExpiresAt(expiry);
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
            var expiry = new DateTime(3000, 12, 25);

            s.Put(content);
            s.ExpiresAt(expiry);
        }
    }
}
