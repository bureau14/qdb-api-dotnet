using System;
using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

using qdb_size_t = System.UIntPtr;

namespace Quasardb.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct qdb_point_result_double_payload
    {
        public double value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct qdb_point_result_int64_payload
    {
        public long value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct qdb_point_result_string_payload
    {
        public char* content;
        public qdb_size_t content_size;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct qdb_point_result_symbol_payload
    {
        public char* content;
        public qdb_size_t content_size;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct qdb_point_result_blob_payload
    {
        public void* content;
        public qdb_size_t content_size;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct qdb_point_result_timestamp_payload
    {
        public qdb_timespec value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct qdb_point_result_count_payload
    {
        public qdb_size_t value;
    }


    [StructLayout(LayoutKind.Explicit)]
    public struct qdb_point_result
    {
        [System.Runtime.InteropServices.FieldOffset(0)]
        public qdb_query_result_value_type type;
        [System.Runtime.InteropServices.FieldOffset(8)]
        public qdb_point_result_double_payload double_payload;
        [System.Runtime.InteropServices.FieldOffset(8)]
        public qdb_point_result_int64_payload int64_payload;
        [System.Runtime.InteropServices.FieldOffset(8)]
        public qdb_point_result_blob_payload blob_payload;
        [System.Runtime.InteropServices.FieldOffset(8)]
        public qdb_point_result_timestamp_payload timestamp_payload;
        [System.Runtime.InteropServices.FieldOffset(8)]
        public qdb_point_result_count_payload count_payload;
        [System.Runtime.InteropServices.FieldOffset(8)]
        public qdb_point_result_string_payload string_payload;
        [System.Runtime.InteropServices.FieldOffset(8)]
        public qdb_point_result_symbol_payload symbol_payload;
    }
}
