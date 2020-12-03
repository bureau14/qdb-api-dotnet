// ReSharper disable InconsistentNaming

namespace Quasardb.Native
{
    public enum qdb_ts_column_type : int
    {
        qdb_ts_column_uninitialized = -1,
        qdb_ts_column_double = 0,
        qdb_ts_column_blob = 1,
        qdb_ts_column_int64 = 2,
        qdb_ts_column_timestamp = 3,
        qdb_ts_column_string = 4,
        qdb_ts_column_symbol = 5,
    }
}
