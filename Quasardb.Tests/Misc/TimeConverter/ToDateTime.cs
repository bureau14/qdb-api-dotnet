using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.NativeApi;

namespace Quasardb.Tests.Misc.TimeConverter
{
    [TestClass]
    public class ToDateTime
    {
        [TestMethod]
        public void GivenZero_ReturnsEpoch()
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            Check(qdb_timespec.Zero, epoch);
        }

        [TestMethod]
        public void GivenMaxValue_ReturnsMaxValue()
        {
            Check(qdb_timespec.MaxValue, DateTime.MaxValue);
        }

        [TestMethod]
        public void GivenMinValue_ReturnsMinValue()
        {
            Check(qdb_timespec.MinValue, DateTime.MinValue);
        }

        [TestMethod]
        public void RealWorldExample()
        {
            Check(
                new qdb_timespec {tv_nsec = 666000000, tv_sec = 1488822490},
                new DateTime(2017, 3, 6, 17, 48, 10, 666, DateTimeKind.Utc)
            );
        }

        static void Check(qdb_timespec input, DateTime expectedOutput)
        {
            var actualOutput = Quasardb.TimeConverter.ToDateTime(input);
            Assert.AreEqual(expectedOutput, actualOutput);
        }
    }
}
