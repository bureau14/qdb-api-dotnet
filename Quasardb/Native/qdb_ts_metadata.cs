using System;
using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

using qdb_size_t = System.UIntPtr;

namespace Quasardb.Native
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal unsafe struct qdb_ts_metadata
    {
        internal qdb_ts_column_info_ex* columns;
        internal qdb_size_t column_count;
        internal qdb_duration shard_size;
        internal qdb_duration ttl;
        internal qdb_aggregated_table* aggregated;
    }
}
