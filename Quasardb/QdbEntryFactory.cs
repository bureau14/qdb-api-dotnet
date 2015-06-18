﻿using System;
using Quasardb.ManagedApi;

namespace Quasardb
{
    class QdbEntryFactory
    {
        readonly QdbApi _handle;

        public QdbEntryFactory(QdbApi handle)
        {
            _handle = handle;
        }

        public QdbEntry Create(string alias)
        {
            return Create(_handle.GetEntryType(alias), alias);
        }
        
        QdbEntry Create(QdbEntryType type, string alias)
        {
            switch (type)
            {
                case QdbEntryType.Blob:
                    return new QdbBlob(_handle, alias);

                case QdbEntryType.HashSet:
                    return new QdbHashSet(_handle, alias);

                case QdbEntryType.Integer:
                    return new QdbInteger(_handle, alias);

                case QdbEntryType.Queue:
                    return new QdbQueue(_handle, alias);

                case QdbEntryType.Tag:
                    return new QdbTag(_handle, alias);

                default:
                    throw new NotImplementedException("Please contact quasardb support.");
            }
        }
    }
}
