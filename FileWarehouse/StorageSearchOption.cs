// -----------------------------------------------------------------------
// <copyright file="StorageSearchOption.cs">
// Copyright (c) 2013-2017 Andrey Veselov. All rights reserved.
// License: Apache License 2.0
// Contacts: http://andrey.moveax.com  andrey@moveax.com
// </copyright>
// -----------------------------------------------------------------------

namespace FileWarehouse
{
    using System;

    [Flags]
    public enum StorageSearchOption
    {
        Default = 0x00,
        AllDirectories = 0x01,
        StripPaths = 0x02
    }
}