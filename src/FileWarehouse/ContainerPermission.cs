// -----------------------------------------------------------------------
// <copyright file="ContainerPermission.cs">
// Copyright (c) 2013-2016 Andrey Veselov. All rights reserved.
// License: Apache License 2.0
// Contacts: http://andrey.moveax.com  andrey@moveax.com
// </copyright>
// -----------------------------------------------------------------------
 
namespace FileWarehouse
{
    using System;

    [Flags]
    public enum ContainerPermission
    {
        Unspecified = 0x00, // Unspecified

        Private = 0x01,
        Public = 0x02
    }
}