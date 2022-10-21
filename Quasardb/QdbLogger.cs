using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Quasardb.Exceptions;
using Quasardb.Native;

// ReSharper disable InconsistentNaming

using qdb_size_t = System.UIntPtr;
using pointer_t = System.IntPtr;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Quasardb
{
    /// An interface to the C-API logging capability.
    public sealed class QdbLogger
    {
        private readonly ILogger _logger;
        log_callback _internal_callback;
        qdb_size_t _id = (qdb_size_t)0;
        bool _is_logging = true;

        /// <summary>
        /// Create a new logger
        /// </summary>
        public QdbLogger(LogLevel minimulLevel = LogLevel.Information)
        {
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddLogging((loggingBuilder) => loggingBuilder
                    .SetMinimumLevel(minimulLevel)
                    .AddSimpleConsole(options =>
                    {
                        options.IncludeScopes = true;
                        options.SingleLine = true;
                    }))
                .BuildServiceProvider();

            _logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<QdbLogger>();

            _internal_callback = log_callback_impl;
            var err = qdb_api.qdb_log_add_callback(_internal_callback, out _id);
            QdbExceptionThrower.ThrowIfNeeded(err);
        }

        /// <summary>
        /// Stops the logger
        /// </summary>
        public void Stop()
        {
            _is_logging = false;
        }

        /// <summary>
        /// Deletes a logger with a specific id
        /// </summary>
        ~QdbLogger()
        {
            qdb_api.qdb_log_remove_callback(_id);
        }

        LogLevel to_net_level(qdb_log_level level)
        {
            switch (level)
            {
                case qdb_log_level.qdb_log_detailed:
                    return LogLevel.Information;
                case qdb_log_level.qdb_log_debug:
                    return LogLevel.Debug;
                case qdb_log_level.qdb_log_info:
                    return LogLevel.Information;
                case qdb_log_level.qdb_log_warning:
                    return LogLevel.Warning;
                case qdb_log_level.qdb_log_error:
                    return LogLevel.Error;
                case qdb_log_level.qdb_log_panic:
                    return LogLevel.Critical;
            }
            return LogLevel.None;
        }

        string to_string(uint[] ts)
        {
            return String.Format("{0:D4}-{1:D2}-{2:D2}T{3:D2}:{4:D2}:{5:D2}.{6:D9}", ts[0], ts[1], ts[2], ts[3], ts[4], ts[5], ts[6]);
        }

        private unsafe void log_callback_impl(
            [In] qdb_log_level level,
            [In] uint[] timestamp,
            [In] uint pid,
            [In] uint tid,
            [In] pointer_t msg,
            [In] qdb_size_t msg_len)
        {
            if (_is_logging)
            {
                var message = Marshal.PtrToStringAnsi(msg, (int)msg_len);
                _logger.Log(to_net_level(level), String.Format("{0} {1}", to_string(timestamp), message));
            }
        }
    }
}
