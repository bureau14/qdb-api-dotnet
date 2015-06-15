using System;
using System.Collections.Generic;
using Quasardb.Interop;

namespace Quasardb.Exceptions
{
    /// <summary>
    /// Base class of all quasardb exceptions
    /// </summary>
    public abstract class QdbExceptionBase : Exception
    {
    }

    /// <summary>
    /// Quasardb: Uninitialized error variable.
    /// </summary>
	public sealed class QdbUninitializedException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: Uninitialized error variable."; }
        }
	}

    /// <summary>
    /// Quasardb: A system error occurred.
    /// </summary>
	public sealed class QdbSystemException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: A system error occurred."; }
        }
	}

    /// <summary>
    /// Quasardb: An internal error occurred.
    /// </summary>
	public sealed class QdbInternalException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: An internal error occurred."; }
        }
	}

    /// <summary>
    /// Quasardb: Insufficient memory is available to complete the operation.
    /// </summary>
	public sealed class QdbNoMemoryException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: Insufficient memory is available to complete the operation."; }
        }
	}

    /// <summary>
    /// Quasardb: The remote host protocol mismatches the client API protocol.
    /// </summary>
	public sealed class QdbInvalidProtocolException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: The remote host protocol mismatches the client API protocol."; }
        }
	}

    /// <summary>
    /// Quasardb: The remote host cannot be resolved.
    /// </summary>
	public sealed class QdbHostNotFoundException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: The remote host cannot be resolved."; }
        }
	}

    /// <summary>
    /// Quasardb: An entry matching the provided alias cannot be found.
    /// </summary>
	public sealed class QdbAliasNotFoundException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: An entry matching the provided alias cannot be found."; }
        }
	}

    /// <summary>
    /// Quasardb: An entry matching the provided alias already exists.
    /// </summary>
	public sealed class QdbAliasAlreadyExistsException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: An entry matching the provided alias already exists."; }
        }
	}

    /// <summary>
    /// Quasardb: The operation timed out.
    /// </summary>
	public sealed class QdbTimeoutException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: The operation timed out."; }
        }
	}

    /// <summary>
    /// Quasardb: The provided buffer is too small.
    /// </summary>
	public sealed class QdbBufferTooSmallException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: The provided buffer is too small."; }
        }
	}

    /// <summary>
    /// Quasardb: Connection refused.
    /// </summary>
	public sealed class QdbConnectionRefusedException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: Connection refused."; }
        }
	}

    /// <summary>
    /// Quasardb: Connection reset by peer.
    /// </summary>
	public sealed class QdbConnectionResetException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: Connection reset by peer."; }
        }
	}

    /// <summary>
    /// Quasardb: Unexpected reply from the remote host.
    /// </summary>
	public sealed class QdbUnexpectedReplyException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: Unexpected reply from the remote host."; }
        }
	}

    /// <summary>
    /// Quasardb: The requested operation is not yet available.
    /// </summary>
	public sealed class QdbNotImplementedException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: The requested operation is not yet available."; }
        }
	}

    /// <summary>
    /// Quasardb: The cluster is unstable. Please try again later.
    /// </summary>
	public sealed class QdbUnstableClusterException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: The cluster is unstable. Please try again later."; }
        }
	}

    /// <summary>
    /// Quasardb: A protocol error occurred.
    /// </summary>
	public sealed class QdbProtocolErrorException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: A protocol error occurred."; }
        }
	}

    /// <summary>
    /// Quasardb: The cluster topology has changed. Please try again.
    /// </summary>
	public sealed class QdbOutdatedTopologyException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: The cluster topology has changed. Please try again."; }
        }
	}

    /// <summary>
    /// Quasardb: A request to the wrong peer has been made. Please try again.
    /// </summary>
	public sealed class QdbWrongPeerException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: A request to the wrong peer has been made. Please try again."; }
        }
	}

    /// <summary>
    /// Quasardb: The remote host and Client API versions mismatch.
    /// </summary>
	public sealed class QdbInvalidVersionException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: The remote host and Client API versions mismatch."; }
        }
	}

    /// <summary>
    /// Quasardb: A temporary error occurred. Please try again.
    /// </summary>
	public sealed class QdbTryAgainException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: A temporary error occurred. Please try again."; }
        }
	}

    /// <summary>
    /// Quasardb: The argument is invalid.
    /// </summary>
	public sealed class QdbInvalidArgumentException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: The argument is invalid."; }
        }
	}

    /// <summary>
    /// Quasardb: Invalid memory access: out of bounds.
    /// </summary>
	public sealed class QdbOutOfBoundsException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: Invalid memory access: out of bounds."; }
        }
	}

    /// <summary>
    /// Quasardb: The operation has been aborted as it conflicts with another ongoing operation.
    /// </summary>
	public sealed class QdbConflictException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: The operation has been aborted as it conflicts with another ongoing operation."; }
        }
	}

    /// <summary>
    /// Quasardb: The handle is not connected.
    /// </summary>
	public sealed class QdbNotConnectedException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: The handle is not connected."; }
        }
	}

    /// <summary>
    /// Quasardb: The handle is invalid.
    /// </summary>
	public sealed class QdbInvalidHandleException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: The handle is invalid."; }
        }
	}

    /// <summary>
    /// Quasardb: The alias name or prefix is reserved.
    /// </summary>
	public sealed class QdbReservedAliasException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: The alias name or prefix is reserved."; }
        }
	}

    /// <summary>
    /// Quasardb: The content does not match.
    /// </summary>
	public sealed class QdbUnmatchedContentException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: The content does not match."; }
        }
	}

    /// <summary>
    /// Quasardb: The iterator is invalid
    /// </summary>
	public sealed class QdbInvalidIteratorException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: The iterator is invalid"; }
        }
	}

    /// <summary>
    /// Quasardb: The supplied prefix is too short
    /// </summary>
	public sealed class QdbPrefixTooShortException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: The supplied prefix is too short"; }
        }
	}

    /// <summary>
    /// Quasardb: This operation has been skipped because of an error that occured in another operation.
    /// </summary>
	public sealed class QdbSkippedException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: This operation has been skipped because of an error that occured in another operation."; }
        }
	}

    /// <summary>
    /// Quasardb: The alias has a type incompatible for this operation.
    /// </summary>
	public sealed class QdbIncompatibleTypeException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: The alias has a type incompatible for this operation."; }
        }
	}

    /// <summary>
    /// Quasardb: The entry contains an empty container.
    /// </summary>
	public sealed class QdbEmptyContainerException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: The entry contains an empty container."; }
        }
	}

    /// <summary>
    /// Quasardb: The container is full.
    /// </summary>
	public sealed class QdbContainerFullException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: The container is full."; }
        }
	}

    /// <summary>
    /// Quasardb: The entry does not contain the given element.
    /// </summary>
	public sealed class QdbElementNotFoundException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: The entry does not contain the given element."; }
        }
	}

    /// <summary>
    /// Quasardb: The entry already contains the given element.
    /// </summary>
	public sealed class QdbElementAlreadyExistsException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: The entry already contains the given element."; }
        }
	}

    /// <summary>
    /// Quasardb: The operation provokes overflow.
    /// </summary>
	public sealed class QdbOverflowException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: The operation provokes overflow."; }
        }
	}

    /// <summary>
    /// Quasardb: The operation provokes underflow.
    /// </summary>
	public sealed class QdbUnderflowException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: The operation provokes underflow."; }
        }
	}

    /// <summary>
    /// Quasardb: The entry is already marked with the provided tag.
    /// </summary>
	public sealed class QdbTagAlreadySetException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: The entry is already marked with the provided tag."; }
        }
	}

    /// <summary>
    /// Quasardb: The entry is not marked with the provided tag.
    /// </summary>
	public sealed class QdbTagNotSetException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: The entry is not marked with the provided tag."; }
        }
	}

    /// <summary>
    /// Quasardb: The entry is larger than the allowed limit on the remote node.
    /// </summary>
	public sealed class QdbEntryTooLargeException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: The entry is larger than the allowed limit on the remote node."; }
        }
	}

    /// <summary>
    /// Quasardb: An unrecoverable occurred while committing or rollbacking the transaction.
    /// </summary>
	public sealed class QdbTransactionPartialFailureException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: An unrecoverable occurred while committing or rollbacking the transaction."; }
        }
	}

	static class QdbExceptionFactory 
	{
		static readonly Dictionary<qdb_error,Type> _types = new Dictionary<qdb_error,Type>
		{
			{qdb_error.qdb_e_uninitialized, typeof(QdbUninitializedException)},
			{qdb_error.qdb_e_system, typeof(QdbSystemException)},
			{qdb_error.qdb_e_internal, typeof(QdbInternalException)},
			{qdb_error.qdb_e_no_memory, typeof(QdbNoMemoryException)},
			{qdb_error.qdb_e_invalid_protocol, typeof(QdbInvalidProtocolException)},
			{qdb_error.qdb_e_host_not_found, typeof(QdbHostNotFoundException)},
			{qdb_error.qdb_e_alias_not_found, typeof(QdbAliasNotFoundException)},
			{qdb_error.qdb_e_alias_already_exists, typeof(QdbAliasAlreadyExistsException)},
			{qdb_error.qdb_e_timeout, typeof(QdbTimeoutException)},
			{qdb_error.qdb_e_buffer_too_small, typeof(QdbBufferTooSmallException)},
			{qdb_error.qdb_e_connection_refused, typeof(QdbConnectionRefusedException)},
			{qdb_error.qdb_e_connection_reset, typeof(QdbConnectionResetException)},
			{qdb_error.qdb_e_unexpected_reply, typeof(QdbUnexpectedReplyException)},
			{qdb_error.qdb_e_not_implemented, typeof(QdbNotImplementedException)},
			{qdb_error.qdb_e_unstable_cluster, typeof(QdbUnstableClusterException)},
			{qdb_error.qdb_e_protocol_error, typeof(QdbProtocolErrorException)},
			{qdb_error.qdb_e_outdated_topology, typeof(QdbOutdatedTopologyException)},
			{qdb_error.qdb_e_wrong_peer, typeof(QdbWrongPeerException)},
			{qdb_error.qdb_e_invalid_version, typeof(QdbInvalidVersionException)},
			{qdb_error.qdb_e_try_again, typeof(QdbTryAgainException)},
			{qdb_error.qdb_e_invalid_argument, typeof(QdbInvalidArgumentException)},
			{qdb_error.qdb_e_out_of_bounds, typeof(QdbOutOfBoundsException)},
			{qdb_error.qdb_e_conflict, typeof(QdbConflictException)},
			{qdb_error.qdb_e_not_connected, typeof(QdbNotConnectedException)},
			{qdb_error.qdb_e_invalid_handle, typeof(QdbInvalidHandleException)},
			{qdb_error.qdb_e_reserved_alias, typeof(QdbReservedAliasException)},
			{qdb_error.qdb_e_unmatched_content, typeof(QdbUnmatchedContentException)},
			{qdb_error.qdb_e_invalid_iterator, typeof(QdbInvalidIteratorException)},
			{qdb_error.qdb_e_prefix_too_short, typeof(QdbPrefixTooShortException)},
			{qdb_error.qdb_e_skipped, typeof(QdbSkippedException)},
			{qdb_error.qdb_e_incompatible_type, typeof(QdbIncompatibleTypeException)},
			{qdb_error.qdb_e_empty_container, typeof(QdbEmptyContainerException)},
			{qdb_error.qdb_e_container_full, typeof(QdbContainerFullException)},
			{qdb_error.qdb_e_element_not_found, typeof(QdbElementNotFoundException)},
			{qdb_error.qdb_e_element_already_exists, typeof(QdbElementAlreadyExistsException)},
			{qdb_error.qdb_e_overflow, typeof(QdbOverflowException)},
			{qdb_error.qdb_e_underflow, typeof(QdbUnderflowException)},
			{qdb_error.qdb_e_tag_already_set, typeof(QdbTagAlreadySetException)},
			{qdb_error.qdb_e_tag_not_set, typeof(QdbTagNotSetException)},
			{qdb_error.qdb_e_entry_too_large, typeof(QdbEntryTooLargeException)},
			{qdb_error.qdb_e_transaction_partial_failure, typeof(QdbTransactionPartialFailureException)},
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
