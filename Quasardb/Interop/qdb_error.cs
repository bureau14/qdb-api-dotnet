using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable InconsistentNaming

namespace Quasardb.Interop
{
    public enum qdb_error
    {
        qdb_e_uninitialized = -1,
        qdb_e_ok = 0,
        qdb_e_system = 1, /* check errno or GetLastError() for actual error */
        qdb_e_internal = 2,
        qdb_e_no_memory = 3,
        qdb_e_invalid_protocol = 4,
        qdb_e_host_not_found = 5,
        qdb_e_invalid_option = 6,
        qdb_e_alias_too_long = 7,
        qdb_e_alias_not_found = 8,
        qdb_e_alias_already_exists = 9,
        qdb_e_timeout = 10,
        qdb_e_buffer_too_small = 11,
        qdb_e_invalid_command = 12,
        qdb_e_invalid_input = 13, /* deprecated, qdb_e_invalid_argument is used instead */
        qdb_e_connection_refused = 14,
        qdb_e_connection_reset = 15,
        qdb_e_unexpected_reply = 16,
        qdb_e_not_implemented = 17,
        qdb_e_unstable_hive = 18,
        qdb_e_protocol_error = 19,
        qdb_e_outdated_topology = 20,
        qdb_e_wrong_peer = 21,
        qdb_e_invalid_version = 22,
        qdb_e_try_again = 23,
        qdb_e_invalid_argument = 24,
        qdb_e_out_of_bounds = 25,
        qdb_e_conflict = 26,
        qdb_e_not_connected = 27,
        qdb_e_invalid_handle = 28,
        qdb_e_reserved_alias = 29,
        qdb_e_unmatched_content = 30,
        qdb_e_invalid_iterator = 31,
        qdb_e_prefix_too_short = 32,
        qdb_e_skipped = 33,
        qdb_e_incompatible_type = 34,
        qdb_e_empty_container = 35,
        qdb_e_container_full = 36,
        qdb_e_element_not_found = 37,
        qdb_e_element_already_exists = 38,
        qdb_e_overflow = 39,
        qdb_e_underflow = 40,
        qdb_e_tag_already_set = 41,
        qdb_e_tag_not_set = 42,
        qdb_e_entry_too_large = 43,
        qdb_e_transaction_partial_failure = 44
    }
}
