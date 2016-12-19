// -----------------------------------------------------------------------
// <copyright file="StorageExceptionReason.cs">
// Copyright (c) 2013-2016 Andrey Veselov. All rights reserved.
// License: Apache License 2.0
// Contacts: http://andrey.moveax.com  andrey@moveax.com
// </copyright>
// -----------------------------------------------------------------------
 
namespace FileWarehouse
{
    public enum StorageExceptionReason
    {
        Unspecified = 0,

        ContainerNotFound = 1,
        ContainerExistAlready = 2,

        StorageOperationFailed = 255
    }
}