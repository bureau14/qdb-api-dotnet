﻿namespace Quasardb.Exceptions
{
    /// <summary>
    /// Exception thrown when the specified entry already exists in the database.
    /// </summary>
    public sealed class QdbAliasAlreadyExistsException : QdbOperationException
    {
        internal QdbAliasAlreadyExistsException() : base("An entry matching the provided alias already exists.")
        {
        }
    }
}