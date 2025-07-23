using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

using qdb_size_t = System.UIntPtr;
using qdb_char_ptr = System.IntPtr;

namespace Quasardb.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct qdb_bulk_reader_table
    {
        internal qdb_char_ptr name;
        internal qdb_ts_range* ranges;
        internal qdb_size_t range_count;
    }
}
