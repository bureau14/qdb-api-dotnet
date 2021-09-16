using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

using qdb_size_t = System.UIntPtr;

namespace Quasardb.Native
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct qdb_ts_batch_column_info
    {
        internal string timeseries;
        internal string column;
        internal qdb_size_t elements_count_hint;
    };
}
