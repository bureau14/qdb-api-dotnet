using Quasardb.Native;

namespace Quasardb
{
    internal interface IOperation
    {
        void MarshalTo(ref qdb_operation op);
        void UnmarshalFrom(ref qdb_operation op);
    }
}