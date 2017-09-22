// ReSharper disable InconsistentNaming

namespace Quasardb.Native
{
    public enum qdb_ts_filter_type : int
    {
        qdb_ts_filter_none = 0,
        qdb_ts_filter_unique = 1,
        qdb_ts_filter_sample = 2,
        qdb_ts_filter_double_inside_range = 3,
        qdb_ts_filter_double_outside_range = 4
    }
}