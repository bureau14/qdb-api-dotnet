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
using NReco.Logging.File;
using Microsoft.Extensions.Logging.Console;

namespace Quasardb
{
    /// <summary>
    /// A configuration builder to define the properties of the logger.
    /// </summary>
    public class QdbLoggerBuilder
    {
        LogLevel _level = LogLevel.Information;
        string _filename = null;
        bool _append = true;
        bool _is_nano = false;

        /// <summary>
        /// Instantiate a new logger builder
        /// </summary>
        public QdbLoggerBuilder()
        {}

        /// <summary>
        /// Defines the minimum log level
        /// </summary>
        public QdbLoggerBuilder WithLevel(LogLevel level)
        {
            _level = level;
            return this;
        }

        /// <summary>
        /// Defines the logging file
        /// </summary>
        public QdbLoggerBuilder WithFile(string filename, bool append = true)
        {
            _filename = filename;
            _append = append;
            return this;
        }

        /// <summary>
        /// Defines whether the date format should contain nanoseconds
        /// </summary>
        public QdbLoggerBuilder WithNanosecondDate()
        {
            _is_nano = true;
            return this;
        }

        internal LogLevel Level()
        {
            return _level;
        }

        internal string Filename()
        {
            return _filename;
        }

        internal bool Append()
        {
            return _append;
        }

        internal bool IsNano()
        {
            return _is_nano;
        }
    }


    /// An interface to the C-API logging capability.
    internal sealed class QdbLogger
    {
        private readonly ILogger _logger;
        log_callback _internal_callback;
        qdb_size_t _id = (qdb_size_t)0;
        bool _is_logging = true;

        string _date_format = null;

        internal QdbLogger(QdbLoggerBuilder builder)
        {
            _internal_callback = log_callback_impl;
            var err = qdb_api.qdb_log_add_callback(_internal_callback, out _id);
            QdbExceptionThrower.ThrowIfNeeded(err);

            ServiceProvider serviceProvider = new ServiceCollection()
                .AddLogging((loggingBuilder) =>
                {
                    if (builder.Filename() == null)
                    {
                        loggingBuilder
                        .SetMinimumLevel(builder.Level()).AddSimpleConsole(options =>
                        {
                            options.IncludeScopes = true;
                            options.SingleLine = true;
                        });
                    }
                    else
                    {
                        loggingBuilder.AddFile(builder.Filename(), append: builder.Append());
                    }
                })
                .BuildServiceProvider();

            _logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<QdbLogger>();


            if (builder.IsNano())
            {
                _date_format = "{0:D4}-{1:D2}-{2:D2}T{3:D2}:{4:D2}:{5:D2}.{6:D9}";
            }
            else
            {
                _date_format = "{0:D4}-{1:D2}-{2:D2}T{3:D2}:{4:D2}:{5:D2}";
            }
        }

        /// <summary>
        /// Stops the logger
        /// </summary>
        internal void Stop()
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
            return String.Format(_date_format, ts[0], ts[1], ts[2], ts[3], ts[4], ts[5], ts[6]);
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
