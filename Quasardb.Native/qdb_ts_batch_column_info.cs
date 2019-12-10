using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

using qdb_size_t = System.UIntPtr;

namespace Quasardb.Native
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct qdb_ts_batch_column_info
    {
        public string timeseries;
        public string column;
        public qdb_size_t elements_count_hint;
    };
}
