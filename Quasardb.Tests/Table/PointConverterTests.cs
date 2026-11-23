using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.TimeSeries;

namespace Quasardb.Tests.Table
{
    [TestClass]
    public class PointConverterTests
    {
        [TestMethod]
        public void StringPoint_ToNativeUsesUtf8ByteCount()
        {
            var point = new QdbStringPoint(new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc), "café");
            var native = point.ToNative(out var pin);
            try
            {
                Assert.AreEqual(5UL, native.content_size.ToUInt64());
                Assert.AreEqual(point.Value, native.ToManaged().Value);
            }
            finally
            {
                pin?.Free();
            }
        }
    }
}
