using Quasardb.Exceptions;
using Quasardb.ManagedApi;
using Quasardb.Native;

namespace Quasardb
{
    delegate void MarshalFunc(ref qdb_operation op);
    delegate void UnmarshalFunc(ref qdb_operation op);
    delegate T UnmarshalFunc<out T>(ref qdb_operation op);

    class DelegateOperation : IOperation, IQdbFuture
    {
        qdb_error_t _error;
        string _alias;
        readonly MarshalFunc _marshal;
        readonly UnmarshalFunc _unmarshal;

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
            _alias = op.alias;
        }

        public QdbException Exception => QdbExceptionFactory.Create(_error, alias: _alias);
    }

    class DelegateOperation<T> : IOperation, IQdbFuture<T>
    {
        qdb_error_t _error;
        string _alias;
        T _result;
        readonly MarshalFunc _marshal;
        readonly UnmarshalFunc<T> _unmarshal;

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
            _alias = op.alias;
        }

        public QdbException Exception => QdbExceptionFactory.Create(_error, alias: _alias);

        public T Result
        {
            get
            {
                QdbExceptionThrower.ThrowIfNeeded(_error, alias: _alias);
                return _result;
            }
        }
    }
}
