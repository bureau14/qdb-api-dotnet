using System;
using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

using qdb_int_t = System.Int64;
using qdb_time_t = System.Int64;
using size_t = System.UIntPtr;
using qdb_size_t = System.UIntPtr;

namespace Quasardb.Native
{
    [StructLayout(LayoutKind.Sequential, Size = 56, CharSet = CharSet.Ansi)]
    internal struct qdb_operation
    {
        internal qdb_operation_type type;
        internal string alias;
        internal qdb_error error;
        internal qdb_operation_args args;
    };

    [StructLayout(LayoutKind.Explicit)]
    internal struct qdb_operation_args
    {
        [FieldOffset(0)]
        internal qdb_blob_get_args blob_get;

        [FieldOffset(0)]
        internal qdb_blob_set_args blob_set;

        [FieldOffset(0)]
        internal qdb_blob_cas_args blob_cas;

        [FieldOffset(0)]
        internal qdb_blob_get_and_update_args blob_get_and_update;

        [FieldOffset(0)]
        internal qdb_int_get_args int_get;

        [FieldOffset(0)]
        internal qdb_int_add_args int_add;

        [FieldOffset(0)]
        internal qdb_int_set_args int_set;

        [FieldOffset(0)]
        internal qdb_has_tag_args has_tag;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct qdb_blob_get_args
    {
        internal IntPtr content;
        internal qdb_size_t content_size;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct qdb_blob_set_args
    {
        internal IntPtr content;
        internal qdb_size_t content_size;
        internal qdb_time_t expiry;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct qdb_blob_cas_args
    {
        internal IntPtr original_content;
        internal qdb_size_t original_content_size;
        internal IntPtr new_content;
        internal qdb_size_t new_content_size;
        internal IntPtr comparand;
        internal qdb_size_t comparand_size;
        internal qdb_size_t comparand_offset;
        internal qdb_time_t expiry;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct qdb_blob_get_and_update_args
    {
        internal IntPtr original_content;
        internal qdb_size_t original_content_size;
        internal IntPtr new_content;
        internal qdb_size_t new_content_size;
        internal qdb_time_t expiry;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct qdb_int_get_args
    {
        internal qdb_int_t result;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct qdb_int_add_args
    {
        internal qdb_int_t result;
        internal qdb_int_t addend;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct qdb_int_set_args
    {
        internal qdb_int_t value;
        internal qdb_time_t expiry;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct qdb_has_tag_args
    {
        internal IntPtr tag;
    }
}
