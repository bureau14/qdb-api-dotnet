using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using pointer_t = System.IntPtr;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

using qdb_size_t = System.UIntPtr;

namespace Quasardb.Native
{
    internal enum qdb_query_continuous_mode
    {
        qdb_query_continuous_full = 0,
        qdb_query_continuous_new_values_only = 1,
    }

    [UnmanagedFunctionPointer(qdb_api.CALL_CONV)]
    unsafe delegate int continuous_query_callback(
        [In] pointer_t p,
        [In] qdb_error err,
        [In] qdb_query_result* result);
}