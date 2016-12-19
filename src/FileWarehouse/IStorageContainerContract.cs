// -----------------------------------------------------------------------
// <copyright file="IStorageContainerContract.cs">
// Copyright (c) 2013-2016 Andrey Veselov. All rights reserved.
// License: Apache License 2.0
// Contacts: http://andrey.moveax.com  andrey@moveax.com
// </copyright>
// -----------------------------------------------------------------------
 
namespace FileWarehouse
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;

    [ContractClassFor(typeof(IStorageContainer))]
    internal abstract class IStorageContainerContract : IStorageContainer
    {
        public Task<ContainerPermission> GetPermissionAsync()
        {
            Contract.Ensures(Contract.Result<ContainerPermission>() != ContainerPermission.Unspecified);

            return default(Task<ContainerPermission>);
        }

        public Task SetPermissionAsync(ContainerPermission permission)
        {
            Contract.Requires(permission != ContainerPermission.Unspecified);
            Contract.Ensures(Contract.Result<Task>() != null);

            return default(Task);
        }

        public Task CreateDirectoryAsync(string path)
        {
            Contract.Requires(!string.IsNullOrEmpty(path));
            Contract.Ensures(Contract.Result<Task>() != null);

            return default(Task);
        }

        public Task DeleteDirectoryAsync(string path)
        {
            Contract.Requires(!string.IsNullOrEmpty(path));
            Contract.Ensures(Contract.Result<Task>() != null);

            return default(Task);
        }

        public Task<IEnumerable<string>> GetDirectoriesAsync(string path)
        {
            Contract.Requires(path != null); // path can be empty for root folder
            Contract.Ensures(Contract.Result<Task<IEnumerable<string>>>() != null);
            Contract.Ensures(Contract.Result<Task<IEnumerable<string>>>().Result != null);

            return default(Task<IEnumerable<string>>);
        }

        public Task WriteFileAsync(Stream source, string path)
        {
            Contract.Requires(source != null);
            Contract.Requires(!string.IsNullOrEmpty(path));

            return default(Task);
        }

        public Task<Stream> GetFileStreamAsync(string path)
        {
            Contract.Requires(!string.IsNullOrEmpty(path));
            Contract.Ensures(Contract.Result<Task<Stream>>() != null);
            Contract.Ensures(Contract.Result<Task<Stream>>().Result != null);

            return default(Task<Stream>);
        }

        public Task GetFileStreamAsync(Stream target, string path)
        {
            Contract.Requires(!string.IsNullOrEmpty(path));
            Contract.Ensures(Contract.Result<Task>() != null);

            return default(Task);
        }

        public Task DeleteFileAsync(string path)
        {
            Contract.Requires(!string.IsNullOrEmpty(path));
            Contract.Ensures(Contract.Result<Task>() != null);

            return default(Task);
        }

        public Task<IEnumerable<string>> GetFilesAsync(string path, StorageSearchOption searchOption)
        {
            Contract.Requires(path != null);

            Contract.Ensures(Contract.Result<Task<IEnumerable<string>>>() != null);
            Contract.Ensures(Contract.Result<Task<IEnumerable<string>>>().Result != null);

            return default(Task<IEnumerable<string>>);
        }

        public string GetUrlForFile(string path)
        {
            Contract.Requires(!string.IsNullOrEmpty(path));

            return default(string);
        }
    }
}