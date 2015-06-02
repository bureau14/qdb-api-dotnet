using System;
using Quasardb.Interop;

namespace Quasardb.Exceptions
{
    class QdbErrorAttribute : Attribute
    {
        public readonly qdb_error Error;

        public QdbErrorAttribute(qdb_error error)
        {
            Error = error;
        }
    }
}
