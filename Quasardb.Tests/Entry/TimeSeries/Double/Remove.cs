using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quasardb.Tests.Entry.TimeSeries.Double
{
    [TestClass]
    public class Remove
    {
        [TestMethod]
        public void GivenNonExistingColumn_ReturnsFalse()
        {
            var column = QdbTestCluster.CreateEmptyDoubleColumn();

            Assert.IsFalse(column.Remove());
        }

        [TestMethod]
        public void GivenExistingColumn_ReturnsTrue()
        {
            var column = QdbTestCluster.CreateDoubleColumn();

            Assert.IsTrue(column.Remove());
        }
    }
}