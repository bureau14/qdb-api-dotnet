using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using Quasardb.Exceptions;
using Quasardb.Interop;

namespace Quasardb
{
    sealed class QdbEntryCollection : CriticalFinalizerObject, IDisposable, IEnumerable<QdbEntry>
    {
        readonly qdb_handle _handle;
        internal IntPtr Pointer;
        internal IntPtr Size;
        
        internal QdbEntryCollection(qdb_handle handle)
        {
            _handle = handle;
        }

        ~QdbEntryCollection()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        void Dispose(bool disposing)
        {
            qdb_api.qdb_free_results(_handle, Pointer, Size);
            Pointer = IntPtr.Zero;
            Size = IntPtr.Zero;
        }

        public IEnumerator<QdbEntry> GetEnumerator()
        {
            var factory = new QdbEntryFactory(_handle);

            // CAUTION: limited to 32 bits!!!
            for (var i = 0; i < (int) Size; i++)
            {
                var aliasPointer = Marshal.ReadIntPtr(Pointer,  i*IntPtr.Size);
                var alias = Marshal.PtrToStringAnsi(aliasPointer);

                yield return factory.Create(alias);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


    }
}
