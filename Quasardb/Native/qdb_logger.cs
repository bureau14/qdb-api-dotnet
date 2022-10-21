using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using pointer_t = System.IntPtr;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable InconsistentNaming

using qdb_size_t = System.UIntPtr;

namespace Quasardb.Native
{
    internal enum qdb_log_level
    {
        //! Log everything. Very verbose.
        qdb_log_detailed = 100,
        //! Log debug level messages and below
        qdb_log_debug = 200,
        //! Log information level messages and below
        qdb_log_info = 300,
        //! Log warning level messages and below
        qdb_log_warning = 400,
        //! Log error level messages and below
        qdb_log_error = 500,
        //! Log panic level messages and below. Very terse.
        qdb_log_panic = 600,
    }

    [UnmanagedFunctionPointer(qdb_api.CALL_CONV)]
    unsafe delegate void log_callback(
        [In] qdb_log_level level,
        [In] pointer_t timestamp,
        [In] ulong pid,
        [In] ulong tid,
        [In] byte* msg,
        [In] qdb_size_t msg_len);
}