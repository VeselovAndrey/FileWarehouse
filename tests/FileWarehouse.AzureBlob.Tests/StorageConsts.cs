// -----------------------------------------------------------------------
// <copyright file="StorageConsts.cs">
// Copyright (c) 2013-2016 Andrey Veselov. All rights reserved.
// License: Apache License 2.0
// Contacts: http://andrey.moveax.com  andrey@moveax.com
// </copyright>
// -----------------------------------------------------------------------
 
namespace FileWarehouse.AzureBlob.Tests
{
    public static class StorageConsts
    {
        public const string ConnectionString = "UseDevelopmentStorage=true";
        public const string ContainerNamesPrefix = "WHTEST-";
        public const string Container1Name = "WHTEST-79A1D3CA-6524-453A-B80A-9E223EFF37FF";
        public const string Container2Name = "WHTEST-9FF9A850-EC62-4129-9B3F-97A89B9FFEA1";

        public const string File1Name = @"folder1/subfolder/fileText1.dat";
    }
}