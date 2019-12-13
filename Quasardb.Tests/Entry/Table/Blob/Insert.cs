using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.Exceptions;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Entry.Table.Blob
{
    [TestClass]
    public class Insert
    {
        [TestMethod]
        public void ThrowsIncompatibleType()
        {
            var points = new QdbBlobPointCollection
            {
                {new DateTime(2000, 01, 01), new byte[]{1}},
                {new DateTime(2000, 01, 02), new byte[]{2}},
            };
            var alias = RandomGenerator.CreateUniqueAlias();
            QdbTestCluster.CreateBlob(alias);
            var col = QdbTestCluster.Instance.Table(alias).BlobColumns["hello"];

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

        [TestMethod]
        public void Ok_WithValues()
        {
            var points = new QdbBlobPointCollection
            {
                {new DateTime(2000, 01, 01), new byte[]{1}},
                {new DateTime(2000, 01, 02), new byte[]{2}},
            };
            var col = QdbTestCluster.CreateEmptyBlobColumn();

            col.Insert(points);

            CollectionAssert.AreEqual(points.ToArray(), col.Points().ToArray());
        }

        [TestMethod]
        public void Ok_WithNulls()
        {
            var points = new QdbBlobPointCollection
            {
                {new DateTime(2000, 01, 01), new byte[]{1}},
                {new DateTime(2000, 01, 02), null},
            };
            var col = QdbTestCluster.CreateEmptyBlobColumn();

            col.Insert(points);

            CollectionAssert.AreEqual(points.ToArray(), col.Points().ToArray());
        }
    }
}
