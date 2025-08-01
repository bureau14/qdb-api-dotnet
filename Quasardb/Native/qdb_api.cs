﻿using System;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using pointer_t = System.IntPtr;
using qdb_int_t = System.Int64;
using qdb_size_t = System.UIntPtr;
using qdb_time_t = System.Int64;
using qdb_uint_t = System.UInt64;
using size_t = System.UIntPtr;
using qdb_char_ptr_ptr = System.IntPtr;
using qdb_reader_handle = System.IntPtr;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

namespace Quasardb.Native
{
    using qdb_bulk_reader_table_data = qdb_exp_batch_push_table_data;

    [SuppressUnmanagedCodeSecurity]
    internal static unsafe class qdb_api
    {
        const string LIB_NAME = "qdb_api";
        const int RTLD_NOW = 2; // for dlopen's flags
        const int RTLD_GLOBAL = 8;


        static string get_folder_from_codebase()
        {
            return Path.GetDirectoryName(new Uri(typeof(qdb_api).Assembly.CodeBase).LocalPath);
        }

        static string get_folder_from_location()
        {
            return Path.GetDirectoryName(new Uri(typeof(qdb_api).Assembly.Location).LocalPath);
        }

        static string make_libname(bool is_linux)
        {
            string prefix = is_linux ? "lib" : "";
            string ext = is_linux ? "so" : "dll";
            return prefix + LIB_NAME + "." + ext;
        }

        static string make_fullpath(string folder, bool is_linux)
        {
            if (is_linux)
            {
                folder = Path.Combine(folder, "linux");
            }
            else
            {
                folder = Path.Combine(folder, (Environment.Is64BitProcess ? "win64" : "win32"));
            }
            return Path.Combine(folder, make_libname(is_linux));
        }

        internal class linux_loader
        {
            internal class dl
            {
                [DllImport("libdl.so")]
                static public extern IntPtr dlopen(string filename, int flags);
            }

            internal class dl_backup
            {
                [DllImport("libdl.so.2")]
                static public extern IntPtr dlopen(string filename, int flags);
            }

            static public void load(string library)
            {
                try
                {
                    dl.dlopen(library, RTLD_NOW | RTLD_GLOBAL);
                }
                catch
                {
                    dl_backup.dlopen(library, RTLD_NOW | RTLD_GLOBAL);
                }
            }
        }

        internal class win_loader
        {
            [DllImport("kernel32.dll")]
            private static extern pointer_t LoadLibrary(string dllToLoad);
            static public void load(string library)
            {
                LoadLibrary(library);
            }
        }

        static void load_library(string library, bool is_linux)
        {

            if (is_linux)
            {
                linux_loader.load(library);
            }
            else
            {
                win_loader.load(library);
            }
        }

        const UnmanagedType ALIAS_TYPE = UnmanagedType.LPStr;
        internal const CallingConvention CALL_CONV = CallingConvention.Cdecl;
        static qdb_api()
        {
            bool is_linux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            try
            {
                // try with assembly codebase
                string library = make_fullpath(get_folder_from_codebase(), is_linux);
                load_library(library, is_linux);
            }
            catch
            {
                try
                {
                    // try with assembly location
                    string library = make_fullpath(get_folder_from_location(), is_linux);
                    load_library(library, is_linux);
                }
                catch
                {
                    throw new Exception(String.Format("Could not open dll at {0}, nor {1}", make_fullpath(get_folder_from_codebase(), is_linux), make_fullpath(get_folder_from_location(), is_linux)));
                }
            }
        }

        [DllImport("libdl.so")]
        static extern IntPtr dlopen(string filename, int flags);

