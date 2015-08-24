// -----------------------------------------------------------------------
// <copyright file="DesktopStorage.cs">
// Copyright (c) 2013-2015 Andrey Veselov. All rights reserved.
// License: Apache License 2.0
// Contacts: http://andrey.moveax.com  andrey@moveax.com
// </copyright>
// -----------------------------------------------------------------------

namespace FileWarehouse.FileSystem
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class FileSystemStorage : IStorage
    {
        private readonly string _rootDirectory;
        private readonly TaskCompletionSource<bool> _successTask;

        public FileSystemStorage(string basePath)
        {
            Contract.Requires(!string.IsNullOrEmpty(basePath));

            var rootDirectory = basePath.Trim();
            this._rootDirectory = rootDirectory.EndsWith(@"\") ? rootDirectory.TrimEnd('\\') : rootDirectory;

            this._successTask = new TaskCompletionSource<bool>();
            this._successTask.SetResult(true);
        }

        public Task<IEnumerable<string>> GetContainersListAsync(string mask)
        {
            bool isMaskEmpty = string.IsNullOrEmpty(mask);
            string containerMask = isMaskEmpty ? null : mask.Trim().ToLower();

            IEnumerable<string> query = Directory.EnumerateDirectories(this._rootDirectory)
                .Select(Path.GetFileName);

            query = isMaskEmpty ?
                query.Where(d => !string.IsNullOrEmpty(d)) :
                query.Where(d => !string.IsNullOrEmpty(d) && d.ToLower().Contains(containerMask));

            IEnumerable<string> containers = query.ToArray();

            var tcs = new TaskCompletionSource<IEnumerable<string>>();
            tcs.SetResult(containers);
            return tcs.Task;
        }

        public Task<IStorageContainer> CreateOrGetContainerAsync(string name, ContainerPermission permission, bool failIfExists)
        {
            string containerDir = $"{this._rootDirectory}\\{name}";

            var dirExists = Directory.Exists(containerDir);
            if (!dirExists)
                Directory.CreateDirectory(containerDir);
            else if (failIfExists)
                throw new StorageException(StorageExceptionReason.ContainerExistAlready, "Container exist already.");

            var container = new FileSystemStorageContainer(containerDir);

            var tcs = new TaskCompletionSource<IStorageContainer>();
            tcs.SetResult(container);
            return tcs.Task;
        }

        public Task<IStorageContainer> GetContainerAsync(string name)
        {
            string containerDir = $"{this._rootDirectory}\\{name}";
            var dirExists = Directory.Exists(containerDir);

            if (!dirExists)
                throw new StorageException(StorageExceptionReason.ContainerNotFound, $"Container '{name}' not exist.");

            var container = new FileSystemStorageContainer(containerDir);

            var tcs = new TaskCompletionSource<IStorageContainer>();
            tcs.SetResult(container);
            return tcs.Task;
        }

        public Task DeleteContainerAsync(string name)
        {
            string containerDir = $"{this._rootDirectory}\\{name}";

            Directory.Delete(containerDir, true);

            return this._successTask.Task;
        }
    }
}
