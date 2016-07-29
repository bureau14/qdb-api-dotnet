// ReSharper disable InconsistentNaming

namespace Quasardb.NativeApi
{
    enum qdb_err_origin_t : uint
    {
        qdb_e_origin_system_remote = 0xf0000000,
        qdb_e_origin_system_local = 0xe0000000,
        qdb_e_origin_connection = 0xd0000000,
        qdb_e_origin_input = 0xc0000000,
        qdb_e_origin_operation = 0xb0000000,
        qdb_e_origin_protocol = 0xa0000000,

        qdb_e_origin_mask = 0xf0000000
    }
}
