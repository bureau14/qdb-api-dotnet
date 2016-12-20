// ReSharper disable InconsistentNaming

namespace Quasardb.NativeApi
{
    enum qdb_operation_type : int
    {
        qdb_op_uninitialized = -1,
        qdb_op_blob_get = 0,
        qdb_op_blob_put = 1,
        qdb_op_blob_update = 2,
        qdb_op_blob_cas = 4,
        qdb_op_blob_get_and_update = 5,
        qdb_op_has_tag = 8,
        qdb_op_int_put = 9,
        qdb_op_int_update = 10,
        qdb_op_int_get = 11,
        qdb_op_int_add = 12,
        qdb_op_get_entry_type = 13
    }
}