using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// ReSharper disable InconsistentNaming

namespace Quasardb.Interop
{
    enum qdb_entry_type
    {
        qdb_entry_uninitialized = -1,
        qdb_entry_blob = 0,
        qdb_entry_integer = 1,
        qdb_entry_hset = 2,
        qdb_entry_tag = 3,
        qdb_entry_queue = 4
    }
}
