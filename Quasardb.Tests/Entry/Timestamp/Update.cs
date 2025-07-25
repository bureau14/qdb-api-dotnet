using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Quasardb.Exceptions;

namespace Quasardb.Tests.Entry.Timestamp
{
    [TestClass]
    public class Update
    {
        [TestMethod]
        public void ReturnsTrue_WhenCalledOnce()
        {
            var ts = QdbTestCluster.CreateEmptyTimestamp();

            var created = ts.Update(new DateTime(2020, 1, 1));

            Assert.IsTrue(created);
        }

        [TestMethod]
        public void ReturnsFalse_WhenCalledTwice()
        {
            var ts = QdbTestCluster.CreateEmptyTimestamp();

            ts.Update(new DateTime(2020, 1, 1));
            var created = ts.Update(new DateTime(2021, 1, 1));

            Assert.IsFalse(created);
        }
    }
}
