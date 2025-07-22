using System;
using Quasardb.Exceptions;
using Quasardb.Native;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

using pointer_t = System.IntPtr;
using size_t = System.UIntPtr;
// ReSharper disable InconsistentNaming

namespace Quasardb.Query
{
    /// <summary>
    /// Holds the callback function and relevant information of a continuous query.
    /// </summary>
    public unsafe sealed class QdbContinuousQuery : SafeHandle
    {
        readonly qdb_handle _handle;
        continuous_query_callback _internal_callback;
        readonly Func<QdbQueryResult, int> _callback;
        pointer_t _cont_handle;

        private int internal_query_callback(
            [In] pointer_t p,
            [In] qdb_error err,
            [In] qdb_query_result* result)
        {
            if (err == qdb_error.qdb_e_ok)
            {
                qdb_query_result* result_copy = (qdb_query_result*)IntPtr.Zero;
                qdb_api.qdb_query_copy_results(_handle, result, out result_copy);
                var res = new QdbQueryResult(_handle, result_copy);
                return _callback(res);
            }
            return 1;
        }

        /// <summary>
        /// Specifies the continuous query mode.
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// Full will return all values at every call.
            /// </summary>
            Full = qdb_query_continuous_mode.qdb_query_continuous_full,
            /// <summary>
            /// NewValuesOnly will only deliver updated values.
            /// </summary>
            NewValuesOnly = qdb_query_continuous_mode.qdb_query_continuous_new_values_only,
        }

        internal unsafe QdbContinuousQuery(qdb_handle handle, string query, Mode mode, TimeSpan refresh_rate, Func<QdbQueryResult, int> callback) : base(IntPtr.Zero, true)
        {
            _handle = handle;
            // We need to create an internal reference in order to keep the callbacks alive.
            _callback = callback;
            _internal_callback = internal_query_callback;

            int refresh_rate_ms = (int)refresh_rate.TotalMilliseconds;
            var error = qdb_api.qdb_query_continuous(_handle, query, (qdb_query_continuous_mode)mode, refresh_rate_ms, _internal_callback, IntPtr.Zero, out _cont_handle);
            QdbExceptionThrower.ThrowIfNeededWithMsg(_handle, error);
        }

        /// <inheritdoc />
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle()
        {
            if (!_handle.IsClosed)
            {
                qdb_api.qdb_release(_handle, _cont_handle);
                // TODO(vianney) : once it's decided what we do with the result
                // we might want to release it
                // qdb_api.qdb_release(_handle, new IntPtr(_result));
            }
            return true;
        }

        /// <inheritdoc />
        public override bool IsInvalid
        {
            get { return _handle == null || _handle.IsInvalid; }
        }
    }
}