        [DllImport(LIB_NAME)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ConstCharPtrMarshaler))]
        internal static extern string qdb_error(
            [In] qdb_error error);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal static extern qdb_error qdb_close(
            [In] pointer_t handle);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_connect(
            [In] qdb_handle handle,
            [In][MarshalAs(UnmanagedType.LPStr)] string uri);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_handle qdb_open_tcp();

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern void qdb_release(
            [In] qdb_handle handle,
            [In] pointer_t buffer);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_get_last_error(
            [In] qdb_handle handle,
            [Out] out qdb_error error,
            [Out] out qdb_sized_string* message);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_get_type(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out qdb_entry_type entry_type);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_prefix_get(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string prefix,
            [In] qdb_int_t max_count,
            [Out] out pointer_t aliases,
            [Out] out size_t aliases_count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_suffix_get(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string prefix,
            [In] qdb_int_t max_count,
            [Out] out pointer_t aliases,
            [Out] out size_t aliases_count);

        #region Options

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_option_set_cluster_public_key(
            [In] qdb_handle handle,
            [In] string public_key);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_option_set_user_credentials(
            [In] qdb_handle handle,
            [In] string user_name,
            [In] string private_key);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_option_set_compression(
            [In] qdb_handle handle,
            [In] qdb_compression compression_level);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_option_set_client_max_in_buf_size(
            [In] qdb_handle handle,
            [In] qdb_int_t max_size);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_option_set_client_max_parallelism(
            [In] qdb_handle handle,
            [In] qdb_int_t thread_count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_option_set_stabilization_max_wait(
            [In] qdb_handle handle,
            [In] int wait_ms);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_option_cluster_tidy_memory(
            [In] qdb_handle handle);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_option_set_client_soft_memory_limit(
            [In] qdb_handle handle,
            [In] qdb_uint_t limit);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_option_client_get_memory_info(
            [In] qdb_handle handle,
            [Out] out pointer_t content,
            [Out] out size_t content_length);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_option_client_tidy_memory(
            [In] qdb_handle handle);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_option_set_timezone(
            [In] qdb_handle handle,
            [In] string timezone);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_option_get_timezone(
            [In] qdb_handle handle,
            [Out] out pointer_t timezone);

        #endregion

        #region Functions common to all entries

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_expires_at(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In] long expiry_time);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_expires_from_now(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In] long delay);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_get_expiry_time(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out long expiryTime);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_remove(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias);

        #endregion

        #region Functions specific to blob

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_blob_compare_and_swap(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] new_content,
            [In] size_t new_content_length,
            [In] byte[] comparand,
            [In] size_t comparand_length,
            [In] qdb_time_t expiry_time,
            [Out] out pointer_t original_content,
            [Out] out size_t original_content_length);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_blob_get(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out pointer_t content,
            [Out] out size_t content_length);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_blob_get_and_remove(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out pointer_t content,
            [Out] out size_t content_length);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_blob_get_and_update(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] new_content,
            [In] size_t new_content_length,
            [In] qdb_time_t expiry_time,
            [Out] out pointer_t old_content,
            [Out] out size_t old_content_length);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_blob_put(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] content,
            [In] size_t content_length,
            [In] qdb_time_t expiry_time);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_blob_update(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] content,
            [In] size_t content_length,
            [In] qdb_time_t expiry_time);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_blob_remove_if(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In] byte[] comparand,
            [In] size_t comparand_length);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_blob_scan(
            [In] qdb_handle handle,
            [In] byte[] pattern,
            [In] qdb_size_t pattern_length,
            [In] qdb_int_t max_count,
            [Out] out pointer_t aliases,
            [Out] out size_t aliases_count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_blob_scan_regex(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string pattern,
            [In] qdb_int_t max_count,
            [Out] out pointer_t aliases,
            [Out] out size_t aliases_count);

        #endregion

        #region Functions specific to string

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_string_compare_and_swap(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string new_value,
            [In] size_t new_value_length,
            [In][MarshalAs(ALIAS_TYPE)] string comparand,
            [In] size_t comparand_length,
            [In] qdb_time_t expiry_time,
            [Out] out pointer_t original_value,
            [Out] out size_t original_value_length);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_string_get(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out pointer_t content,
            [Out] out size_t content_length);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_string_get_and_remove(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out pointer_t content,
            [Out] out size_t content_length);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_string_get_and_update(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string update_content,
            [In] size_t update_content_length,
            [In] qdb_time_t expiry_time,
            [Out] out pointer_t get_content,
            [Out] out size_t get_content_length);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_string_put(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string content,
            [In] size_t content_length,
            [In] qdb_time_t expiry_time);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_string_update(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string content,
            [In] size_t content_length,
            [In] qdb_time_t expiry_time);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_string_remove_if(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string comparand,
            [In] size_t comparand_length);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_string_scan(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string pattern,
            [In] qdb_size_t pattern_length,
            [In] qdb_int_t max_count,
            [Out] out pointer_t aliases,
            [Out] out size_t alias_count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_string_scan_regex(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string pattern,
            [In] qdb_int_t max_count,
            [Out] out pointer_t aliases,
            [Out] out size_t alias_count);

        #endregion

        #region Functions specific to integers

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_int_add(
            [In] qdb_handle handle,
            [In, MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_int_t value,
            [Out] out qdb_int_t result);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_int_get(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out qdb_int_t value);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_int_put(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_int_t value,
            [In] qdb_time_t expiry_time);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_int_update(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_int_t value,
            [In] qdb_time_t expiry_time);

        #endregion

        #region Functions specific to double

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_double_add(
            [In] qdb_handle handle,
            [In, MarshalAs(ALIAS_TYPE)] string alias,
            [In] double value,
            [Out] out double result);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_double_get(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out double value);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_double_put(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In] double value,
            [In] qdb_time_t expiry_time);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_double_update(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In] double value,
            [In] qdb_time_t expiry_time);

        #endregion

        #region Functions specific to timestamps

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_timestamp_put(
            [In] qdb_handle handle,
            [In, MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_timespec* value,
            [In] qdb_time_t expiry_time);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_timestamp_update(
            [In] qdb_handle handle,
            [In, MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_timespec* value,
            [In] qdb_time_t expiry_time);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_timestamp_get(
            [In] qdb_handle handle,
            [In, MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out qdb_timespec value);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_timestamp_add(
            [In] qdb_handle handle,
            [In, MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_timespec* addend,
            [Out] out qdb_timespec result);

        #endregion

        #region Functions specific to tags

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_attach_tag(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string tag);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_detach_tag(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string tag);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_has_tag(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string tag);


        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_get_tagged(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string tag,
            [Out] out pointer_t aliases,
            [Out] out size_t aliases_count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_get_tags(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out pointer_t tags,
            [Out] out size_t size);

        #endregion

        #region Functions specific to batches/transactions

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_init_operations(
            [In, Out] qdb_operation[] operations,
            [In] size_t operation_count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_size_t qdb_run_batch(
            [In] qdb_handle handle,
            [In, Out] qdb_operation[] operations,
            [In] size_t operation_count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_release(qdb_handle handle,
            [In][MarshalAs(UnmanagedType.LPArray)] qdb_operation[] operations);

        #endregion

        #region Functions specific to time-series

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_create(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_uint_t shard_size,
            [In] qdb_ts_column_info[] columns,
            [In] qdb_size_t column_count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_create_ex(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_uint_t shard_size,
            [In] qdb_ts_column_info_ex[] columns,
            [In] qdb_size_t column_count,
            [In] qdb_time_t ttl);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_insert_columns(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_ts_column_info[] columns,
            [In] qdb_size_t column_count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_insert_columns_ex(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_ts_column_info_ex[] columns,
            [In] qdb_size_t column_count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_list_columns(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out pointer_t columns,
            [Out] out size_t column_count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_list_columns_ex(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out pointer_t columns,
            [Out] out size_t column_count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_get_metadata(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [Out] out qdb_ts_metadata* metadata);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_blob_insert(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string column,
            [In] qdb_ts_blob_point[] values,
            [In] qdb_size_t count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_double_insert(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string column,
            [In] qdb_ts_double_point[] points,
            [In] qdb_size_t count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_int64_insert(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string column,
            [In] qdb_ts_int64_point[] points,
            [In] qdb_size_t count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_string_insert(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string column,
            [In] qdb_ts_string_point[] points,
            [In] qdb_size_t count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_timestamp_insert(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string column,
            [In] qdb_ts_timestamp_point[] points,
            [In] qdb_size_t count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_blob_insert_truncate(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string column,
            [In] qdb_ts_range[] ranges,
            [In] qdb_size_t range_count,
            [In] qdb_ts_blob_point[] values,
            [In] qdb_size_t count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_double_insert_truncate(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string column,
            [In] qdb_ts_range[] ranges,
            [In] qdb_size_t range_count,
            [In] qdb_ts_double_point[] points,
            [In] qdb_size_t count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_int64_insert_truncate(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string column,
            [In] qdb_ts_range[] ranges,
            [In] qdb_size_t range_count,
            [In] qdb_ts_int64_point[] points,
            [In] qdb_size_t count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_string_insert_truncate(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string column,
            [In] qdb_ts_range[] ranges,
            [In] qdb_size_t range_count,
            [In] qdb_ts_string_point[] points,
            [In] qdb_size_t count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_timestamp_insert_truncate(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string column,
            [In] qdb_ts_range[] ranges,
            [In] qdb_size_t range_count,
            [In] qdb_ts_timestamp_point[] points,
            [In] qdb_size_t count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_blob_get_ranges(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string column,
            [In, Out] qdb_ts_range[] ranges,
            [In] qdb_size_t range_count,
            [Out] out pointer_t points,
            [Out] out size_t point_count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_double_get_ranges(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string column,
            [In, Out] qdb_ts_range[] ranges,
            [In] qdb_size_t range_count,
            [Out] out pointer_t points,
            [Out] out size_t point_count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_int64_get_ranges(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string column,
            [In, Out] qdb_ts_range[] ranges,
            [In] qdb_size_t range_count,
            [Out] out pointer_t points,
            [Out] out size_t point_count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_string_get_ranges(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string column,
            [In, Out] qdb_ts_range[] ranges,
            [In] qdb_size_t range_count,
            [Out] out pointer_t points,
            [Out] out size_t point_count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_timestamp_get_ranges(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string column,
            [In, Out] qdb_ts_range[] ranges,
            [In] qdb_size_t range_count,
            [Out] out pointer_t points,
            [Out] out size_t point_count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_blob_aggregate(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string column,
            [In, Out] qdb_ts_blob_aggregation[] aggregations,
            [In] qdb_size_t aggregation_count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_double_aggregate(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string column,
            [In, Out] qdb_ts_double_aggregation[] aggregations,
            [In] qdb_size_t aggregation_count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_int64_aggregate(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string column,
            [In, Out] qdb_ts_int64_aggregation[] aggregations,
            [In] qdb_size_t aggregation_count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_string_aggregate(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string column,
            [In, Out] qdb_ts_string_aggregation[] aggregations,
            [In] qdb_size_t aggregation_count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_timestamp_aggregate(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string column,
            [In, Out] qdb_ts_timestamp_aggregation[] aggregations,
            [In] qdb_size_t aggregation_count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_get_timestamps(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string column,
            [In] qdb_ts_range[] ranges,
            [In] qdb_size_t range_count,
            [Out] out pointer_t timestamps,
            [Out] out size_t timestamp_count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_erase_ranges(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In][MarshalAs(ALIAS_TYPE)] string column,
            [In] qdb_ts_range[] ranges,
            [In] qdb_size_t range_count,
            [Out] out qdb_uint_t erased_count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_expire_by_size(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_uint_t size);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV), SuppressUnmanagedCodeSecurity]
        internal static extern qdb_error qdb_exp_batch_push(
            [In] qdb_handle handle,
            [In] qdb_exp_batch_push_mode mode,
            [In] qdb_exp_batch_push_table* tables,
            [In] qdb_exp_batch_push_table_schema* schemas,
            [In] long table_count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV), SuppressUnmanagedCodeSecurity]
        internal static extern qdb_error qdb_exp_batch_push_with_options(
            [In] qdb_handle handle,
            [In] qdb_exp_batch_options* options,
            [In] qdb_exp_batch_push_table* tables,
            [In] qdb_exp_batch_push_table_schema* schemas,
            [In] long table_count);

        #region Functions specific to bulk reader

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV), SuppressUnmanagedCodeSecurity]
        internal static extern qdb_error qdb_bulk_reader_fetch(
            [In] qdb_handle handle,
            [In] qdb_char_ptr_ptr columns,
            [In] qdb_size_t column_count,
            [In] qdb_bulk_reader_table* tables,
            [In] qdb_size_t table_count,
            [Out] out qdb_reader_handle reader);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV), SuppressUnmanagedCodeSecurity]
        internal static extern qdb_error qdb_bulk_reader_get_data(
            [In] qdb_reader_handle reader,
            [Out] out qdb_bulk_reader_table_data* data,
            [In] qdb_size_t rows_to_get);

        #endregion

        #region Functions specific to local tables

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_local_table_init(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string alias,
            [In] qdb_ts_column_info[] columns,
            [In] qdb_size_t column_count,
            [Out] out pointer_t table);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_row_get_blob_no_copy(
            [In] pointer_t table,
            [In] qdb_size_t column_index,
            [Out] out pointer_t content,
            [Out] out size_t content_length);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_row_get_double(
            [In] pointer_t table,
            [In] qdb_size_t column_index,
            [Out] out double value);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_row_get_int64(
            [In] pointer_t table,
            [In] qdb_size_t column_index,
            [Out] out qdb_int_t value);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_row_get_string(
            [In] pointer_t table,
            [In] qdb_size_t column_index,
            [Out] out pointer_t content,
            [Out] out size_t content_length);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_row_get_string_no_copy(
            [In] pointer_t table,
            [In] qdb_size_t column_index,
            [Out] out pointer_t content,
            [Out] out size_t content_length);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_row_get_timestamp(
            [In] pointer_t table,
            [In] qdb_size_t column_index,
            [Out] out qdb_timespec value);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_table_next_row(
            [In] pointer_t table,
            [Out] out qdb_timespec timestamp);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_table_get_ranges(
            [In] pointer_t table,
            [In] qdb_ts_range[] ranges,
            [In] qdb_size_t range_count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_table_stream_ranges(
            [In] pointer_t table,
            [In] qdb_ts_range[] ranges,
            [In] qdb_size_t range_count);

        #endregion

        #region Functions specific to batch tables

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_batch_table_init(
            [In] qdb_handle handle,
            [In] qdb_ts_batch_column_info[] columns,
            [In] qdb_size_t column_count,
            [Out] out pointer_t table);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_batch_start_row(
            [In] pointer_t table,
            [In] qdb_timespec* timestamp);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_batch_row_set_blob(
            [In] pointer_t table,
            [In] qdb_size_t index,
            [In] byte[] content,
            [In] qdb_size_t content_length);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_batch_row_set_double(
            [In] pointer_t table,
            [In] qdb_size_t index,
            [In] double value);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_batch_row_set_int64(
            [In] pointer_t table,
            [In] qdb_size_t index,
            [In] qdb_int_t value);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_batch_row_set_string(
            [In] pointer_t table,
            [In] qdb_size_t index,
            [In] byte[] content,
            [In] qdb_size_t content_length);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_batch_row_set_timestamp(
            [In] pointer_t table,
            [In] qdb_size_t index,
            [In] qdb_timespec* value);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_batch_push(
            [In] pointer_t table);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_batch_push_fast(
            [In] pointer_t table);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_ts_batch_push_async(
            [In] pointer_t table);

        #endregion

        #endregion

        #region Functions specific to queries

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_query(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string query,
            [Out] out qdb_query_result* result);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_query_copy_results(
            [In] qdb_handle handle,
            [In] qdb_query_result* result,
            [Out] out qdb_query_result* result_copy);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_query_continuous(
            [In] qdb_handle handle,
            [In][MarshalAs(ALIAS_TYPE)] string query,
            [In] qdb_query_continuous_mode mode,
            [In] int refresh_rate_ms,
            [In][MarshalAs(UnmanagedType.FunctionPtr)] continuous_query_callback callback,
            [In] pointer_t cb_context,
            [Out] out pointer_t cont_handle);

        #endregion

        #region Performance trace functions

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_perf_enable_client_tracking(
            [In] qdb_handle handle);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_perf_disable_client_tracking(
            [In] qdb_handle handle);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_perf_get_profiles(
            [In] qdb_handle handle,
            [Out] out qdb_perf_profile* profiles,
            [Out] out qdb_size_t count);

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_perf_clear_all_profiles(
            [In] qdb_handle handle);

        #endregion
        
        #region Logging functions

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_log_add_callback(
            [In] log_callback cb,
            [Out] out qdb_size_t id
        );

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern qdb_error qdb_log_remove_callback(
            [In] qdb_size_t id
        );


        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern int qdb_log_option_is_sync();

        [DllImport(LIB_NAME, CallingConvention = CALL_CONV)]
        internal static extern void qdb_log_option_set_sync([In] int sync_logger);

        #endregion
    }
}
