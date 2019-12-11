using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.Table
{
    [TestClass]
    public class Columns
    {
        [TestMethod]
        public void ThrowsAliasNotFound()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.Instance.Table(alias);

            try
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                ts.Columns.ToArray();
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
            var ts = QdbTestCluster.Instance.Table(alias);
            QdbTestCluster.CreateBlob(alias);

            try
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                ts.Columns.ToArray();
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(alias, e.Alias);
            }
        }

        [TestMethod]
        public void ReturnsAllColumns()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.Instance.Table(alias);

            ts.Create(
                new QdbBlobColumnDefinition("John"),
                new QdbDoubleColumnDefinition("Paul"),
                new QdbBlobColumnDefinition("Georges"),
                new QdbDoubleColumnDefinition("Ringo"));

            var result = ts.Columns.ToArray();
            Assert.AreEqual(4, result.Length);
            Assert.AreEqual("John", result[0].Name);
            Assert.AreEqual("Paul", result[1].Name);
            Assert.AreEqual("Georges", result[2].Name);
            Assert.AreEqual("Ringo", result[3].Name);
        }
    }
}
