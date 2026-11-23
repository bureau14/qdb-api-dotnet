using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasardb.TimeSeries.Reader;

namespace Quasardb.Tests.Table
{
    [TestClass]
    public class BulkReaderHelperTests
    {
        [TestMethod]
        public void ConvertCharArray_PinsNullTerminatedUtf8()
        {
            var pins = new List<GCHandle>();
            try
            {
                var pointer = BulkReaderHelper.ConvertCharArray("café", ref pins);
                var pin = pins[0];

                Assert.AreEqual(pin.AddrOfPinnedObject(), pointer);
                CollectionAssert.AreEqual(new byte[] { 0x63, 0x61, 0x66, 0xC3, 0xA9, 0x00 }, (byte[])pin.Target);
            }
            finally
            {
                foreach (var pin in pins) pin.Free();
            }
        }
    }
}
