// -----------------------------------------------------------------------
// <copyright file="StorageException.cs">
// Copyright (c) 2013-2015 Andrey Veselov. All rights reserved.
// License: Apache License 2.0
// Contacts: http://andrey.moveax.com  andrey@moveax.com
// </copyright>
// -----------------------------------------------------------------------
 
namespace FileWarehouse
{
    using System;

    public class StorageException : Exception
    {
        public StorageExceptionReason Reason { get; private set; }

        public StorageException(StorageExceptionReason reason, string message)
            : this(reason, message, null)
        {
        }

        public StorageException(StorageExceptionReason reason, string message, Exception innerException)
            : base(message, innerException)
        {
            this.Reason = reason;
        }
    }
}