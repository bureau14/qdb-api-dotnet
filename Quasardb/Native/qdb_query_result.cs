using System;
using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

using qdb_size_t = System.UIntPtr;

namespace Quasardb.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct qdb_query_result
    {
        internal qdb_sized_string* column_names;
        internal qdb_size_t column_count;
        internal qdb_point_result** rows;
        internal qdb_size_t row_count;
        internal qdb_size_t scanned_point_count;
        internal qdb_sized_string error_message;
    }
}
