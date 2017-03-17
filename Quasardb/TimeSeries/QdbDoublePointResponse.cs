using Quasardb.Native;

namespace Quasardb.TimeSeries
{
    sealed unsafe class QdbDoublePointResponse : qdb_buffer<qdb_ts_double_point>
    {
        internal QdbDoublePointResponse(qdb_handle handle) : base(handle)
        {
        }

        protected override unsafe qdb_ts_double_point Dereference(void* p, ulong i)
        {
            return ((qdb_ts_double_point*)p)[i];
        }
    }
}
