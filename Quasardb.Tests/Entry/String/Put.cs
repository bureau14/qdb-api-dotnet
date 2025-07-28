using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;

namespace Quasardb.Tests.Entry.String
{
    [TestClass]
    public class Put
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNull()
        {
            var s = QdbTestCluster.CreateEmptyString();

            s.Put(null);
        }

        [TestMethod]
        public void NoError_WhenCalledOnce()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content = RandomGenerator.CreateRandomString();

            s.Put(content);
        }

        [TestMethod]
        public void ThrowsAliasAlreadyExists_WhenCalledTwice()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content = RandomGenerator.CreateRandomString();

            s.Put(content);

            try
            {
                s.Put(content);
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasAlreadyExistsException e)
            {
                Assert.AreEqual(s.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void NoError_AfterRemove()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content = RandomGenerator.CreateRandomString();

            s.Put(content);
            s.Remove();
            s.Put(content);
        }

        [TestMethod]
        public void NoError_AfterRemoveIf()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content = RandomGenerator.CreateRandomString();

            s.Put(content);
            s.RemoveIf(content);
            s.Put(content);
        }

        [TestMethod]
        public void NoError_AfterGetAndRemove()
        {
            var s = QdbTestCluster.CreateEmptyString();
            var content = RandomGenerator.CreateRandomString();

            s.Put(content);
            s.GetAndRemove();
            s.Put(content);
        }
    }
}
