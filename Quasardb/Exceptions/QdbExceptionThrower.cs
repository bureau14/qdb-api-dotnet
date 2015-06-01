using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quasardb.Interop;

namespace Quasardb.Exceptions
{
    static class QdbExceptionThrower
    {
        static readonly Dictionary<qdb_error, Type> _types = new Dictionary<qdb_error, Type>
        {
            {qdb_error.qdb_e_ok, null},
            {qdb_error.qdb_e_invalid_argument, typeof(ArgumentException)}
        };

        public static void ThrowIfNeeded(qdb_error error)
        {
            Type exceptionType;
            if (_types.TryGetValue(error, out exceptionType))
            {
                if (exceptionType != null)
                {
                    throw (Exception)Activator.CreateInstance(exceptionType);
                }
            }
            else
            {
                throw new NotSupportedException("Unexpected qdb_error: " + error);
            }
        }
    }
}
