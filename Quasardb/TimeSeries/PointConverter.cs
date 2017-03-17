using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
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

        public static QdbPoint<double> ToManaged(this qdb_ts_double_point pt)
        {
            return new QdbPoint<double>(TimeConverter.ToDateTime(pt.timestamp), pt.value);
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

        public static QdbPoint<byte[]> ToManaged(this qdb_ts_blob_point pt)
        {
            return new QdbPoint<byte[]>(TimeConverter.ToDateTime(pt.timestamp), null/*pt.value*/);
        }
    }
}
