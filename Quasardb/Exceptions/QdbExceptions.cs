using System;
using System.Collections.Generic;
using Quasardb.Interop;

namespace Quasardb.Exceptions
{
    public abstract class QdbExceptionBase : Exception
    {
    }

	public sealed class QdbUninitializedException : QdbExceptionBase 
	{
	}

	public sealed class QdbSystemException : QdbExceptionBase 
	{
	}

	public sealed class QdbInternalException : QdbExceptionBase 
	{
	}

	public sealed class QdbNoMemoryException : QdbExceptionBase 
	{
	}

	public sealed class QdbInvalidProtocolException : QdbExceptionBase 
	{
	}

	public sealed class QdbHostNotFoundException : QdbExceptionBase 
	{
	}

	public sealed class QdbInvalidOptionException : QdbExceptionBase 
	{
	}

	public sealed class QdbAliasTooLongException : QdbExceptionBase 
	{
	}

	public sealed class QdbAliasNotFoundException : QdbExceptionBase 
	{
	}

	public sealed class QdbAliasAlreadyExistsException : QdbExceptionBase 
	{
	}

	public sealed class QdbTimeoutException : QdbExceptionBase 
	{
	}

	public sealed class QdbBufferTooSmallException : QdbExceptionBase 
	{
	}

	public sealed class QdbInvalidCommandException : QdbExceptionBase 
	{
	}

	public sealed class QdbInvalidInputException : QdbExceptionBase 
	{
	}

	public sealed class QdbConnectionRefusedException : QdbExceptionBase 
	{
	}

	public sealed class QdbConnectionResetException : QdbExceptionBase 
	{
	}

	public sealed class QdbUnexpectedReplyException : QdbExceptionBase 
	{
	}

	public sealed class QdbNotImplementedException : QdbExceptionBase 
	{
	}

	public sealed class QdbUnstableHiveException : QdbExceptionBase 
	{
	}

	public sealed class QdbProtocolErrorException : QdbExceptionBase 
	{
	}

	public sealed class QdbOutdatedTopologyException : QdbExceptionBase 
	{
	}

	public sealed class QdbWrongPeerException : QdbExceptionBase 
	{
	}

	public sealed class QdbInvalidVersionException : QdbExceptionBase 
	{
	}

	public sealed class QdbTryAgainException : QdbExceptionBase 
	{
	}

	public sealed class QdbInvalidArgumentException : QdbExceptionBase 
	{
	}

	public sealed class QdbOutOfBoundsException : QdbExceptionBase 
	{
	}

	public sealed class QdbConflictException : QdbExceptionBase 
	{
	}

	public sealed class QdbNotConnectedException : QdbExceptionBase 
	{
	}

	public sealed class QdbInvalidHandleException : QdbExceptionBase 
	{
	}

	public sealed class QdbReservedAliasException : QdbExceptionBase 
	{
	}

	public sealed class QdbUnmatchedContentException : QdbExceptionBase 
	{
	}

	public sealed class QdbInvalidIteratorException : QdbExceptionBase 
	{
	}

	public sealed class QdbPrefixTooShortException : QdbExceptionBase 
	{
	}

	public sealed class QdbSkippedException : QdbExceptionBase 
	{
	}

	public sealed class QdbIncompatibleTypeException : QdbExceptionBase 
	{
	}

	public sealed class QdbEmptyContainerException : QdbExceptionBase 
	{
	}

	public sealed class QdbContainerFullException : QdbExceptionBase 
	{
	}

	public sealed class QdbElementNotFoundException : QdbExceptionBase 
	{
	}

	public sealed class QdbElementAlreadyExistsException : QdbExceptionBase 
	{
	}

	public sealed class QdbOverflowException : QdbExceptionBase 
	{
	}

	public sealed class QdbUnderflowException : QdbExceptionBase 
	{
	}

	public sealed class QdbTagAlreadySetException : QdbExceptionBase 
	{
	}

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
