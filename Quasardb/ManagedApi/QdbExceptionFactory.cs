using System;
using Quasardb.Exceptions;
using Quasardb.NativeApi;

namespace Quasardb.ManagedApi
{
  	static class QdbExceptionFactory 
	{
		public static QdbExceptionBase Create(qdb_error error)
		{
			switch (error)
			{
				case qdb_error.qdb_e_alias_already_exists:
					return new QdbAliasAlreadyExistsException();

				case qdb_error.qdb_e_alias_not_found:
					return new QdbAliasNotFoundException();

				case qdb_error.qdb_e_buffer_too_small:
					return new QdbBufferTooSmallException();

				case qdb_error.qdb_e_conflict:
					return new QdbConflictException();

				case qdb_error.qdb_e_connection_refused:
					return new QdbConnectionRefusedException();

				case qdb_error.qdb_e_connection_reset:
					return new QdbConnectionResetException();

				case qdb_error.qdb_e_container_empty:
					return new QdbEmptyContainerException();

				case qdb_error.qdb_e_container_full:
					return new QdbContainerFullException();

				case qdb_error.qdb_e_element_already_exists:
					return new QdbElementAlreadyExistsException();

				case qdb_error.qdb_e_element_not_found:
					return new QdbElementNotFoundException();

				case qdb_error.qdb_e_entry_too_large:
					return new QdbEntryTooLargeException();

				case qdb_error.qdb_e_host_not_found:
					return new QdbHostNotFoundException();

				case qdb_error.qdb_e_incompatible_type:
					return new QdbIncompatibleTypeException();

				case qdb_error.qdb_e_internal_local:
					return new QdbInternalLocalException();

                case qdb_error.qdb_e_internal_remote:
                    return new QdbInternalRemoteException();

				case qdb_error.qdb_e_invalid_argument:
					return new QdbInvalidArgumentException();

				case qdb_error.qdb_e_invalid_handle:
					return new QdbInvalidHandleException();

				case qdb_error.qdb_e_invalid_iterator:
					return new QdbInvalidIteratorException();

				case qdb_error.qdb_e_invalid_protocol:
					return new QdbInvalidProtocolException();

				case qdb_error.qdb_e_invalid_version:
					return new QdbInvalidVersionException();

				case qdb_error.qdb_e_no_memory_local:
					return new QdbNoMemoryLocalException();

                case qdb_error.qdb_e_no_memory_remote:
                    return new QdbNoMemoryRemoteException();

				case qdb_error.qdb_e_not_connected:
					return new QdbNotConnectedException();

				case qdb_error.qdb_e_not_implemented:
					return new QdbNotImplementedException();

				case qdb_error.qdb_e_out_of_bounds:
					return new QdbOutOfBoundsException();

				case qdb_error.qdb_e_outdated_topology:
					return new QdbOutdatedTopologyException();

				case qdb_error.qdb_e_overflow:
					return new QdbOverflowException();

				case qdb_error.qdb_e_protocol_error:
					return new QdbProtocolErrorException();

				case qdb_error.qdb_e_reserved_alias:
					return new QdbReservedAliasException();

				case qdb_error.qdb_e_resource_locked:
					return new QdbResourceLockedException();

				case qdb_error.qdb_e_skipped:
					return new QdbSkippedException();

				case qdb_error.qdb_e_system_local:
					return new QdbSystemLocalException();

                case qdb_error.qdb_e_system_remote:
                    return new QdbSystemRemoteException();

				case qdb_error.qdb_e_tag_already_set:
					return new QdbTagAlreadySetException();

				case qdb_error.qdb_e_tag_not_set:
					return new QdbTagNotSetException();

				case qdb_error.qdb_e_timeout:
					return new QdbTimeoutException();

				case qdb_error.qdb_e_transaction_partial_failure:
					return new QdbTransactionPartialFailureException();

				case qdb_error.qdb_e_try_again:
					return new QdbTryAgainException();

				case qdb_error.qdb_e_underflow:
					return new QdbUnderflowException();

				case qdb_error.qdb_e_unexpected_reply:
					return new QdbUnexpectedReplyException();

				case qdb_error.qdb_e_unmatched_content:
					return new QdbUnmatchedContentException();

				case qdb_error.qdb_e_unstable_cluster:
					return new QdbUnstableClusterException();

				case qdb_error.qdb_e_wrong_peer:
					return new QdbWrongPeerException();

				default:
					throw new NotSupportedException("Unexpected qdb_error: 0x" + error.ToString("X"));
			}
		}
	}
}
