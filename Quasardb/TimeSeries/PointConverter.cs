using System;
using System.Runtime.InteropServices;
using Quasardb.Native;

namespace Quasardb.TimeSeries
{
    static class PointConverter
    {
        public static qdb_ts_double_point ToNative(this QdbPoint<double> pt)
        {
            return new qdb_ts_double_point
            {
                timestamp = TimeConverter.ToTimespec(pt.Time),
                value = pt.Value
            };
        }

        public static QdbDoublePoint ToManaged(this qdb_ts_double_point pt)
        {
            return new QdbDoublePoint(TimeConverter.ToDateTime(pt.timestamp), pt.value);
        }

        public static unsafe qdb_ts_blob_point ToNative(this QdbPoint<byte[]> pt, out GCHandle pin)
        {
            pin = GCHandle.Alloc(pt.Value, GCHandleType.Pinned);
            return new qdb_ts_blob_point
            {
                timestamp = TimeConverter.ToTimespec(pt.Time),
                content = (void*) pin.AddrOfPinnedObject(),
                content_size = (UIntPtr)pt.Value.Length
            };
        }

        public static unsafe QdbBlobPoint ToManaged(this qdb_ts_blob_point pt)
        {
            // TODO: limited to 32-bit
            var content = new byte[(int)pt.content_size];
            Marshal.Copy(new IntPtr(pt.content), content, 0, (int)pt.content_size);
            return new QdbBlobPoint(TimeConverter.ToDateTime(pt.timestamp), content);
        }
    }
}
