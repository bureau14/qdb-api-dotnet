using System;
using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

using qdb_size_t = System.UIntPtr;

namespace Quasardb.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct qdb_point_result_double_payload
    {
        internal double value;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct qdb_point_result_int64_payload
    {
        internal long value;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct qdb_point_result_string_payload
    {
        internal char* content;
        internal qdb_size_t content_size;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct qdb_point_result_symbol_payload
    {
        internal char* content;
        internal qdb_size_t content_size;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct qdb_point_result_blob_payload
    {
        internal void* content;
        internal qdb_size_t content_size;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct qdb_point_result_timestamp_payload
    {
        internal qdb_timespec value;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct qdb_point_result_count_payload
    {
        internal qdb_size_t value;
    }


    [StructLayout(LayoutKind.Explicit)]
    internal struct qdb_point_result
    {
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal qdb_query_result_value_type type;
        [System.Runtime.InteropServices.FieldOffset(8)]
        internal qdb_point_result_double_payload double_payload;
        [System.Runtime.InteropServices.FieldOffset(8)]
        internal qdb_point_result_int64_payload int64_payload;
        [System.Runtime.InteropServices.FieldOffset(8)]
        internal qdb_point_result_blob_payload blob_payload;
        [System.Runtime.InteropServices.FieldOffset(8)]
        internal qdb_point_result_timestamp_payload timestamp_payload;
        [System.Runtime.InteropServices.FieldOffset(8)]
        internal qdb_point_result_count_payload count_payload;
        [System.Runtime.InteropServices.FieldOffset(8)]
        internal qdb_point_result_string_payload string_payload;
        [System.Runtime.InteropServices.FieldOffset(8)]
        internal qdb_point_result_symbol_payload symbol_payload;
    }
}
