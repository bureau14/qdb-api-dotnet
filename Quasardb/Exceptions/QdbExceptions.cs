using System;

namespace Quasardb.Exceptions
{
    /// <summary>
    /// Base class of all quasardb exceptions
    /// </summary>
    public abstract class QdbExceptionBase : Exception
    {
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
    /// Quasardb: This operation has been skipped because of an error that occured in another operatio
    /// </summary>
	public sealed class QdbSkippedException : QdbExceptionBase 
	{
		/// <inheritdoc />
        public override string Message
        {
            get { return "Quasardb: This operation has been skipped because of an error that occured in another operatio"; }
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
}
