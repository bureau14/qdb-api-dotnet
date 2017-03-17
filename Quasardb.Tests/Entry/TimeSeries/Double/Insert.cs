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
        [Ignore] // Requires support for columns in qdb_api.dll
        public void ThrowsIncompatibleType()
        {
            var points = new QdbDoublePointCollection
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
                Assert.AreEqual(ts.Series.Alias, e.Alias);
            }
        }
    }
}
