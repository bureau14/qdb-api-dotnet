using Quasardb.Native;

namespace Quasardb.TimeSeries
{
    sealed unsafe class QdbBlobPointResponse : qdb_buffer<qdb_ts_blob_point>
    {
        internal QdbBlobPointResponse(qdb_handle handle) : base(handle)
        {
        }

        protected override qdb_ts_blob_point Dereference(void* p, ulong i)
        {
            return ((qdb_ts_blob_point*)p)[i];
        }
    }
}
