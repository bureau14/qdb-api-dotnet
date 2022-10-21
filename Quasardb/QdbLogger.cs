using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Quasardb.Exceptions;
using Quasardb.Native;

// ReSharper disable InconsistentNaming

using qdb_size_t = System.UIntPtr;
using pointer_t = System.IntPtr;

namespace Quasardb
{
    /// An interface to the C-API logging capability.
    internal sealed class QdbLogger
    {
        readonly qdb_handle _handle;
        log_callback _internal_callback;
        qdb_size_t _id;

        string to_string(qdb_log_level level)
        {
            switch (level)
            {
                case qdb_log_level.qdb_log_detailed:
                    return "[detail]";
                case qdb_log_level.qdb_log_debug:
                    return "[debug]";
                case qdb_log_level.qdb_log_info:
                    return "[info]";
                case qdb_log_level.qdb_log_warning:
                    return "[warning]";
                case qdb_log_level.qdb_log_error:
                    return "[error]";
                case qdb_log_level.qdb_log_panic:
                    return "[panic]";
            }
            return "[unknown]";
        }

        string to_string(pointer_t timestamp)
        {
            byte[] ts = new byte[6];
            Marshal.Copy(timestamp, ts, 0, 6);
            return String.Format("{0}{1}{2}{3}{4}{5}", ts[0], ts[1], ts[2], ts[3], ts[4], ts[5]);
        }

        private unsafe void log_callback_impl(
            [In] qdb_log_level level,
            [In] pointer_t timestamp,
            [In] ulong pid,
            [In] ulong tid,
            [In] byte* msg,
            [In] qdb_size_t msg_len)
        {
            var message = Marshal.PtrToStringAnsi(new IntPtr(msg), (int)msg_len);
            Console.WriteLine(String.Format("{0} {1} {2}", to_string(level), to_string(timestamp), message));
        }

        unsafe QdbLogger(qdb_handle handle)
        {
            _internal_callback = log_callback_impl;
            qdb_api.qdb_log_add_callback(_internal_callback, _id);
        }

        ~QdbLogger()
        {
            qdb_api.qdb_log_remove_callback(_id);
        }
    }
}
