using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using Quasardb.Exceptions;
using Quasardb.Native;

namespace Quasardb
{
    class QdbNativeBatch : SafeHandle
    {
        readonly qdb_handle _handle;
        readonly List<IOperation> _operations;
        readonly qdb_operation[] _batch;

        public QdbNativeBatch(qdb_handle handle, List<IOperation> operations) : base(IntPtr.Zero, true)
        {
            _handle = handle;
            _operations = operations;
            _batch = new qdb_operation[operations.Count];

            var err = qdb_api.qdb_init_operations(_batch, Size);
            QdbExceptionThrower.ThrowIfNeeded(err);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle()
        {
            if (!_handle.IsClosed)
            {
                qdb_api.qdb_release(_handle, _batch);
            }
            return true;
        }

        public override bool IsInvalid
        {
            get { return _handle == null || _handle.IsInvalid; }
        }

        UIntPtr Size => (UIntPtr)_batch.Length;

        public void Run()
        {
            for (var i = 0; i < _operations.Count; i++)
                _operations[i].MarshalTo(ref _batch[i]);

            qdb_api.qdb_run_batch(_handle, _batch, Size);

            for (var i = 0; i < _operations.Count; i++)
                _operations[i].UnmarshalFrom(ref _batch[i]);
        }
    }
}