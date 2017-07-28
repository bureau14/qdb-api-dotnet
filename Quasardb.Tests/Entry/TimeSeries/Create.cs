using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.TimeSeries
{
    [TestClass]
    public class Create
    {
        [TestMethod]
        public void OK_GivenNoColumns()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.Instance.TimeSeries(alias);

            try
            {
                ts.Create();
            }
            catch (QdbException)
            {
                Assert.Fail("Creating a timeseries without columns should not throw");
            }
        }

        [TestMethod]
        public void ThrowsIncompatibleType_GivenExistingBlob()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.Instance.TimeSeries(alias);

            QdbTestCluster.CreateBlob(alias);

            try
            {
                ts.Create(new QdbBlobColumnDefinition("Hello"));
                Assert.Fail("No exception thrown");
            }
            catch (QdbIncompatibleTypeException e)
            {
                Assert.AreEqual(ts.Alias, e.Alias);
            }
        }

        [TestMethod]
        public void ThrowsAliasAlreadyExists_WhenCalledTwice()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.Instance.TimeSeries(alias);
            var columns = new QdbColumnDefinition[] {new QdbBlobColumnDefinition("Hello")};

            ts.Create(columns);

            try
            {
                ts.Create(columns);
                Assert.Fail("No exception thrown");
            }
            catch (QdbAliasAlreadyExistsException e)
            {
                Assert.AreEqual(ts.Alias, e.Alias);
            }
        }
    }
}
