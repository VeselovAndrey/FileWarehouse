// -----------------------------------------------------------------------
// <copyright file="IStorage.cs">
// Copyright (c) 2013-2017 Andrey Veselov. All rights reserved.
// License: Apache License 2.0
// Contacts: http://andrey.moveax.com  andrey@moveax.com
// </copyright>
// -----------------------------------------------------------------------

namespace FileWarehouse
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    [ContractClass(typeof(IStorageContract))]
    public interface IStorage
    {
        /// <summary>Gets the containers list asynchronously.</summary>
        /// <param name="mask">The mask.</param>
        /// <returns>The task object representing the asynchronous operation. The Result property on the task object returns a list of containers.</returns>
        Task<IEnumerable<string>> GetContainersListAsync(string mask);

        /// <summary>Creates the storage container asynchronously.</summary>
        /// <param name="name">The container name.</param>
        /// <param name="permission">The permission.</param>
        /// <param name="failIfExists"><c>true</c> to throw an exception if an container with specified name is exists.</param>
        /// <returns>The task object representing the asynchronous operation. The Result property on the task object returns a storage container interface.</returns>
        Task<IStorageContainer> CreateOrGetContainerAsync(string name, ContainerPermission permission, bool failIfExists);

        /// <summary>Gets the existing container asynchronously.</summary>
        /// <param name="name">The container name.</param>
        /// <returns>The task object representing the asynchronous operation. The Result property on the task object returns a storage container interface.</returns>
        Task<IStorageContainer> GetContainerAsync(string name);

        /// <summary>Deletes the specified container asynchronously.</summary>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task DeleteContainerAsync(string name);
    }
}