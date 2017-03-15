using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.NativeApi;

namespace Quasardb.Tests
{
    [TestClass]
    public class TimeConverterTests
    {
        [TestMethod]
        public void GivenTimeSpecZero_ReturnsEpoch()
        {
            qdb_timespec ts;
            ts.tv_nsec = 0;
            ts.tv_sec = 0;

            var result = TimeConverter.ToDateTime(ts);

            Assert.AreEqual(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), result);
        }

        [TestMethod]
        public void GivenEpoch_ReturnsZero()
        {
            var result = TimeConverter.ToTimespec(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            Assert.AreEqual(0, result.tv_sec);
            Assert.AreEqual(0, result.tv_nsec);
        }

        [TestMethod]
        public void GivenTimeSpec_ReturnsDate()
        {
            qdb_timespec ts;
            ts.tv_nsec = 666000000;
            ts.tv_sec = 1488822490;

            var result = TimeConverter.ToDateTime(ts);

            Assert.AreEqual(new DateTime(2017, 3, 6, 17, 48, 10, 666, DateTimeKind.Utc), result);
        }

        [TestMethod]
        public void GivenDate_ReturnsTimeSpec()
        {
            var result = TimeConverter.ToTimespec(new DateTime(2017, 3, 6, 17, 48, 10, 666, DateTimeKind.Utc));

            Assert.AreEqual(1488822490, result.tv_sec);
            Assert.AreEqual(666000000, result.tv_nsec);
        }
    }
}
