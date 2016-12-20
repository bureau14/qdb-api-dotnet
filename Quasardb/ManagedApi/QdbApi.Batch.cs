using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Quasardb.NativeApi;

namespace Quasardb.ManagedApi
{
    internal interface IOperation
    {
        void MarshalTo(ref qdb_operation op);
        void UnmarshalFrom(ref qdb_operation op);
    }

    partial class QdbApi
    {
        public void RunBatch(List<IOperation> batch)
        {
            if (batch.Count == 0) return;

            var operations = new qdb_operation[batch.Count];

            var err = qdb_api.qdb_init_operations(operations, (UIntPtr) operations.Length);
            QdbExceptionThrower.ThrowIfNeeded(err);
            
            for (var i = 0; i < batch.Count; i++)
            {
                batch[i].MarshalTo(ref operations[i]);
            }
            qdb_api.qdb_run_batch(_handle, operations,  (UIntPtr) operations.Length);
            for (var i = 0; i < batch.Count; i++)
            {
                batch[i].UnmarshalFrom(ref operations[i]);
            }

            err = qdb_api.qdb_free_operations(_handle, operations, (UIntPtr) operations.Length);
            QdbExceptionThrower.ThrowIfNeeded(err);
        }
    }
}
