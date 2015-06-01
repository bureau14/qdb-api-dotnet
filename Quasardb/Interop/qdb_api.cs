using System;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace Quasardb.Interop
{
    static class qdb_api
    {
        [DllImport("qdb_api.dll")]
        public static extern qdb_handle qdb_open_tcp();

        [DllImport("qdb_api.dll")]
        public static extern qdb_error qdb_close(IntPtr handle);

        [DllImport("qdb_api.dll")]
        public static extern qdb_error qdb_connect(
            qdb_handle handle, 
            [MarshalAs(UnmanagedType.LPStr)] string uri);

        [DllImport("qdb_api.dll")]
        public static extern qdb_error qdb_put(
            [In] qdb_handle handle,
            [In] [MarshalAs(UnmanagedType.LPStr)] string alias,
            [In] byte[] content,
            [In] Int64 content_length,
            [In] Int64 expiry_time);
    }
}
