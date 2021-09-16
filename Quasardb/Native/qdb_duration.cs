// ReSharper disable InconsistentNaming

namespace Quasardb.Native
{
    internal enum qdb_duration : int
    {
        qdb_d_millisecond = 1,
        qdb_d_second = qdb_d_millisecond * 1000,
        qdb_d_minute = qdb_d_second * 60,
        qdb_d_hour = qdb_d_minute * 60,
        qdb_d_day = qdb_d_hour * 24,
        qdb_d_week = qdb_d_day * 7,

        qdb_d_default_shard_size = qdb_d_day
    }
}