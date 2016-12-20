// ReSharper disable InconsistentNaming

namespace Quasardb.NativeApi
{
    enum qdb_err_origin : uint
    {
        system_remote = 0xf0000000,
        system_local = 0xe0000000,
        connection = 0xd0000000,
        input = 0xc0000000,
        operation = 0xb0000000,
        protocol = 0xa0000000,

        mask = 0xf0000000
    }
}
