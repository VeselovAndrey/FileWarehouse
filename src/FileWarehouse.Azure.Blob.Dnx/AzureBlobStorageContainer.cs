// -----------------------------------------------------------------------
// <copyright file="AzureBlobStorageContainer.cs">
// Copyright (c) 2013-2015 Andrey Veselov. All rights reserved.
// License: Apache License 2.0
// Contacts: http://andrey.moveax.com  andrey@moveax.com
// </copyright>
// -----------------------------------------------------------------------
 
namespace FileWarehouse.Azure.Blob
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage.Blob;
    using System.Diagnostics.Contracts;

    internal class AzureBlobStorageContainer : IStorageContainer
    {
        private readonly CloudBlobContainer _blobContainer;

        private static readonly Task _successTask;

        static AzureBlobStorageContainer()
        {
            var tcs = new TaskCompletionSource<bool>();
            tcs.SetResult(true);

            AzureBlobStorageContainer._successTask = tcs.Task;
        }

        public AzureBlobStorageContainer(CloudBlobContainer blobContainer)
        {
            Contract.Requires(blobContainer != null);

            this._blobContainer = blobContainer;
        }

#region Permissions

        public async Task<ContainerPermission> GetPermissionAsync()
        {
            BlobContainerPermissions permissions;

            try {
                permissions = await this._blobContainer.GetPermissionsAsync().ConfigureAwait(false);
            }
            catch (Exception e) {
                throw new StorageException(StorageExceptionReason.StorageOperationFailed, "Unable to get permissions for the storage.", e);
            }

            return permissions.PublicAccess == BlobContainerPublicAccessType.Blob ?
                ContainerPermission.Public :
                ContainerPermission.Private;
        }

        public async Task SetPermissionAsync(ContainerPermission permission)
        {
            BlobContainerPublicAccessType accessType = permission.HasFlag(ContainerPermission.Public) ?
                BlobContainerPublicAccessType.Blob :
                BlobContainerPublicAccessType.Off;

            try {
                await this._blobContainer
                    .SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = accessType })
                    .ConfigureAwait(false);
            }
            catch (Exception e) {
                throw new StorageException(StorageExceptionReason.StorageOperationFailed, "Unable to set permissions for the storage.", e);
            }
        }

#endregion Permissions

#region Directories methods

        public Task CreateDirectoryAsync(string path)
        {
            // nothing to do for Azure Blob Storage
            return AzureBlobStorageContainer._successTask;
        }

        public async Task DeleteDirectoryAsync(string path)
        {
            CloudBlobDirectory blobDirectory = this._blobContainer.GetDirectoryReference(this.GetCorrectPath(path, true));

            try {
                var blobs = await this.ListBlobsAsync(blobDirectory, useFlatBlobListing: true)
                    .ConfigureAwait(false);
                    
                foreach (var blob in blobs) {
                    if (blob is CloudPageBlob)
                        await ((CloudPageBlob)blob).DeleteAsync().ConfigureAwait(false);
                    else if (blob is CloudBlockBlob)
                        await ((CloudBlockBlob)blob).DeleteAsync().ConfigureAwait(false);
                }
            }
            catch (Exception e) {
                throw new StorageException(
                    StorageExceptionReason.StorageOperationFailed,
                    $"Unable to delete the directory {path} from the storage.",
                    e);
            }
        }

        public async Task<IEnumerable<string>> GetDirectoriesAsync(string path)
        {
            try {
                IEnumerable<IListBlobItem> blobs = await this.ListBlobsAsync(this._blobContainer, this.GetCorrectPath(path, true), useFlatBlobListing: false);

                var items = blobs
                    .Where(item => item is CloudBlobDirectory)
                    .Select(item => item.Uri.Segments.Last().TrimEnd('/', '\\'))
                    .ToArray();

                return items;
            }
            catch (Exception e) {
                throw new StorageException(
                    StorageExceptionReason.StorageOperationFailed,
                    $"Unable to get the directories list. {path}",
                    e);
            }
        }

#endregion Directories methods

