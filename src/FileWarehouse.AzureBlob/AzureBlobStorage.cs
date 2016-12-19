// -----------------------------------------------------------------------
// <copyright file="AzureBlobStorage.cs">
// Copyright (c) 2013-2016 Andrey Veselov. All rights reserved.
// License: Apache License 2.0
// Contacts: http://andrey.moveax.com  andrey@moveax.com
// </copyright>
// -----------------------------------------------------------------------
 
namespace FileWarehouse.AzureBlob
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using System.Diagnostics.Contracts;

    public class AzureBlobStorage : IStorage
    {
        private readonly CloudBlobClient _blobClient;

        public AzureBlobStorage(string connectionString)
        {
            Contract.Requires(!string.IsNullOrEmpty(connectionString));

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            this._blobClient = storageAccount.CreateCloudBlobClient();
        }

        public async Task<IEnumerable<string>> GetContainersListAsync(string mask)
        {
            try {
                BlobContinuationToken continuationToken = null;
                var containers = new List<CloudBlobContainer>();

                do {
                    var response = await this._blobClient
                        .ListContainersSegmentedAsync(mask, continuationToken)
                        .ConfigureAwait(false);

                    containers.AddRange(response.Results);
                    continuationToken = response.ContinuationToken;
                }
                while (continuationToken != null);

                IEnumerable<string> result = containers
                    .Select(c => c.Name)
                    .ToArray();

                return result;
            }
            catch (Exception e) {
                throw new FileWarehouse.StorageException(
                    StorageExceptionReason.StorageOperationFailed,
                    $"Unable to get the containers list. Mask: {mask}",
                    e);
            }
        }

        public async Task<IStorageContainer> CreateOrGetContainerAsync(string name, ContainerPermission permission, bool failIfExist)
        {
            CloudBlobContainer container = this._blobClient.GetContainerReference(name.ToLower());
            var isCreated = await container.CreateIfNotExistsAsync().ConfigureAwait(false);

            if (failIfExist && !isCreated)
                throw new FileWarehouse.StorageException(StorageExceptionReason.ContainerExistAlready, "Container exist already.");

            IStorageContainer storageContainer = new AzureBlobStorageContainer(container);
            await storageContainer.SetPermissionAsync(permission).ConfigureAwait(false);

            return storageContainer;
        }

        public async Task<IStorageContainer> GetContainerAsync(string name)
        {
            CloudBlobContainer container = this._blobClient.GetContainerReference(name.ToLower());

            bool containerExists = await container.ExistsAsync().ConfigureAwait(false);
            if (!containerExists)
                throw new FileWarehouse.StorageException(StorageExceptionReason.ContainerNotFound, $"Container '{name}' not exist.");

            IStorageContainer storageContainer = new AzureBlobStorageContainer(container);

            return storageContainer;
        }

        public async Task DeleteContainerAsync(string name)
        {
            CloudBlobContainer container = this._blobClient.GetContainerReference(name.ToLower());
            await container.DeleteIfExistsAsync().ConfigureAwait(false);
        }
    }
}