using System.Runtime.InteropServices;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

namespace Quasardb.Native
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct qdb_ts_column_info_ex
    {
        public string name;
        public qdb_ts_column_type type;
        public string symtable;
    };
}