#region File operations

        public async Task WriteFileAsync(Stream source, string path)
        {
            CloudBlockBlob blockBlob = this._blobContainer.GetBlockBlobReference(this.GetCorrectPath(path, false));

            try {
                await blockBlob.UploadFromStreamAsync(source).ConfigureAwait(false);
            }
            catch (Exception e) {
                throw new StorageException(
                    StorageExceptionReason.StorageOperationFailed,
                    $"Error while writing the file. {path}",
                    e);
            }
        }

        public async Task<Stream> GetFileStreamAsync(string path)
        {
            CloudBlockBlob blockBlob = this._blobContainer.GetBlockBlobReference(this.GetCorrectPath(path, false));

            var ms = new MemoryStream();
            try {
                await blockBlob.DownloadToStreamAsync(ms).ConfigureAwait(false);
                ms.Position = 0;
            }
            catch (Exception e) {
                throw new StorageException(
                    StorageExceptionReason.StorageOperationFailed,
                    $"Error while reading the file. {path}",
                    e);
            }

            return ms;
        }

        public async Task GetFileStreamAsync(Stream target, string path)
        {
            CloudBlockBlob blockBlob = this._blobContainer.GetBlockBlobReference(this.GetCorrectPath(path, false));

            try {
                var pos = target.Position;
                await blockBlob.DownloadToStreamAsync(target).ConfigureAwait(false);
                target.Position = pos;
            }
            catch (Exception e) {
                throw new StorageException(
                    StorageExceptionReason.StorageOperationFailed,
                    $"Error while reading the file. {path}",
                    e);
            }
        }

        public async Task DeleteFileAsync(string path)
        {
            CloudBlockBlob blockBlob = this._blobContainer.GetBlockBlobReference(this.GetCorrectPath(path, false));

            try {
                await blockBlob.DeleteIfExistsAsync().ConfigureAwait(false);
            }
            catch (Exception e) {
                throw new StorageException(
                    StorageExceptionReason.StorageOperationFailed,
                    $"Error while deleting the file. {path}",
                    e);
            }
        }

        public async Task<IEnumerable<string>> GetFilesAsync(string path, StorageSearchOption searchOption)
        {
            try {
                // ReSharper disable RedundantCast
                var selector = searchOption.HasFlag(StorageSearchOption.StripPaths) ?
                    (Func<IListBlobItem, string>)(item => item.Uri.Segments.Last()) :
                    (Func<IListBlobItem, string>)(item => {
                        string url = item.Uri.ToString();
                        var containerUrlLenght = this._blobContainer.Uri.ToString().Length;
                        if (containerUrlLenght < url.Length)
                            url = url.Substring(containerUrlLenght + 1);

                        return url;
                    });

                // ReSharper restore RedundantCast

                var blobs = await this.ListBlobsAsync(this._blobContainer, this.GetCorrectPath(path, true), searchOption.HasFlag(StorageSearchOption.AllDirectories))
                    .ConfigureAwait(false);

                IEnumerable<string> files = blobs
                        .Where(item => !(item is CloudBlobDirectory))
                        .Select(selector)
                        .ToList();

                return files;
            }
            catch (Exception e) {
                throw new StorageException(
                    StorageExceptionReason.StorageOperationFailed,
                    $"Unable to get the files list. {path}",
                    e);
            }
        }

        public string GetUrlForFile(string path)
        {
            CloudBlockBlob blockBlob = this._blobContainer.GetBlockBlobReference(this.GetCorrectPath(path, false));

            try {
                return blockBlob.Uri.ToString();
            }
            catch (Exception e) {
                throw new StorageException(
                    StorageExceptionReason.StorageOperationFailed,
                    $"Error while getting the file URL. {path}",
                    e);
            }
        }

#endregion File operations

        private async Task<IEnumerable<IListBlobItem>> ListBlobsAsync(CloudBlobDirectory blobDirectory, bool useFlatBlobListing, BlobListingDetails blobListingDetails = BlobListingDetails.None)
        {
            BlobContinuationToken continuationToken = null;
            List<IListBlobItem> results = new List<IListBlobItem>();
            do {
                var response = await blobDirectory
                    .ListBlobsSegmentedAsync(useFlatBlobListing, blobListingDetails, null, continuationToken, null, null)
                    .ConfigureAwait(false);

                continuationToken = response.ContinuationToken;
                results.AddRange(response.Results);
            }
            while (continuationToken != null);

            return results;
        }

        private async Task<IEnumerable<IListBlobItem>> ListBlobsAsync(CloudBlobContainer blobContainer, string prefix, bool useFlatBlobListing, BlobListingDetails blobListingDetails = BlobListingDetails.None)
        {
            BlobContinuationToken continuationToken = null;
            List<IListBlobItem> results = new List<IListBlobItem>();
            do {
                var response = await blobContainer
                    .ListBlobsSegmentedAsync(prefix, useFlatBlobListing, blobListingDetails, null, continuationToken, null, null)
                    .ConfigureAwait(false);

                continuationToken = response.ContinuationToken;
                results.AddRange(response.Results);
            }
            while (continuationToken != null);

            return results;
        }

        private string GetCorrectPath(string source, bool isDirectory)
        {
            if (string.IsNullOrEmpty(source))
                return source;

            var path = source.Replace('\\', '/');

            if (path[0] == '/')
                path = path.TrimStart('/');

            if (isDirectory && path.Length > 1 && path[path.Length - 1] != '/')
                path = path + '/';

            return path;
        }
    }
}