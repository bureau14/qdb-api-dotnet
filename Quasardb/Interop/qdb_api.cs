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
        public static extern qdb_error qdb_close(
            [In] IntPtr handle);

        [DllImport(DLL_NAME)]
        public static extern qdb_error qdb_connect(
            [In] qdb_handle handle, 
            [In] [MarshalAs(UnmanagedType.LPStr)] string uri);

        [DllImport(DLL_NAME)]
        public static extern qdb_handle qdb_open_tcp();

        #region Functions common to all entries

        [DllImport(DLL_NAME)]
        public static extern qdb_error qdb_expires_at(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] long expiry_time);

        [DllImport(DLL_NAME)]
        public static extern qdb_error qdb_expires_from_now(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] long delay);

        [DllImport(DLL_NAME)]
        public static extern qdb_error qdb_get_expiry_time(
            [In] qdb_handle handle, 
            [In] [MarshalAs(ALIAS_TYPE)] string alias, 
            [Out] out long expiryTime);

        [DllImport(DLL_NAME)]
        public static extern qdb_error qdb_remove(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias);

        #endregion

        #region Functions specific to blob entries

        [DllImport(DLL_NAME)]
        public static extern qdb_error qdb_compare_and_swap(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] new_content,
            [In] long new_content_length,
            [In] byte[] comparand,
            [In] long comparand_length,
            [In] long expiry_time,
            [Out] out qdb_buffer original_content, 
            [Out] out long original_content_length);

        [DllImport(DLL_NAME)]
        public static extern qdb_error qdb_get(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out qdb_buffer content,
            [Out] out long content_length);

        [DllImport(DLL_NAME)]
        public static extern qdb_error qdb_get_and_remove(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out qdb_buffer content,
            [Out] out long content_length);

        [DllImport(DLL_NAME)]
        public static extern qdb_error qdb_get_and_update(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] new_content,
            [In] long new_content_length,
            [In] long expiry_time,
            [Out] out qdb_buffer old_content,
            [Out] out long old_content_length);

        [DllImport(DLL_NAME)]
        public static extern void qdb_free_buffer(
            [In] IntPtr handle);

        [DllImport(DLL_NAME)]
        public static extern qdb_error qdb_put(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] content,
            [In] long content_length,
            [In] long expiry_time);

        [DllImport(DLL_NAME)]
        public static extern qdb_error qdb_update(
            [In] qdb_handle handle, 
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] content,
            [In] long content_length, 
            [In] long expiry_time);

        [DllImport(DLL_NAME)]
        public static extern qdb_error qdb_remove_if(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] comparand,
            [In] long comparand_length);

        #endregion

        #region Functions specific to integers

        [DllImport(DLL_NAME)]
        public static extern qdb_error qdb_int_add(
            [In] qdb_handle handle, 
            [In, MarshalAs(UnmanagedType.LPStr)] string alias,
            [In] long value,
            [Out] out long result);

        [DllImport(DLL_NAME)]
        public static extern qdb_error qdb_int_get(
            [In] qdb_handle handle,
            [In] [MarshalAs(UnmanagedType.LPStr)] string alias, 
            [Out] out long value);

        [DllImport(DLL_NAME)]
        public static extern qdb_error qdb_int_put(
            [In] qdb_handle handle, 
            [In] [MarshalAs(UnmanagedType.LPStr)] string alias,
            [In] long value);

        [DllImport(DLL_NAME)]
        public static extern qdb_error qdb_int_update(
            [In] qdb_handle handle,
            [In] [MarshalAs(UnmanagedType.LPStr)] string alias,
            [In] long value);

        #endregion

        #region Functions specific to queues

        [DllImport(DLL_NAME)]
        public static extern qdb_error qdb_queue_pop_back(
            [In] qdb_handle handle,
            [In] string alias,
            [Out] out qdb_buffer buffer,
            [Out] out long contentLength);

        [DllImport(DLL_NAME)]
        public static extern qdb_error qdb_queue_push_front(
            [In] qdb_handle handle, string alias,
            [In] byte[] content,
            [Out] long content_length);

        [DllImport(DLL_NAME)]
        public static extern qdb_error qdb_queue_push_back(
            [In] qdb_handle handle, string alias,
            [In] byte[] content,
            [Out] long content_length);

        #endregion

    }
}
