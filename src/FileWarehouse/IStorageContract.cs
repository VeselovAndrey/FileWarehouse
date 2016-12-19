// -----------------------------------------------------------------------
// <copyright file="IStorageContract.cs">
// Copyright (c) 2013-2016 Andrey Veselov. All rights reserved.
// License: Apache License 2.0
// Contacts: http://andrey.moveax.com  andrey@moveax.com
// </copyright>
// -----------------------------------------------------------------------
 
namespace FileWarehouse
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    [ContractClassFor(typeof(IStorage))]
    internal abstract class IStorageContract : IStorage
    {
        public Task<IEnumerable<string>> GetContainersListAsync(string mask)
        {
            Contract.Ensures(Contract.Result<Task<IEnumerable<string>>>() != null);
            Contract.Ensures(Contract.Result<Task<IEnumerable<string>>>().Result != null);

            return default(Task<IEnumerable<string>>);
        }

        public Task<IStorageContainer> CreateOrGetContainerAsync(string name, ContainerPermission permission, bool failIfExists)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));
            Contract.Requires(permission != ContainerPermission.Unspecified);

            Contract.Ensures(Contract.Result<Task<IStorageContainer>>() != null);

            return default(Task<IStorageContainer>);
        }

        public Task<IStorageContainer> GetContainerAsync(string name)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));
            Contract.Ensures(Contract.Result<Task<IStorageContainer>>() != null);

            return default(Task<IStorageContainer>);
        }

        public Task DeleteContainerAsync(string name)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));
            Contract.Ensures(Contract.Result<Task>() != null);

            return default(Task);
        }
    }
}