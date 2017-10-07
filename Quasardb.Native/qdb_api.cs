using System;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

using qdb_int_t = System.Int64;
using qdb_time_t = System.Int64;
using size_t = System.UIntPtr;
using qdb_size_t = System.UIntPtr;
using pointer_t = System.IntPtr;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

namespace Quasardb.Native
{
    [SuppressUnmanagedCodeSecurity]
    public static unsafe class qdb_api
    {
        const string DLL_NAME = "qdb_api.dll";
        const UnmanagedType ALIAS_TYPE = UnmanagedType.LPStr;
        const CallingConvention CALL_CONV = CallingConvention.Cdecl;

        static qdb_api()
        {
            var is64 = pointer_t.Size == 8;
            var myLocation = new Uri(typeof(qdb_api).Assembly.CodeBase).LocalPath;
            var folder = Path.GetDirectoryName(myLocation);
            var subfolder = is64 ? "\\win64\\" : "\\win32\\";
            LoadLibrary(folder + subfolder + DLL_NAME);
        }

        [DllImport("kernel32.dll")]
        private static extern pointer_t LoadLibrary(string dllToLoad);

        [DllImport(DLL_NAME)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
        public static extern string qdb_error(
            [In] qdb_error_t error);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static extern qdb_error_t qdb_close(
            [In] pointer_t handle);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_connect(
            [In] qdb_handle handle,
            [In] [MarshalAs(UnmanagedType.LPStr)] string uri);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_handle qdb_open_tcp();

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern void qdb_free_buffer(
            [In] qdb_handle handle,
            [In] pointer_t buffer);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern void qdb_free_results(
            [In] qdb_handle handle,
            [In] pointer_t results,
            [In] size_t results_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_get_type(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out qdb_entry_type entry_type);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_prefix_get(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string prefix,
            [In] qdb_int_t max_count,
            [Out] out pointer_t aliases,
            [Out] out size_t aliases_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_suffix_get(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string prefix,
            [In] qdb_int_t max_count,
            [Out] out pointer_t aliases,
            [Out] out size_t aliases_count);

        #region Functions common to all entries

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_expires_at(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] long expiry_time);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_expires_from_now(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] long delay);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_get_expiry_time(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out long expiryTime);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_remove(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias);

        #endregion

        #region Functions specific to blob

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_blob_compare_and_swap(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] new_content,
            [In] size_t new_content_length,
            [In] byte[] comparand,
            [In] size_t comparand_length,
            [In] qdb_time_t expiry_time,
            [Out] out pointer_t original_content,
            [Out] out size_t original_content_length);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_blob_get(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out pointer_t content,
            [Out] out size_t content_length);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_blob_get_and_remove(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out pointer_t content,
            [Out] out size_t content_length);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_blob_get_and_update(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] new_content,
            [In] size_t new_content_length,
            [In] qdb_time_t expiry_time,
            [Out] out pointer_t old_content,
            [Out] out size_t old_content_length);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_blob_put(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] content,
            [In] size_t content_length,
            [In] qdb_time_t expiry_time);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_blob_update(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] content,
            [In] size_t content_length,
            [In] qdb_time_t expiry_time);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_blob_remove_if(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] comparand,
            [In] size_t comparand_length);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_blob_scan(
            [In] qdb_handle handle,
            [In] byte[] pattern,
            [In] qdb_size_t pattern_length,
            [In] qdb_int_t max_count,
            [Out] out pointer_t aliases,
            [Out] out size_t aliases_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_blob_scan_regex(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string pattern,
            [In] qdb_int_t max_count,
            [Out] out pointer_t aliases,
            [Out] out size_t aliases_count);

        #endregion

        #region Functions specific to integers

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_int_add(
            [In] qdb_handle handle,
            [In, MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_int_t value,
            [Out] out qdb_int_t result);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_int_get(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out qdb_int_t value);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_int_put(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_int_t value,
            [In] qdb_time_t expiry_time);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_int_update(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_int_t value,
            [In] qdb_time_t expiry_time);

        #endregion

        #region Functions specific to deques

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_deque_size(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out qdb_size_t size);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_deque_back(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out pointer_t buffer,
            [Out] out size_t contentLength);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_deque_front(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out pointer_t buffer,
            [Out] out size_t contentLength);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_deque_pop_back(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out pointer_t buffer,
            [Out] out size_t contentLength);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_deque_pop_front(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out pointer_t buffer,
            [Out] out size_t contentLength);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_deque_push_front(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] content,
            [Out] size_t content_length);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_deque_push_back(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] content,
            [Out] size_t content_length);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_deque_get_at(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_int_t index,
            [Out] out pointer_t content,
            [Out] out size_t contentLength);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_deque_set_at(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_int_t index,
            [In] byte[] content,
            [Out] size_t content_length);

        #endregion

        #region Functions specific to sets

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_hset_contains(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] content,
            [In] size_t content_length);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_hset_erase(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] content,
            [In] size_t content_length);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_hset_insert(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] content,
            [In] size_t content_length);

        #endregion

        #region Functions specific to tags

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_attach_tag(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string tag);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_detach_tag(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string tag);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_has_tag(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string tag);


        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_get_tagged(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string tag,
            [Out] out pointer_t aliases,
            [Out] out size_t aliases_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_get_tags(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out pointer_t tags,
            [Out] out size_t size);

        #endregion

        #region Functions specific to streams

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_stream_open(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_stream_mode mode,
            [Out] out qdb_stream_handle stream);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static extern qdb_error_t qdb_stream_close(
            [In] pointer_t stream);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_stream_size(
            [In] qdb_stream_handle handle,
            [Out] out ulong size);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_stream_write(
            [In] qdb_stream_handle handle,
            [In] byte* buffer,
            [In] size_t count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_stream_read(
            [In] qdb_stream_handle handle,
            [In] byte* buffer,
            [In,Out] ref size_t size);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_stream_getpos(
            [In] qdb_stream_handle handle,
            [Out] out ulong position);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_stream_setpos(
            [In] qdb_stream_handle handle,
            [In] ulong position);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_stream_truncate(
            [In] qdb_stream_handle handle,
            [In] ulong position);

        #endregion

        #region Functions specific to batches/transactions

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_init_operations(
            [In, Out] qdb_operation[] operations,
            [In] size_t operation_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_size_t qdb_run_batch(
            [In] qdb_handle handle,
            [In, Out] qdb_operation[] operations,
            [In] size_t operation_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error_t qdb_free_operations(qdb_handle handle,
            [In] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] qdb_operation[] operations,
            [In] size_t operation_count);

        #endregion
    }
}
