using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.TimeSeries.Double
{
    [TestClass]
    public class Insert
    {
        [TestMethod]
        public void ThrowsIncompatibleType()
        {
            var points = new QdbDoublePointCollection
            {
                {new DateTime(2000, 01, 01), 1},
                {new DateTime(2000, 01, 02), 2},
            };
            var alias = RandomGenerator.CreateUniqueAlias();
            QdbTestCluster.CreateBlob(alias);
            var col = QdbTestCluster.Instance.TimeSeries(alias).DoubleColumns["hello"];

            try
            {
                col.Insert(points);
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(col.Series.Alias, e.Alias);
            }
        }
    }
}
