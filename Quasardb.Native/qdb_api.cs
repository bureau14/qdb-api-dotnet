using System;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

using qdb_int_t = System.Int64;
using qdb_uint_t = System.UInt64;
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
            [In] qdb_error error);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static extern qdb_error qdb_close(
            [In] pointer_t handle);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_connect(
            [In] qdb_handle handle,
            [In] [MarshalAs(UnmanagedType.LPStr)] string uri);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_handle qdb_open_tcp();

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern void qdb_release(
            [In] qdb_handle handle,
            [In] pointer_t buffer);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_get_last_error(
            [In] qdb_handle handle,
            [Out] out qdb_error error,
            [Out] out qdb_sized_string message);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_get_type(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out qdb_entry_type entry_type);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_prefix_get(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string prefix,
            [In] qdb_int_t max_count,
            [Out] out pointer_t aliases,
            [Out] out size_t aliases_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_suffix_get(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string prefix,
            [In] qdb_int_t max_count,
            [Out] out pointer_t aliases,
            [Out] out size_t aliases_count);

        #region Options

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_option_set_cluster_public_key(
            [In] qdb_handle handle,
            [In] string public_key);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_option_set_user_credentials(
            [In] qdb_handle handle,
            [In] string user_name,
            [In] string private_key);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_option_set_compression(
            [In] qdb_handle handle,
            [In] qdb_compression compression_level);

        #endregion

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

        #region Functions specific to blob

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_blob_compare_and_swap(
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
        public static extern qdb_error qdb_blob_get(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out pointer_t content,
            [Out] out size_t content_length);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_blob_get_and_remove(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out pointer_t content,
            [Out] out size_t content_length);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_blob_get_and_update(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] new_content,
            [In] size_t new_content_length,
            [In] qdb_time_t expiry_time,
            [Out] out pointer_t old_content,
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

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_blob_scan(
            [In] qdb_handle handle,
            [In] byte[] pattern,
            [In] qdb_size_t pattern_length,
            [In] qdb_int_t max_count,
            [Out] out pointer_t aliases,
            [Out] out size_t aliases_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_blob_scan_regex(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string pattern,
            [In] qdb_int_t max_count,
            [Out] out pointer_t aliases,
            [Out] out size_t aliases_count);

        #endregion

        #region Functions specific to integers

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_int_add(
            [In] qdb_handle handle,
            [In, MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_int_t value,
            [Out] out qdb_int_t result);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_int_get(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out qdb_int_t value);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_int_put(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_int_t value,
            [In] qdb_time_t expiry_time);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_int_update(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_int_t value,
            [In] qdb_time_t expiry_time);

        #endregion

        #region Functions specific to tags

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_attach_tag(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string tag);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_detach_tag(
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
            [Out] out pointer_t aliases,
            [Out] out size_t aliases_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_get_tags(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out pointer_t tags,
            [Out] out size_t size);

        #endregion

        #region Functions specific to batches/transactions

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_init_operations(
            [In, Out] qdb_operation[] operations,
            [In] size_t operation_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_size_t qdb_run_batch(
            [In] qdb_handle handle,
            [In, Out] qdb_operation[] operations,
            [In] size_t operation_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_release(qdb_handle handle,
            [In] [MarshalAs(UnmanagedType.LPArray)] qdb_operation[] operations);

        #endregion

        #region Functions specific to time-series

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_create(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_uint_t shard_size,
            [In] qdb_ts_column_info[] columns,
            [In] qdb_size_t column_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_create_ex(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_uint_t shard_size,
            [In] qdb_ts_column_info_ex[] columns,
            [In] qdb_size_t column_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_insert_columns(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_ts_column_info_ex[] columns,
            [In] qdb_size_t column_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_list_columns(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out pointer_t columns,
            [Out] out size_t column_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_list_columns_ex(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out pointer_t columns,
            [Out] out size_t column_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_shard_size(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out qdb_uint_t shard_size);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_blob_insert(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string column,
            [In] qdb_ts_blob_point[] values,
            [In] qdb_size_t count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_double_insert(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string column,
            [In] qdb_ts_double_point[] points,
            [In] qdb_size_t count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_int64_insert(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string column,
            [In] qdb_ts_int64_point[] points,
            [In] qdb_size_t count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_string_insert(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string column,
            [In] qdb_ts_string_point[] points,
            [In] qdb_size_t count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_timestamp_insert(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string column,
            [In] qdb_ts_timestamp_point[] points,
            [In] qdb_size_t count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_symbol_insert(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string column,
            [In] qdb_ts_symbol_point[] points,
            [In] qdb_size_t count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_blob_insert_truncate(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string column,
            [In] qdb_ts_range[] ranges,
            [In] qdb_size_t range_count,
            [In] qdb_ts_blob_point[] values,
            [In] qdb_size_t count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_double_insert_truncate(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string column,
            [In] qdb_ts_range[] ranges,
            [In] qdb_size_t range_count,
            [In] qdb_ts_double_point[] points,
            [In] qdb_size_t count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_int64_insert_truncate(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string column,
            [In] qdb_ts_range[] ranges,
            [In] qdb_size_t range_count,
            [In] qdb_ts_int64_point[] points,
            [In] qdb_size_t count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_string_insert_truncate(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string column,
            [In] qdb_ts_range[] ranges,
            [In] qdb_size_t range_count,
            [In] qdb_ts_string_point[] points,
            [In] qdb_size_t count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_timestamp_insert_truncate(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string column,
            [In] qdb_ts_range[] ranges,
            [In] qdb_size_t range_count,
            [In] qdb_ts_timestamp_point[] points,
            [In] qdb_size_t count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_symbol_insert_truncate(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string column,
            [In] qdb_ts_range[] ranges,
            [In] qdb_size_t range_count,
            [In] qdb_ts_symbol_point[] points,
            [In] qdb_size_t count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_blob_get_ranges(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string column,
            [In, Out] qdb_ts_range[] ranges,
            [In] qdb_size_t range_count,
            [Out] out pointer_t points,
            [Out] out size_t point_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_double_get_ranges(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string column,
            [In, Out] qdb_ts_range[] ranges,
            [In] qdb_size_t range_count,
            [Out] out pointer_t points,
            [Out] out size_t point_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_int64_get_ranges(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string column,
            [In, Out] qdb_ts_range[] ranges,
            [In] qdb_size_t range_count,
            [Out] out pointer_t points,
            [Out] out size_t point_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_string_get_ranges(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string column,
            [In, Out] qdb_ts_range[] ranges,
            [In] qdb_size_t range_count,
            [Out] out pointer_t points,
            [Out] out size_t point_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_timestamp_get_ranges(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string column,
            [In, Out] qdb_ts_range[] ranges,
            [In] qdb_size_t range_count,
            [Out] out pointer_t points,
            [Out] out size_t point_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_symbol_get_ranges(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string column,
            [In, Out] qdb_ts_range[] ranges,
            [In] qdb_size_t range_count,
            [Out] out pointer_t points,
            [Out] out size_t point_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_blob_aggregate(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string column,
            [In, Out] qdb_ts_blob_aggregation[] aggregations,
            [In] qdb_size_t aggregation_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_double_aggregate(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string column,
            [In, Out] qdb_ts_double_aggregation[] aggregations,
            [In] qdb_size_t aggregation_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_int64_aggregate(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string column,
            [In, Out] qdb_ts_int64_aggregation[] aggregations,
            [In] qdb_size_t aggregation_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_string_aggregate(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string column,
            [In, Out] qdb_ts_string_aggregation[] aggregations,
            [In] qdb_size_t aggregation_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_timestamp_aggregate(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string column,
            [In, Out] qdb_ts_timestamp_aggregation[] aggregations,
            [In] qdb_size_t aggregation_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_symbol_aggregate(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string column,
            [In, Out] qdb_ts_symbol_aggregation[] aggregations,
            [In] qdb_size_t aggregation_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_get_timestamps(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string column,
            [In] qdb_ts_range[] ranges,
            [In] qdb_size_t range_count,
            [Out] out pointer_t timestamps,
            [Out] out size_t timestamp_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_erase_ranges(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] [MarshalAs(ALIAS_TYPE)] string column,
            [In] qdb_ts_range[] ranges,
            [In] qdb_size_t range_count,
            [Out] out qdb_uint_t erased_count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_expire_by_size(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_uint_t size);

        #region Functions specific to local tables

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_local_table_init(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_ts_column_info[] columns,
            [In] qdb_size_t column_count,
            [Out] out pointer_t table);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_row_get_blob_no_copy(
            [In] pointer_t table,
            [In] qdb_size_t column_index,
            [Out] out pointer_t content,
            [Out] out size_t content_length);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_row_get_double(
            [In] pointer_t table,
            [In] qdb_size_t column_index,
            [Out] out double value);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_row_get_int64(
            [In] pointer_t table,
            [In] qdb_size_t column_index,
            [Out] out qdb_int_t value);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_row_get_string(
            [In] pointer_t table,
            [In] qdb_size_t column_index,
            [Out] out pointer_t content,
            [Out] out size_t content_length);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_row_get_string_no_copy(
            [In] pointer_t table,
            [In] qdb_size_t column_index,
            [Out] out pointer_t content,
            [Out] out size_t content_length);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_row_get_symbol(
            [In] pointer_t table,
            [In] qdb_size_t column_index,
            [Out] out pointer_t content,
            [Out] out size_t content_length);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_row_get_symbol_no_copy(
            [In] pointer_t table,
            [In] qdb_size_t column_index,
            [Out] out pointer_t content,
            [Out] out size_t content_length);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_row_get_timestamp(
            [In] pointer_t table,
            [In] qdb_size_t column_index,
            [Out] out qdb_timespec value);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_table_next_row(
            [In] pointer_t table,
            [Out] out qdb_timespec timestamp);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_table_get_ranges(
            [In] pointer_t table,
            [In] qdb_ts_range[] ranges,
            [In] qdb_size_t range_count);

        #endregion

        #region Functions specific to batch tables

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_batch_table_init(
            [In] qdb_handle handle,
            [In] qdb_ts_batch_column_info[] columns,
            [In] qdb_size_t column_count,
            [Out] out pointer_t table);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_batch_start_row(
            [In] pointer_t table,
            [In] qdb_timespec* timestamp);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_batch_row_set_blob(
            [In] pointer_t table,
            [In] qdb_size_t index,
            [In] byte[] content,
            [In] qdb_size_t content_length);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_batch_row_set_double(
            [In] pointer_t table,
            [In] qdb_size_t index,
            [In] double value);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_batch_row_set_int64(
            [In] pointer_t table,
            [In] qdb_size_t index,
            [In] qdb_int_t value);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_batch_row_set_string(
            [In] pointer_t table,
            [In] qdb_size_t index,
            [In] byte[] content,
            [In] qdb_size_t content_length);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_batch_row_set_symbol(
            [In] pointer_t table,
            [In] qdb_size_t index,
            [In] byte[] content,
            [In] qdb_size_t content_length);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_batch_row_set_timestamp(
            [In] pointer_t table,
            [In] qdb_size_t index,
            [In] qdb_timespec* value);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_batch_push(
            [In] pointer_t table);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_batch_push_fast(
            [In] pointer_t table);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_ts_batch_push_async(
            [In] pointer_t table);

        #endregion

        #endregion

        #region Functions specific to queries

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_query(
            [In] qdb_handle handle,
            [In] [MarshalAs(ALIAS_TYPE)] string query,
            [Out] out qdb_query_result* result);

        #endregion
        
        #region Performance trace functions

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_perf_enable_client_tracking(
            [In] qdb_handle handle);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_perf_disable_client_tracking(
            [In] qdb_handle handle);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_perf_get_profiles(
            [In] qdb_handle handle,
            [Out] out qdb_perf_profile* profiles,
            [Out] out qdb_size_t count);

        [DllImport(DLL_NAME, CallingConvention = CALL_CONV)]
        public static extern qdb_error qdb_perf_clear_all_profiles(
            [In] qdb_handle handle);

        #endregion
    }
}
