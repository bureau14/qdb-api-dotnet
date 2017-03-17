using System;
using System.Collections;
using System.Collections.Generic;
using Quasardb.Native;

namespace Quasardb.ManagedApi
{
    sealed unsafe class QdbDoublePointCollection : IEnumerable<qdb_ts_double_point>, IDisposable
    {
        readonly qdb_handle _handle;
        internal qdb_ts_double_point* Pointer;
        internal UIntPtr Size;
        
        internal QdbDoublePointCollection(qdb_handle handle)
        {
            _handle = handle;
        }

        public void Dispose()
        {
            qdb_api.qdb_free_buffer(_handle, Pointer);
        }
        
        public IEnumerator<qdb_ts_double_point> GetEnumerator()
        {
            for (var i = 0UL; i < Size.ToUInt64(); i++)
                yield return this[i];
        }

        public qdb_ts_double_point this[ulong index] => Pointer[index];

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
