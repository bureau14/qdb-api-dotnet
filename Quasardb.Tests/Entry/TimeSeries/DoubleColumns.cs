using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.TimeSeries
{
    [TestClass]
    public class DoubleColumns
    {
        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.Instance.TimeSeries(alias);
        
            try
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                ts.DoubleColumns.ToArray();
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasNotFoundException e)
            {
                Assert.AreEqual(alias, e.Alias);
            }
        }

        [TestMethod]
        public void ThrowsIncompatibleType()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.Instance.TimeSeries(alias);
            QdbTestCluster.CreateBlob(alias);

            try
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                ts.DoubleColumns.ToArray();
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(alias, e.Alias);
            }
        }

        [TestMethod]
        public void ReturnsOnlyDoubleColumns()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.Instance.TimeSeries(alias);

            ts.Create(
                new QdbBlobColumnDefinition("John"),
                new QdbDoubleColumnDefinition("Paul"),
                new QdbBlobColumnDefinition("Georges"),
                new QdbDoubleColumnDefinition("Ringo"));

            var result = ts.DoubleColumns.ToArray();
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("Paul", result[0].Name);
            Assert.AreEqual("Ringo", result[1].Name);
        }
    }
}
