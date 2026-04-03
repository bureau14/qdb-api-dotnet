using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.Table
{
    [TestClass]
    public class ExpireBySize
    {
        [TestMethod]
        public void ThrowsInvalidArgument_GivenNegativeSize()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.Instance.Table(alias);

            ts.Create();

            try
            {
                ts.ExpireBySize(-1);
                Assert.Fail("No exception thrown");
            }
            catch (QdbInvalidArgumentException)
            { }
        }
    }
}
