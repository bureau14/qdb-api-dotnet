using Quasardb.Native;

namespace Quasardb.TimeSeries
{
    sealed unsafe class QdbColumnTypeCollection : qdb_buffer<qdb_ts_column_type>
    {
        internal QdbColumnTypeCollection(qdb_handle handle) : base(handle)
        {
        }

        protected override qdb_ts_column_type Dereference(void* p, ulong i)
        {
            return ((qdb_ts_column_type*) p)[i];
        }
    }
}
