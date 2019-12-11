using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.Table
{
    [TestClass]
    public class Create
    {
        [TestMethod]
        public void OK_GivenNoColumns()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.Instance.Table(alias);

            try
            {
                ts.Create();
            }
            catch (QdbException)
            {
                Assert.Fail("Creating a table without columns should not throw");
            }
        }

        [TestMethod]
        public void OK_GivenNoColumnsAndShardSize()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.Instance.Table(alias);

            try
            {
                ts.Create(TimeSpan.FromHours(5));
            }
            catch (QdbException)
            {
                Assert.Fail("Creating a table with shard size should not throw");
            }
        }

        [TestMethod]
        public void ThrowsIncompatibleType_GivenExistingBlob()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.Instance.Table(alias);

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
        public void OK_GivenOneColumn()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.Instance.Table(alias);
            var columns = new QdbColumnDefinition[] {new QdbBlobColumnDefinition("Hello")};

            try
            {
                ts.Create(columns);
            }
            catch (QdbException)
            {
                Assert.Fail("Creating a table with a single column should not throw");
            }
        }

        [TestMethod]
        public void OK_GivenSeveralColumn()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.Instance.Table(alias);
            var columns = new QdbColumnDefinition[]{
                new QdbBlobColumnDefinition("Hello"),
                new QdbDoubleColumnDefinition("World"),
                new QdbBlobColumnDefinition("Comment"),
                new QdbDoubleColumnDefinition("Value")};

            try
            {
                ts.Create(columns);
            }
            catch (QdbException)
            {
                Assert.Fail("Creating a table with several column should not throw");
            }
        }


        [TestMethod]
        public void ThrowsAliasAlreadyExists_WhenCalledTwice()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.Instance.Table(alias);
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

        [TestMethod]
        [ExpectedException(typeof(QdbInvalidArgumentException))]
        public void ThrowsInvalidArgument_GivenShardSizeLessThanOneMillisecond()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.Instance.Table(alias);

            ts.Create(TimeSpan.FromMilliseconds(0));
        }

        [TestMethod]
        [ExpectedException(typeof(QdbInvalidArgumentException))]
        public void ThrowsInvalidArgument_GivenShardSizeLessThanOneMillisecondAndColumns()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.Instance.Table(alias);

            var columns = new QdbColumnDefinition[] {new QdbBlobColumnDefinition("Hello")};
            ts.Create(TimeSpan.FromMilliseconds(0), columns);
        }
    }
}
