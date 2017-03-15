using System;
using System.Collections;
using System.Collections.Generic;
using Quasardb.NativeApi;

namespace Quasardb.ManagedApi
{
    sealed unsafe class QdbAliasCollection : IEnumerable<qdb_string>, IDisposable
    {
        readonly qdb_handle _handle;
        internal qdb_string* Pointer;
        internal UIntPtr Size;
        
        internal QdbAliasCollection(qdb_handle handle)
        {
            _handle = handle;
        }

        public void Dispose()
        {
            qdb_api.qdb_free_results(_handle, Pointer, Size);
        }

        public qdb_string this[ulong index] => Pointer[index];

        public IEnumerator<qdb_string> GetEnumerator()
        {
            for (var i = 0UL; i < Size.ToUInt64(); i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
