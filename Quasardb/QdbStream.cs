using System;
using System.IO;
using Quasardb.ManagedApi;

namespace Quasardb
{
    public enum QdbStreamMode
    {
        Open,
        Append
    }

    public sealed class QdbStream : QdbEntry
    {


        internal QdbStream(QdbApi api, string alias) : base(api, alias)
        {
        }

        public Stream Open(QdbStreamMode mode)
        {
            return Api.StreamOpen(Alias, (ManagedApi.QdbStreamMode) mode);
        }
    }
}
