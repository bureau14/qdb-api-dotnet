using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.NativeApi;

namespace Quasardb.Tests.QdbTimeConverterTests
{
    [TestClass]
    public class ToTimespec
    {
        [TestMethod]
        public void GivenEpoch_ReturnsZero()
        {
            Check(
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), 
                qdb_timespec.Zero
            );
        }

        [TestMethod]
        public void GivenMaxValue_Returns31December9999()
        {
            Check(
                DateTime.MaxValue,
                new qdb_timespec {tv_sec = 253402300799, tv_nsec = 999999900 }
            );
        }

        [TestMethod]
        public void GivenMinValue_Returns1January0001()
        {
            Check(
                DateTime.MinValue,
                new qdb_timespec { tv_sec = -62135596800, tv_nsec = 0}
            );
        }

        [TestMethod]
        public void RealWorldExample()
        {
            Check(
                new DateTime(2017, 3, 6, 17, 48, 10, 666, DateTimeKind.Utc),
                new qdb_timespec {tv_nsec = 666000000, tv_sec = 1488822490}
            );
        }

        static void Check(DateTime input, qdb_timespec expectedOutput)
        {
            var actualOutput = TimeConverter.ToTimespec(input);
            Assert.AreEqual(expectedOutput.tv_sec, actualOutput.tv_sec);
            Assert.AreEqual(expectedOutput.tv_nsec, actualOutput.tv_nsec);
        }
    }
}
