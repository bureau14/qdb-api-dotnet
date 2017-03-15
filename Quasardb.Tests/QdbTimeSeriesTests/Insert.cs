using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.Tests.Helpers;

namespace Quasardb.Tests.QdbTimeSeriesTests
{
    [TestClass]
    public class Insert
    {
        [TestMethod]
        public void ThrowsIncompatibleType()
        {
            var points = new QdbTimeSeries.PointCollection
            {
                {new DateTime(2000, 01, 01), 1},
                {new DateTime(2000, 01, 02), 2},
            };
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.CreateEmptyTimeSeries(alias);
            QdbTestCluster.CreateBlob(alias);

            try
            {
                ts.Insert(points);
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(ts.Alias, e.Alias);
            }
        }
    }
}
