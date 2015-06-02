using System;
using System.Collections.Generic;
using Quasardb.Interop;

namespace Quasardb.Exceptions
{
    public abstract class QdbExceptionBase : Exception
    {
    }

	[QdbError(qdb_error.qdb_e_uninitialized)]
	public sealed class QdbUninitializedException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_system)]
	public sealed class QdbSystemException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_internal)]
	public sealed class QdbInternalException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_no_memory)]
	public sealed class QdbNoMemoryException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_invalid_protocol)]
	public sealed class QdbInvalidProtocolException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_host_not_found)]
	public sealed class QdbHostNotFoundException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_invalid_option)]
	public sealed class QdbInvalidOptionException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_alias_too_long)]
	public sealed class QdbAliasTooLongException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_alias_not_found)]
	public sealed class QdbAliasNotFoundException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_alias_already_exists)]
	public sealed class QdbAliasAlreadyExistsException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_timeout)]
	public sealed class QdbTimeoutException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_buffer_too_small)]
	public sealed class QdbBufferTooSmallException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_invalid_command)]
	public sealed class QdbInvalidCommandException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_invalid_input)]
	public sealed class QdbInvalidInputException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_connection_refused)]
	public sealed class QdbConnectionRefusedException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_connection_reset)]
	public sealed class QdbConnectionResetException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_unexpected_reply)]
	public sealed class QdbUnexpectedReplyException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_not_implemented)]
	public sealed class QdbNotImplementedException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_unstable_hive)]
	public sealed class QdbUnstableHiveException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_protocol_error)]
	public sealed class QdbProtocolErrorException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_outdated_topology)]
	public sealed class QdbOutdatedTopologyException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_wrong_peer)]
	public sealed class QdbWrongPeerException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_invalid_version)]
	public sealed class QdbInvalidVersionException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_try_again)]
	public sealed class QdbTryAgainException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_invalid_argument)]
	public sealed class QdbInvalidArgumentException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_out_of_bounds)]
	public sealed class QdbOutOfBoundsException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_conflict)]
	public sealed class QdbConflictException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_not_connected)]
	public sealed class QdbNotConnectedException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_invalid_handle)]
	public sealed class QdbInvalidHandleException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_reserved_alias)]
	public sealed class QdbReservedAliasException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_unmatched_content)]
	public sealed class QdbUnmatchedContentException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_invalid_iterator)]
	public sealed class QdbInvalidIteratorException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_prefix_too_short)]
	public sealed class QdbPrefixTooShortException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_skipped)]
	public sealed class QdbSkippedException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_incompatible_type)]
	public sealed class QdbIncompatibleTypeException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_empty_container)]
	public sealed class QdbEmptyContainerException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_container_full)]
	public sealed class QdbContainerFullException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_element_not_found)]
	public sealed class QdbElementNotFoundException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_element_already_exists)]
	public sealed class QdbElementAlreadyExistsException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_overflow)]
	public sealed class QdbOverflowException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_underflow)]
	public sealed class QdbUnderflowException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_tag_already_set)]
	public sealed class QdbTagAlreadySetException : QdbExceptionBase 
	{
	}

	[QdbError(qdb_error.qdb_e_tag_not_set)]
	public sealed class QdbTagNotSetException : QdbExceptionBase 
	{
	}

	public static class QdbExecptionFactory 
	{
		static readonly Dictionary<qdb_error,Type> _types = new Dictionary<qdb_error,Type>
		{
			{ qdb_error.qdb_e_uninitialized, typeof(QdbUninitializedException) },
			{ qdb_error.qdb_e_system, typeof(QdbSystemException) },
			{ qdb_error.qdb_e_internal, typeof(QdbInternalException) },
			{ qdb_error.qdb_e_no_memory, typeof(QdbNoMemoryException) },
			{ qdb_error.qdb_e_invalid_protocol, typeof(QdbInvalidProtocolException) },
			{ qdb_error.qdb_e_host_not_found, typeof(QdbHostNotFoundException) },
			{ qdb_error.qdb_e_invalid_option, typeof(QdbInvalidOptionException) },
			{ qdb_error.qdb_e_alias_too_long, typeof(QdbAliasTooLongException) },
			{ qdb_error.qdb_e_alias_not_found, typeof(QdbAliasNotFoundException) },
			{ qdb_error.qdb_e_alias_already_exists, typeof(QdbAliasAlreadyExistsException) },
			{ qdb_error.qdb_e_timeout, typeof(QdbTimeoutException) },
			{ qdb_error.qdb_e_buffer_too_small, typeof(QdbBufferTooSmallException) },
			{ qdb_error.qdb_e_invalid_command, typeof(QdbInvalidCommandException) },
			{ qdb_error.qdb_e_invalid_input, typeof(QdbInvalidInputException) },
			{ qdb_error.qdb_e_connection_refused, typeof(QdbConnectionRefusedException) },
			{ qdb_error.qdb_e_connection_reset, typeof(QdbConnectionResetException) },
			{ qdb_error.qdb_e_unexpected_reply, typeof(QdbUnexpectedReplyException) },
			{ qdb_error.qdb_e_not_implemented, typeof(QdbNotImplementedException) },
			{ qdb_error.qdb_e_unstable_hive, typeof(QdbUnstableHiveException) },
			{ qdb_error.qdb_e_protocol_error, typeof(QdbProtocolErrorException) },
			{ qdb_error.qdb_e_outdated_topology, typeof(QdbOutdatedTopologyException) },
			{ qdb_error.qdb_e_wrong_peer, typeof(QdbWrongPeerException) },
			{ qdb_error.qdb_e_invalid_version, typeof(QdbInvalidVersionException) },
			{ qdb_error.qdb_e_try_again, typeof(QdbTryAgainException) },
			{ qdb_error.qdb_e_invalid_argument, typeof(QdbInvalidArgumentException) },
			{ qdb_error.qdb_e_out_of_bounds, typeof(QdbOutOfBoundsException) },
			{ qdb_error.qdb_e_conflict, typeof(QdbConflictException) },
			{ qdb_error.qdb_e_not_connected, typeof(QdbNotConnectedException) },
			{ qdb_error.qdb_e_invalid_handle, typeof(QdbInvalidHandleException) },
			{ qdb_error.qdb_e_reserved_alias, typeof(QdbReservedAliasException) },
			{ qdb_error.qdb_e_unmatched_content, typeof(QdbUnmatchedContentException) },
			{ qdb_error.qdb_e_invalid_iterator, typeof(QdbInvalidIteratorException) },
			{ qdb_error.qdb_e_prefix_too_short, typeof(QdbPrefixTooShortException) },
			{ qdb_error.qdb_e_skipped, typeof(QdbSkippedException) },
			{ qdb_error.qdb_e_incompatible_type, typeof(QdbIncompatibleTypeException) },
			{ qdb_error.qdb_e_empty_container, typeof(QdbEmptyContainerException) },
			{ qdb_error.qdb_e_container_full, typeof(QdbContainerFullException) },
			{ qdb_error.qdb_e_element_not_found, typeof(QdbElementNotFoundException) },
			{ qdb_error.qdb_e_element_already_exists, typeof(QdbElementAlreadyExistsException) },
			{ qdb_error.qdb_e_overflow, typeof(QdbOverflowException) },
			{ qdb_error.qdb_e_underflow, typeof(QdbUnderflowException) },
			{ qdb_error.qdb_e_tag_already_set, typeof(QdbTagAlreadySetException) },
			{ qdb_error.qdb_e_tag_not_set, typeof(QdbTagNotSetException) },
		};

		public static QdbExceptionBase Create(qdb_error error)
		{
			Type exceptionType;
            if (!_types.TryGetValue(error, out exceptionType))
				throw new NotSupportedException("Unexpected qdb_error: " + error);
            
			return (QdbExceptionBase)Activator.CreateInstance(exceptionType);
		}
	}
}
