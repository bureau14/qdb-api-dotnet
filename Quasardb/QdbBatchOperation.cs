using Quasardb.Exceptions;
using Quasardb.ManagedApi;
using Quasardb.NativeApi;

namespace Quasardb
{
    delegate void MarshalFunc(ref qdb_operation op);
    delegate void UnmarshalFunc(ref qdb_operation op);
    delegate T UnmarshalFunc<out T>(ref qdb_operation op);

    class DelegateOperation : IOperation, IQdbFuture
    {
        private qdb_error_t _error;
        private readonly MarshalFunc _marshal;
        private readonly UnmarshalFunc _unmarshal;

        public DelegateOperation(MarshalFunc marshal, UnmarshalFunc unmarshal)
        {
            _marshal = marshal;
            _unmarshal = unmarshal;
        }

        public void MarshalTo(ref qdb_operation op)
        {
            _marshal(ref op);
        }

        public void UnmarshalFrom(ref qdb_operation op)
        {
            _unmarshal?.Invoke(ref op);
            _error = op.error;
        }

        public QdbException Exception => QdbExceptionFactory.Create(_error);
    }

    class DelegateOperation<T> : IOperation, IQdbFuture<T>
    {
        private qdb_error_t _error;
        private T _result;
        private readonly MarshalFunc _marshal;
        private readonly UnmarshalFunc<T> _unmarshal;

        public DelegateOperation(MarshalFunc marshal, UnmarshalFunc<T> unmarshal)
        {
            _marshal = marshal;
            _unmarshal = unmarshal;
        }

        public void MarshalTo(ref qdb_operation op)
        {
            _marshal(ref op);
        }

        public void UnmarshalFrom(ref qdb_operation op)
        {
            _result = _unmarshal(ref op);
            _error = op.error;
        }

        public QdbException Exception => QdbExceptionFactory.Create(_error);

        public T Result
        {
            get
            {
                QdbExceptionThrower.ThrowIfNeeded(_error);
                return _result;
            }
        }
    }
}
