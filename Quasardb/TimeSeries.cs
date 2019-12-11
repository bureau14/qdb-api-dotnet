using Quasardb.Native;
using Quasardb.TimeSeries;
using System;

namespace Quasardb.TimeSeries
{
    /// <summary>
    /// A timeseries
    /// </summary>
    [Obsolete("Use Quasardb.QdbTable instead")]
    public class QdbTimeSeries : QdbTable
    {
        internal QdbTimeSeries(qdb_handle handle, string alias) : base(handle, alias)
        {
        }
    }
}
