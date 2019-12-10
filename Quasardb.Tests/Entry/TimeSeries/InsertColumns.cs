using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.TimeSeries
{
    [TestClass]
    public class InsertColumns
    {
        [TestMethod]
        public void ThrowsInvalidArgument_GivenNoColumns()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.Instance.TimeSeries(alias);

            ts.Create();

            try
            {
                ts.InsertColumns();
                Assert.Fail("No exception thrown");
            }
            catch (QdbInvalidArgumentException)
            { }
        }

        [TestMethod]
        public void OK_GivenOneColumn()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.Instance.TimeSeries(alias);
            var columns = new QdbColumnDefinition[] { new QdbBlobColumnDefinition("Hello") };

            ts.Create();

            try
            {
                ts.InsertColumns(columns);
            }
            catch (QdbException)
            {
                Assert.Fail("Creating a timeseries with a single column should not throw");
            }
        }

        [TestMethod]
        public void OK_GivenSeveralColumn()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.Instance.TimeSeries(alias);
            var columns = new QdbColumnDefinition[]{
                new QdbBlobColumnDefinition("Hello"),
                new QdbDoubleColumnDefinition("World"),
                new QdbBlobColumnDefinition("Comment"),
                new QdbDoubleColumnDefinition("Value")};

            ts.Create();

            try
            {
                ts.InsertColumns(columns);
            }
            catch (QdbException)
            {
                Assert.Fail("Creating a timeseries with several column should not throw");
            }
        }

        [TestMethod]
        public void ThrowsInvalidArgument_GivenExistingColumn()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.Instance.TimeSeries(alias);

            ts.Create();
            ts.InsertColumns(new QdbBlobColumnDefinition("Hello"));

            try
            {
                ts.InsertColumns(new QdbBlobColumnDefinition("Hello"));
                Assert.Fail("No exception thrown");
            }
            catch (QdbInvalidArgumentException)
            { }
        }
    }
}
