// ReSharper disable InconsistentNaming

namespace Quasardb.Native
{
    public enum qdb_err_severity : uint
    {
        unrecoverable = 0x03000000,
        error = 0x02000000,
        warning = 0x01000000,
        info = 0x00000000,

        mask = 0x0f000000
    }
}
