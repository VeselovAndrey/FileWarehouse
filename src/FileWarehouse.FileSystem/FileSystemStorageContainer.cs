// -----------------------------------------------------------------------
// <copyright file="DesktopStorageContainer.cs">
// Copyright (c) 2013-2016 Andrey Veselov. All rights reserved.
// License: Apache License 2.0
// Contacts: http://andrey.moveax.com  andrey@moveax.com
// </copyright>
// -----------------------------------------------------------------------

namespace FileWarehouse.FileSystem
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    internal class FileSystemStorageContainer : IStorageContainer
    {
        private readonly string _urlBase;
        private readonly string _containerRoot;
        private readonly Task<bool> _successTask;

        public FileSystemStorageContainer(string containerRoot)
            : this(containerRoot, null)
        {
        }

        public FileSystemStorageContainer(string containerRoot, string urlBase)
        {
            Contract.Requires(!string.IsNullOrEmpty(containerRoot));

            var rootDirectory = containerRoot.Trim();
            this._containerRoot = rootDirectory.EndsWith(@"\") ? rootDirectory.TrimEnd('\\') : rootDirectory;

            this._urlBase = null;
            if (!string.IsNullOrEmpty(urlBase)) {
                var url = urlBase.Trim();
                this._urlBase = url.EndsWith(@"\") ? url.TrimEnd('\\') : url;
            }

            var tcs = new TaskCompletionSource<bool>();
            tcs.SetResult(true);
            this._successTask = tcs.Task;
        }

        #region Permissions

        public Task<ContainerPermission> GetPermissionAsync()
        {
            var tcs = new TaskCompletionSource<ContainerPermission>();
            tcs.SetResult(ContainerPermission.Private);

            return tcs.Task;
        }

        public Task SetPermissionAsync(ContainerPermission permission)
        {
            return this._successTask;
        }

        #endregion

        #region Directories methods

        public Task CreateDirectoryAsync(string path)
        {
            string dirFolder = $"{ this._containerRoot}\\{path.Trim()}";

            try {
                Directory.CreateDirectory(dirFolder);

                return this._successTask;
            }
            catch (Exception ex) {
                throw new StorageException(
                    StorageExceptionReason.StorageOperationFailed,
                    $"Error while creating the directory. {path}",
                    ex);
            }
        }

        public Task DeleteDirectoryAsync(string path)
        {
            string dirFolder = $"{ this._containerRoot}\\{path.Trim()}";

            try {
                Directory.Delete(dirFolder, true);

                return this._successTask;
            }
            catch (Exception ex) {
                throw new StorageException(
                    StorageExceptionReason.StorageOperationFailed,
                    $"Error while deleting the directory. {path}",
                    ex);
            }
        }

        public Task<IEnumerable<string>> GetDirectoriesAsync(string path)
        {
            string basePath = $"{this._containerRoot}\\{path.Trim('\\', ' ')}";

            IEnumerable<string> directories = Directory.EnumerateDirectories(basePath)
                .Select(Path.GetFileName)
                .Where(d => !string.IsNullOrEmpty(d))
                .ToArray();

            var tcs = new TaskCompletionSource<IEnumerable<string>>();
            tcs.SetResult(directories);
            return tcs.Task;
        }

        #endregion

        #region File operations

        public async Task WriteFileAsync(Stream source, string path)
        {
            string fileName = $"{this._containerRoot}\\{path.Trim()}";

            try {
                using (var fileStream = File.Create(fileName)) {
                    source.Seek(0, SeekOrigin.Begin);
                    await source.CopyToAsync(fileStream)
                        .ConfigureAwait(false);
                }
            }
            catch (Exception ex) {
                throw new StorageException(
                    StorageExceptionReason.StorageOperationFailed,
                    $"Error while writing the file. {path}",
                    ex);
            }
        }

        public Task<Stream> GetFileStreamAsync(string path)
        {
            string fileName = $"{this._containerRoot}\\{path.Trim()}";

            try {
                FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

                var tcs = new TaskCompletionSource<Stream>();
                tcs.SetResult(stream);
                return tcs.Task;
            }
            catch (Exception ex) {
                throw new StorageException(
                    StorageExceptionReason.StorageOperationFailed,
                    $"Error while reading the file. {path}",
                    ex);
            }
        }

        public async Task GetFileStreamAsync(Stream target, string path)
        {
            string fileName = $"{this._containerRoot}\\{path.Trim()}";

            try {
                using (FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    await stream.CopyToAsync(target)
                        .ConfigureAwait(false);
                }
            }
            catch (Exception ex) {
                throw new StorageException(
                    StorageExceptionReason.StorageOperationFailed,
                    $"Error while reading the file. {path}",
                    ex);
            }
        }

        public Task DeleteFileAsync(string path)
        {
            string fileName = $"{this._containerRoot}\\{path.Trim()}";

            try {
                File.Delete(fileName);

                return this._successTask;
            }
            catch (Exception ex) {
                throw new StorageException(
                    StorageExceptionReason.StorageOperationFailed,
                    $"Error while deleting the file. {path}",
                    ex);
            }
        }

        public Task<IEnumerable<string>> GetFilesAsync(string path, StorageSearchOption storageSearchOption)
        {
            string fileName = $"{this._containerRoot}\\{path.Trim()}";
            var containerRootLength = this._containerRoot.Length;
            SearchOption searchOption = storageSearchOption.HasFlag(StorageSearchOption.AllDirectories) ?
                SearchOption.AllDirectories :
                SearchOption.TopDirectoryOnly;

            try {
                IEnumerable<string> query = Directory.EnumerateFiles(fileName, "*.*", searchOption);

                query = storageSearchOption.HasFlag(StorageSearchOption.StripPaths) ?
                    query.Select(Path.GetFileName) :
                    query.Select(f => f.Substring(containerRootLength));

                IEnumerable<string> files = query.ToArray();

                var tcs = new TaskCompletionSource<IEnumerable<string>>();
                tcs.SetResult(files);
                return tcs.Task;
            }
            catch (Exception ex) {
                throw new StorageException(
                    StorageExceptionReason.StorageOperationFailed,
                    $"Error while getting files list. {path}",
                    ex);
            }
        }

        public string GetUrlForFile(string path)
        {
            if (string.IsNullOrEmpty(this._urlBase))
                throw new InvalidOperationException();

            return $"{this._urlBase}/{path.Trim().TrimStart('/')}";
        }

        #endregion
    }
}
