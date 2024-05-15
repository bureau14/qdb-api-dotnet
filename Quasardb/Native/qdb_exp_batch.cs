using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

using qdb_int_t = System.Int64;
using qdb_uint_t = System.UInt64;
using qdb_size_t = System.UIntPtr;

namespace Quasardb.Native
{
    // this matches qdb_exp_batch_push_mode_t enum type in ts.h
    internal enum qdb_exp_batch_push_mode : int
    {
        transactional = 0,
        truncate = 1,
        fast = 2,
        async = 3,
    }

    // this matches qdb_exp_batch_push_flags_t enum type in ts.h
    internal enum qdb_exp_batch_push_flags : int
    {
        none = 0,
        write_through = 1,
        asynchronous_client_push = 2,
    }

    // this matches qdb_exp_batch_deduplication_mode_t enum type in ts.h
    internal enum qdb_exp_batch_deduplication_mode : int
    {
        disabled = 0,
        drop = 1,
        upsert = 2,
    }

    internal enum qdb_exp_batch_creation_mode : int
    {
        dont_create = 0,
        create_tables = 1,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct qdb_exp_batch_options
    {
        //! Specifies how the data is pushed.
        internal qdb_exp_batch_push_mode mode;

        //! Flags that apply to every push in the call
        internal qdb_uint_t push_flags;
    };

    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct qdb_exp_batch_push_column_data
    {
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal qdb_timespec* timestamps;
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal qdb_sized_string* strings;
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal qdb_blob* blobs;
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal qdb_int_t* ints;
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal double* doubles;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct qdb_exp_batch_push_column
    {
        //! The column name in UTF-8 format.
        internal qdb_sized_string name;

        //! The column data type, how it's stored client-side.
        internal qdb_ts_column_type data_type;

        internal qdb_exp_batch_push_column_data data;
    };

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct qdb_exp_batch_push_table_data
    {
        //! The number of rows to send.
        internal qdb_size_t row_count;

        //! The number of columns to send.
        internal qdb_size_t column_count;

        //! The rows timestamps.
        internal qdb_timespec* timestamps;

        //! The table columns to send.
        internal qdb_exp_batch_push_column* columns;
    };

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct qdb_exp_batch_push_table
    {
        //! The table name in UTF-8 format.
        internal qdb_sized_string name;

        //! The table data.
        internal qdb_exp_batch_push_table_data data;

        //! Field used by \ref qdb_exp_batch_push_truncate. The ranges
        //! specifying previous data to erase.
        internal qdb_ts_range* truncate_ranges;

        //! Field used by \ref qdb_exp_batch_push_truncate. The number of
        //! truncated ranges.
        internal qdb_size_t truncate_range_count;

        //! Field used for controlling work with duplicated data.
        //! Except of \ref qdb_exp_batch_push_truncate mode.
        internal qdb_exp_batch_deduplication_mode deduplication_mode;

        //! Field used by \ref qdb_exp_batch_option_unique. The column names
        //! array for duplication check. If NULL then all columns will be
        //! checked.
        internal qdb_sized_string* where_duplicate;

        //! Size of \ref where_duplicate array.
        internal qdb_size_t where_duplicate_count;
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct qdb_exp_batch_push_column_schema
    {
        //! The column type, how it's stored server-side.
        internal qdb_ts_column_type column_type;

        //! The column index.
        internal long index;

        //! The column symbol table (for symbol columns).
        internal qdb_sized_string symtable;
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal unsafe struct qdb_exp_batch_push_table_schema
    {
        //! The table shard size.
        internal long shard_size;

        //! The table columns. The column count is given by the associated \ref
        //! qdb_exp_batch_push_table_t, at data.column_count.
        internal qdb_exp_batch_push_column_schema* columns;
    };
}
