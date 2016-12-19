// -----------------------------------------------------------------------
// <copyright file="DesktopStorageContainerTests.cs">
// Copyright (c) 2013-2016 Andrey Veselov. All rights reserved.
// License: Apache License 2.0
// Contacts: http://andrey.moveax.com  andrey@moveax.com
// </copyright>
// -----------------------------------------------------------------------

namespace FileWarehouse.FileSystem.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    //[DeploymentItem("LocalStorage", "LocalStorage")]
    public class FileSystemStorageTests
    {
        private readonly string _storagePath;
        private const string _precreatedContainerName = "Container1";

        public FileSystemStorageTests()
        {
            // {AppDomain.CurrentDomain.BaseDirectory}\\
            this._storagePath = $"LocalStorage";
        }

        [Fact]
        public void FileSystemStorageStorageCreate()
        {
            // Arrange

            // Act
            IStorage storage = new FileSystemStorage(this._storagePath);

            // Assert
            Assert.NotNull(storage);
        }

        [Fact]
        public async Task DesktopStorageGetListOfContainers()
        {
            // Arrange
            IStorage storage = new FileSystemStorage(this._storagePath);

            // Act
            var list = await storage.GetContainersListAsync(null);

            // Assert
            Assert.NotNull(list);
            Assert.True(list.Any(l => string.Equals(FileSystemStorageTests._precreatedContainerName, l)));
        }

        [Fact]
        public async Task DesktopStorageGetExisingContainer()
        {
            // Arrange
            IStorage storage = new FileSystemStorage(this._storagePath);

            // Act
            var container = await storage.GetContainerAsync(FileSystemStorageTests._precreatedContainerName);

            // Assert
            Assert.NotNull(container);
        }

        [Fact]
        public async Task DesktopStorageGetNonExisingContainer()
        {
            // Arrange
            IStorage storage = new FileSystemStorage(this._storagePath);
            StorageException ex = null;

            // Act
            try {
                await storage.GetContainerAsync(FileSystemStorageTests._precreatedContainerName + "-NOT-EXISTS");
            }
            catch (StorageException e) {
                ex = e;
            }

            // Assert
            Assert.NotNull(ex);
            Assert.Equal(StorageExceptionReason.ContainerNotFound, ex.Reason);
        }

        [Fact]
        public async Task DesktopStorageCreateContainer()
        {
            // Arrange
            IStorage storage = new FileSystemStorage(this._storagePath);

            // Act
            var container = await storage.CreateOrGetContainerAsync("FileSystemStorageCreateContainer", ContainerPermission.Public, failIfExists: true);

            // Assert
            Assert.NotNull(container);
        }

        [Fact]
        public async Task DesktopStorageTryCreateContainer()
        {
            // Arrange
            IStorage storage = new FileSystemStorage(this._storagePath);
            StorageException ex = null;

            // Act
            try {
                await storage.CreateOrGetContainerAsync(FileSystemStorageTests._precreatedContainerName, ContainerPermission.Public, failIfExists: true);
            }
            catch (StorageException e) {
                ex = e;
            }

            // Assert
            Assert.NotNull(ex);
            Assert.Equal(StorageExceptionReason.ContainerExistAlready, ex.Reason);
        }

        [Fact]
        public async Task DesktopStorageTryToDeleteContainer()
        {
            // Arrange
            IStorage storage = new FileSystemStorage(this._storagePath);
            const string containerName = "DesktopStorageTryToDeleteContainer";

            await storage.CreateOrGetContainerAsync(containerName, ContainerPermission.Public, failIfExists: false);
            var storages1 = await storage.GetContainersListAsync(containerName);

            // Act

            await storage.DeleteContainerAsync(containerName);
            var storages2 = await storage.GetContainersListAsync(containerName);

            // Assert
            Assert.NotNull(storages1);
            Assert.NotNull(storages2);
            Assert.True(storages1.All(c => string.Equals(c, containerName)));
            Assert.False(storages2.Any());
        }
    }
}
