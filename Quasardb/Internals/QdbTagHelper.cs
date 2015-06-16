using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quasardb.Exceptions;
using Quasardb.Interop;

namespace Quasardb.Internals
{
    static class QdbTagHelper
    {
        public static bool AddTag(qdb_handle handle, string alias, string tag)
        {
            var error = qdb_api.qdb_add_tag(handle, alias, tag);

            switch (error)
            {
                case qdb_error.qdb_e_tag_already_set:
                    return false;

                case qdb_error.qdb_e_ok:
                    return true;

                default:
                    throw QdbExceptionFactory.Create(error);
            }
        }

        public static bool HasTag(qdb_handle handle, string alias, string tag)
        {
            var error = qdb_api.qdb_has_tag(handle, alias, tag);

            switch (error)
            {
                case qdb_error.qdb_e_tag_not_set:
                    return false;

                case qdb_error.qdb_e_ok:
                    return true;

                default:
                    throw QdbExceptionFactory.Create(error);
            }
        }

        public static bool RemoveTag(qdb_handle handle, string alias, string tag)
        {
            var error = qdb_api.qdb_remove_tag(handle, alias, tag);

            switch (error)
            {
                case qdb_error.qdb_e_tag_not_set:
                    return false;

                case qdb_error.qdb_e_ok:
                    return true;

                default:
                    throw QdbExceptionFactory.Create(error);
            }
        }
    }
}
