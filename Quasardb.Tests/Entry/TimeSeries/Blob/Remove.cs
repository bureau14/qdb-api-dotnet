﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quasardb.Tests.Entry.TimeSeries.Blob
{
    [TestClass]
    public class Remove
    {
        [TestMethod]
        public void GivenNonExistingColumn_ReturnsFalse()
        {
            var column = QdbTestCluster.CreateEmptyBlobColumn();

            Assert.IsFalse(column.Remove());
        }

        [TestMethod]
        public void GivenExistingColumn_ReturnsTrue()
        {
            var column = QdbTestCluster.CreateBlobColumn();

            Assert.IsTrue(column.Remove());
        }
    }
}