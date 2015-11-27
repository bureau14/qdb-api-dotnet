using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Quasardb.NativeApi;

namespace Quasardb.ManagedApi
{
    sealed class QdbAliasCollection : IEnumerable<string>
    {
        readonly qdb_handle _handle;
        internal IntPtr Pointer;
        internal UIntPtr Size;
        
        internal QdbAliasCollection(qdb_handle handle)
        {
            _handle = handle;
        }

        ~QdbAliasCollection()
        {
            qdb_api.qdb_free_results(_handle, Pointer, Size);
        }

        public IEnumerator<string> GetEnumerator()
        {
            // CAUTION: limited to 32 bits!!!
            for (var i = 0; i < (int) Size; i++)
            {
                var aliasPointer = Marshal.ReadIntPtr(Pointer,  i*IntPtr.Size);
                var alias = Marshal.PtrToStringAnsi(aliasPointer);

                yield return alias;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
