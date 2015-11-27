using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

using qdb_int = System.Int64;
using qdb_time_t = System.Int64;
using size_t = System.IntPtr;

namespace Quasardb.NativeApi
{
    static class qdb_api
    {
        const string DLL_NAME = "qdb_api.dll";
        const UnmanagedType ALIAS_TYPE = UnmanagedType.LPStr;
        const CallingConvention CALL_CONV = CallingConvention.Cdecl;

        static qdb_api()
        {
            var is64 = IntPtr.Size == 8;
            var dllFolder = is64 ? "win64/" : "win32/";
            LoadLibrary(dllFolder + DLL_NAME);
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_close(
            [In] IntPtr handle);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_connect(
            [In] qdb_handle handle, 
            [In] [MarshalAs(UnmanagedType.LPStr)] string uri);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_handle qdb_open_tcp();

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern void qdb_free_buffer(
            [In] qdb_handle handle,
            [In] IntPtr buffer);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_get_type(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out qdb_entry_type entry_type);

        #region Functions common to all entries

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_expires_at(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] long expiry_time);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_expires_from_now(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] long delay);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_get_expiry_time(
            [In] qdb_handle handle, 
            [In] [MarshalAs(ALIAS_TYPE)] string alias, 
            [Out] out long expiryTime);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_remove(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias);

        #endregion

        #region Functions specific to blob entries

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_blob_compare_and_swap(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] new_content,
            [In] size_t new_content_length,
            [In] byte[] comparand,
            [In] size_t comparand_length,
            [In] qdb_time_t expiry_time,
            [Out] out IntPtr original_content,
            [Out] out size_t original_content_length);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_blob_get(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out IntPtr content,
            [Out] out size_t content_length);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_blob_get_and_remove(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out IntPtr content,
            [Out] out size_t content_length);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_blob_get_and_update(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] new_content,
            [In] size_t new_content_length,
            [In] qdb_time_t expiry_time,
            [Out] out IntPtr old_content,
            [Out] out size_t old_content_length);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_blob_put(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] content,
            [In] size_t content_length,
            [In] qdb_time_t expiry_time);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_blob_update(
            [In] qdb_handle handle, 
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] content,
            [In] size_t content_length,
            [In] qdb_time_t expiry_time);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_blob_remove_if(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] comparand,
            [In] size_t comparand_length);

        #endregion

        #region Functions specific to integers

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_int_add(
            [In] qdb_handle handle,
            [In, MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_int value,
            [Out] out qdb_int result);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_int_get(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out qdb_int value);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_int_put(
            [In] qdb_handle handle, 
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_int value,
            [In] qdb_time_t expiry_time);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_int_update(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_int value,
            [In] qdb_time_t expiry_time);

        #endregion

        #region Functions specific to deques

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_deque_back(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out IntPtr buffer,
            [Out] out size_t contentLength);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_deque_front(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out IntPtr buffer,
            [Out] out size_t contentLength);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_deque_pop_back(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out IntPtr buffer,
            [Out] out size_t contentLength);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_deque_pop_front(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out IntPtr buffer,
            [Out] out size_t contentLength);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_deque_push_front(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] content,
            [Out] size_t content_length);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_deque_push_back(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] content,
            [Out] size_t content_length);

        #endregion

        #region Functions specific to sets

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_hset_contains(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] content,
            [In] size_t content_length);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_hset_erase(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] content,
            [In] size_t content_length);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_hset_insert(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] content,
            [In] size_t content_length);

        #endregion

        #region Functions specific to tags

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_add_tag(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string tag);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_remove_tag(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string tag);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_has_tag(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string tag);

        
        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_get_tagged(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string tag,
            [Out] out IntPtr aliases,
            [Out] out size_t aliases_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_get_tags(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias, 
            [Out] out IntPtr pointer,
            [Out] out IntPtr size);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern void qdb_free_results(
            [In] qdb_handle handle,
            [In] IntPtr results,
            [In] size_t results_count
        );

        #endregion

        #region Functions specific to tags

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_stream_open(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_stream_mode mode,
            [Out] out qdb_stream_handle stream);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_stream_close(
            [In] IntPtr stream);

        #endregion
    }
}
