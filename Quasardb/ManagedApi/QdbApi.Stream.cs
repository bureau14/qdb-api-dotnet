﻿using System;
using System.IO;
using Quasardb.NativeApi;

namespace Quasardb.ManagedApi
{
    enum QdbStreamMode
    {
        Open,
        Append
    }

    partial class QdbApi
    {
        public Stream StreamOpen(string alias, QdbStreamMode mode)
        {
            qdb_stream_handle handle;
            var error = qdb_api.qdb_stream_open(_handle, alias, (qdb_stream_mode) mode, out handle);
            QdbExceptionThrower.ThrowIfNeeded(error);
            return new QdbStreamAdapter(handle, mode == QdbStreamMode.Append);
        }

        public void StreamRemove(string alias)
        {
            var error = qdb_api.qdb_stream_remove(_handle, alias);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }
    }
}
