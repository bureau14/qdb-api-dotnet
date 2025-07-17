using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

using qdb_size_t = System.UIntPtr;
using qdb_char_ptr = System.IntPtr;
using qdb_char_ptr_ptr = System.IntPtr;

namespace Quasardb.Native
{
    using qdb_reader_handle = System.IntPtr;
    using qdb_bulk_reader_table_data = qdb_exp_batch_push_table_data;

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct qdb_bulk_reader_table
    {
        internal qdb_char_ptr name;
        internal qdb_ts_range* ranges;
        internal qdb_size_t range_count;
    }
}
