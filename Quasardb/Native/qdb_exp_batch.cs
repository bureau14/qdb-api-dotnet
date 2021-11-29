using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

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

    // this matches qdb_exp_batch_push_options_t enum type in ts.h
    internal enum qdb_exp_batch_push_options : int
    {
        standard = 0,
        unique = 1,
    }

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
        internal long* ints;
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
        internal  qdb_ts_range* truncate_ranges;

        //! Field used by \ref qdb_exp_batch_push_truncate. The number of
        //! truncated ranges.
        internal qdb_size_t truncate_range_count;

        internal qdb_exp_batch_push_options options;
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
