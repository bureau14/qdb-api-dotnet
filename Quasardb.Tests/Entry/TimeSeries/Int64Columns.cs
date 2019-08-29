using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.TimeSeries
{
    [TestClass]
    public class Int64Columns
    {
        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.Instance.TimeSeries(alias);

            try
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                ts.Int64Columns.ToArray();
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
                ts.Int64Columns.ToArray();
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(alias, e.Alias);
            }
        }

        [TestMethod]
        public void ReturnsOnlyInt64Columns()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.Instance.TimeSeries(alias);

            ts.Create(
                new QdbBlobColumnDefinition("John"),
                new QdbInt64ColumnDefinition("Paul"),
                new QdbBlobColumnDefinition("Georges"),
                new QdbInt64ColumnDefinition("Ringo"));

            var result = ts.Int64Columns.ToArray();
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("Paul", result[0].Name);
            Assert.AreEqual("Ringo", result[1].Name);
        }
    }
}
