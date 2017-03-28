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
    public struct qdb_operation
    {
        public qdb_operation_type type;
        public string alias;
        public qdb_error error;
        public qdb_operation_args args;
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct qdb_operation_args
    {
        [FieldOffset(0)]
        public qdb_blob_get_args blob_get;

        [FieldOffset(0)]
        public qdb_blob_set_args blob_set;

        [FieldOffset(0)]
        public qdb_blob_cas_args blob_cas;

        [FieldOffset(0)]
        public qdb_blob_get_and_update_args blob_get_and_update;

        [FieldOffset(0)]
        public qdb_int_get_args int_get;

        [FieldOffset(0)]
        public qdb_int_add_args int_add;

        [FieldOffset(0)]
        public qdb_int_set_args int_set;

        [FieldOffset(0)]
        public qdb_has_tag_args has_tag;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct qdb_blob_get_args
    {
        public IntPtr content;
        public qdb_size_t content_size;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct qdb_blob_set_args
    {
        public IntPtr content;
        public qdb_size_t content_size;
        public qdb_time_t expiry;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct qdb_blob_cas_args
    {
        public IntPtr original_content;
        public qdb_size_t original_content_size;
        public IntPtr new_content;
        public qdb_size_t new_content_size;
        public IntPtr comparand;
        public qdb_size_t comparand_size;
        public qdb_size_t comparand_offset;
        public qdb_time_t expiry;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct qdb_blob_get_and_update_args
    {
        public IntPtr original_content;
        public qdb_size_t original_content_size;
        public IntPtr new_content;
        public qdb_size_t new_content_size;
        public qdb_time_t expiry;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct qdb_int_get_args
    {
        public qdb_int_t result;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct qdb_int_add_args
    {
        public qdb_int_t result;
        public qdb_int_t addend;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct qdb_int_set_args
    {
        public qdb_int_t value;
        public qdb_time_t expiry;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct qdb_has_tag_args
    {
        public IntPtr tag;
    }
}
