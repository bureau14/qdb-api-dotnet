using System;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace Quasardb.Interop
{
    static class qdb_api
    {
        const string DLL_NAME = "qdb_api.dll";
        const UnmanagedType ALIAS_TYPE = UnmanagedType.LPStr;

        [DllImport(DLL_NAME)]
        public static extern qdb_handle qdb_open_tcp();

        [DllImport(DLL_NAME)]
        public static extern qdb_error qdb_close(IntPtr handle);

        [DllImport(DLL_NAME)]
        public static extern qdb_error qdb_connect(
            qdb_handle handle, 
            [MarshalAs(UnmanagedType.LPStr)] string uri);

        [DllImport(DLL_NAME)]
        public static extern qdb_error qdb_put(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] content,
            [In] Int64 content_length,
            [In] Int64 expiry_time);

        [DllImport(DLL_NAME)]
        public static extern qdb_error qdb_get(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out qdb_buffer content,
            [Out] out Int64 content_length);

        [DllImport(DLL_NAME)]
        public static extern void qdb_free_buffer(IntPtr handle);
    }
}
