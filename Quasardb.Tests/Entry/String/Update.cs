using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quasardb.Tests.Entry.String
{
    [TestClass]
    public class Update
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsArgumentNull()
        {
            var s = QdbTestCluster.CreateEmptyString();

            s.Update(null);
        }

        [TestMethod]
        public void ReturnsTrue_WithNewAlias()
        {
            var content = RandomGenerator.CreateRandomString();
            var s = QdbTestCluster.CreateEmptyString();

            var created = s.Update(content);

            Assert.IsTrue(created);
        }

        [TestMethod]
        public void ReturnsFalse_WhenCalledTwice()
        {
            var content = RandomGenerator.CreateRandomString();
            var s = QdbTestCluster.CreateEmptyString();

            s.Update(content);
            var created = s.Update(content);

            Assert.IsFalse(created);
        }
    }
}
