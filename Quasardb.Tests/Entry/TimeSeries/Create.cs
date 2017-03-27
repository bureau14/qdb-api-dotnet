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
        [ExpectedException(typeof(QdbInvalidArgumentException))]
        public void ThrowsInvalidArgument_GivenNoColumns()
        {
            var alias = RandomGenerator.CreateUniqueAlias();
            var ts = QdbTestCluster.Instance.TimeSeries(alias);

            ts.Create();
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
