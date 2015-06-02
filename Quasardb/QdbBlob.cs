using System;
using Quasardb.Exceptions;
using Quasardb.Interop;

namespace Quasardb
{
    public class QdbBlob : QdbExpirableEntry
    {
        public QdbBlob(qdb_handle handle, string alias) : base(handle, alias)
        {
        }

        public byte[] CompareAndSwap(byte[] content, byte[] comparand)
        {
            qdb_buffer oldContent;
            long oldContentLength;
            
            var error = qdb_api.qdb_compare_and_swap(_handle, _alias, content, content.LongLength, comparand, comparand.LongLength, 0, out oldContent, out oldContentLength);
            QdbExceptionThrower.ThrowIfNeeded(error);

            return oldContent.Copy(oldContentLength);
        }

        public byte[] Get()
        {
            qdb_buffer content;
            long contentLength;
            var error = qdb_api.qdb_get(_handle, _alias, out content, out contentLength);
            QdbExceptionThrower.ThrowIfNeeded(error);
            return content.Copy(contentLength);
        }

        public byte[] GetAndRemove()
        {
            qdb_buffer content;
            long contentLength;
            var error = qdb_api.qdb_get_and_remove(_handle, _alias, out content, out contentLength);
            QdbExceptionThrower.ThrowIfNeeded(error);
            return content.Copy(contentLength);
        }

        public byte[] GetAndUpdate(byte[] content)
        {
            qdb_buffer oldContent;
            long oldContentLength;
            var error = qdb_api.qdb_get_and_update(_handle, _alias, content, content.LongLength, 0, out oldContent, out oldContentLength);
            QdbExceptionThrower.ThrowIfNeeded(error);
            return oldContent.Copy(oldContentLength);
        }

        public void Put(byte[] content, DateTime expiryTime=default(DateTime))
        {

            var error = qdb_api.qdb_put(_handle, _alias, content, content.LongLength, TranslateExpiryTime(expiryTime));
            QdbExceptionThrower.ThrowIfNeeded(error);
        }

        public bool RemoveIf(byte[] comparand)
        {
            var error = qdb_api.qdb_remove_if(_handle, _alias, comparand, comparand.Length);
            if (error == qdb_error.qdb_e_unmatched_content) return false;
            QdbExceptionThrower.ThrowIfNeeded(error);
            return true;
        }

        public void Update(byte[] content)
        {
            var error = qdb_api.qdb_update(_handle, _alias, content, content.Length, 0);
            QdbExceptionThrower.ThrowIfNeeded(error);
        }
    }
}
