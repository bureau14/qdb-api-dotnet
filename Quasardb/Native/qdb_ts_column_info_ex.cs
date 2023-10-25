using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

namespace Quasardb.Native
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct qdb_ts_column_info_ex
    {
        internal qdb_sized_string name;
        internal qdb_ts_column_type type;
        internal qdb_sized_string symtable;
    };
}
