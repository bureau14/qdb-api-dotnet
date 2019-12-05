using System;
using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

using qdb_size_t = System.UIntPtr;

namespace Quasardb.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct qdb_query_result
    {
        public qdb_sized_string* column_names;
        public qdb_size_t column_count;
        public qdb_point_result** rows;
        public qdb_size_t row_count;
        public qdb_size_t scanned_point_count;
        public qdb_sized_string error_message;
    }
}
