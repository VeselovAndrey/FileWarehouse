// -----------------------------------------------------------------------
// <copyright file="AssemblyInitializeAndCleanup.cs">
// Copyright (c) 2013-2015 Andrey Veselov. All rights reserved.
// License: Apache License 2.0
// Contacts: http://andrey.moveax.com  andrey@moveax.com
// </copyright>
// -----------------------------------------------------------------------

namespace FileWarehouse.Azure.Blob.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    public class InitializeAndCleanupFixture : IDisposable
    {
        public void Dispose()
        {
            this.ContainersCleanup().Wait();
        }

        public void TestEmulatorRunnig()
        {
            var count = Process.GetProcessesByName("AzureStorageEmulator").Length;

            if (count == 0)
                throw new Exception("Windows Azure Storage Emulator is not running.");
        }

        public async Task ContainersCleanup()
        {
            // Get blob client
            var storageAccount = CloudStorageAccount.Parse(StorageConsts.ConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();

            // Remove all test containers
            var testContainers = await this.ListContainers(blobClient, StorageConsts.ContainerNamesPrefix)
                .ConfigureAwait(false);

            foreach (var container in testContainers) {
                await container.DeleteIfExistsAsync()
                    .ConfigureAwait(false);
            }
        }

        private async Task<IEnumerable<CloudBlobContainer>> ListContainers(CloudBlobClient blobClient, string prefix)
        {
            BlobContinuationToken continuationToken = null;
            var containers = new List<CloudBlobContainer>();

            do {
                var response = await blobClient
                    .ListContainersSegmentedAsync(prefix, continuationToken)
                    .ConfigureAwait(false);

                containers.AddRange(response.Results);
                continuationToken = response.ContinuationToken;
            }
            while (continuationToken != null);

            return containers;
        }


    }
}
