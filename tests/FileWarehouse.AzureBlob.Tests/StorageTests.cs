// -----------------------------------------------------------------------
// <copyright file="StorageTests.cs">
// Copyright (c) 2013-2016 Andrey Veselov. All rights reserved.
// License: Apache License 2.0
// Contacts: http://andrey.moveax.com  andrey@moveax.com
// </copyright>
// -----------------------------------------------------------------------

namespace FileWarehouse.AzureBlob.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class StorageTests : IClassFixture<InitializeAndCleanupFixture>
    {
        private readonly InitializeAndCleanupFixture _fixture;

        public StorageTests(InitializeAndCleanupFixture fixture)
        {
            _fixture = fixture;

            fixture.TestEmulatorRunnig();
        }

        // ReSharper disable PossibleMultipleEnumeration

        [Fact]
        public async Task AzureStorageCreateAndDeleteContainer()
        {
            // Arrange
            await this._fixture.ContainersCleanup();

            const ContainerPermission permission = ContainerPermission.Public;
            IStorage storage = new AzureBlobStorage(StorageConsts.ConnectionString);
            bool exceptionCaught = false;

            // Act 
            var container1 = await storage.CreateOrGetContainerAsync(StorageConsts.Container1Name, permission, failIfExists: true);
            var container2 = await storage.GetContainerAsync(StorageConsts.Container1Name);
            IStorageContainer container3 = null;
            try {
                container3 = await storage.GetContainerAsync(StorageConsts.Container2Name);
            }
            catch (StorageException ex) {
                if (ex.Reason == StorageExceptionReason.ContainerNotFound)
                    exceptionCaught = true;
            }
            catch (AggregateException ex) { // async exception can be aggregate
                if (ex.InnerExceptions.Any(e => e is StorageException))
                    exceptionCaught = true;
            }

            var resultPermission1 = await container1.GetPermissionAsync();
            var resultPermission2 = await container2.GetPermissionAsync();

            var containersList1 = await storage.GetContainersListAsync(StorageConsts.ContainerNamesPrefix);
            var containersList2 = await storage.GetContainersListAsync(StorageConsts.Container2Name.Substring(0, 12));

            await storage.DeleteContainerAsync(StorageConsts.Container1Name);

            var containersList3 = await storage.GetContainersListAsync(StorageConsts.ContainerNamesPrefix);

            // Assert
            Assert.NotNull(container1);
            Assert.NotNull(container2);
            Assert.Null(container3);
            Assert.True(exceptionCaught);
            Assert.Equal(permission, resultPermission1);
            Assert.Equal(permission, resultPermission2);
            Assert.NotNull(containersList1);
            Assert.NotNull(containersList2);
            Assert.True(containersList1.Any());
            Assert.True(!containersList2.Any());
            Assert.Equal(1, containersList1.Count() - containersList3.Count());
        }

        [Fact]
        public async Task AzureStorageTryCreateContainerWithEmptyName()
        {
            // Arrange
            await this._fixture.ContainersCleanup();

            const ContainerPermission permission = ContainerPermission.Public;
            IStorage storage = new AzureBlobStorage(StorageConsts.ConnectionString);
            bool exceptionCaught = false;

            // Act 
            try {
                await storage.CreateOrGetContainerAsync(null, permission, failIfExists: true);
            }
            catch (Exception) {
                exceptionCaught = true;
            }

            // Arrange
            Assert.True(exceptionCaught);
        }

        [Fact]
        public async Task AzureStorageModifyContainerPermission()
        {
            // Arrange
            await this._fixture.ContainersCleanup();

            const ContainerPermission sourcePermission = ContainerPermission.Private;
            const ContainerPermission targetPermission = ContainerPermission.Public;

            IStorage storage = new AzureBlobStorage(StorageConsts.ConnectionString);
            var container = await storage.CreateOrGetContainerAsync(StorageConsts.Container1Name, sourcePermission, failIfExists: true);

            // Act 
            await container.SetPermissionAsync(targetPermission);
            var resultPermission = await container.GetPermissionAsync();

            // Assert
            Assert.Equal(targetPermission, resultPermission);
        }

        [Fact]
        public async Task AzureStorageCreateAndDeleteDirectory()
        {
            // Arrange 
            await this._fixture.ContainersCleanup();

            IStorage storage = new AzureBlobStorage(StorageConsts.ConnectionString);
            var container = await storage.CreateOrGetContainerAsync(StorageConsts.Container1Name, ContainerPermission.Private, failIfExists: true);

            // Act
            await container.CreateDirectoryAsync("TestDir");
            await container.DeleteDirectoryAsync("TestDir");
            var dirs = await container.GetDirectoriesAsync("");

            // Assert
            Assert.True(!dirs.Any());
        }

        [Fact]
        public async Task AzureStorageCreateAndDeleteFile()
        {
            // Arrange 
            await this._fixture.ContainersCleanup();

            byte[] sourceData = this.CreateDataArray(255);
            IStorage storage = new AzureBlobStorage(StorageConsts.ConnectionString);
            var container = await storage.CreateOrGetContainerAsync(StorageConsts.Container1Name, ContainerPermission.Private, failIfExists: true);

            // Act 
            await container.WriteFileAsync(new MemoryStream(sourceData), StorageConsts.File1Name);
            var files1 = await container.GetFilesAsync(@"/folder1/subfolder/", StorageSearchOption.StripPaths); // first slash should be trimmed.

            await container.DeleteFileAsync(@"folder1/subfolder/fileText1.dat");
            var files2 = await container.GetFilesAsync(@"folder1/subfolder/", StorageSearchOption.StripPaths);

            // Assert
            Assert.True(files1.Count() == 1);
            Assert.Equal("fileText1.dat", files1.First());
            Assert.True(!files2.Any());
        }

        [Fact]
        public async Task AzureStorageWriteAndReadFile()
        {
            // Arrange 
            await this._fixture.ContainersCleanup();

            byte[] sourceData = this.CreateDataArray(255);
            IStorage storage = new AzureBlobStorage(StorageConsts.ConnectionString);
            var container = await storage.CreateOrGetContainerAsync(StorageConsts.Container1Name, ContainerPermission.Private, failIfExists: true);

            // Act 
            await container.WriteFileAsync(new MemoryStream(sourceData), StorageConsts.File1Name);
            var dirs1 = await container.GetDirectoriesAsync(@"");
            var dirs2 = await container.GetDirectoriesAsync(@"folder1");
            var files = await container.GetFilesAsync(@"folder1/subfolder/", StorageSearchOption.StripPaths);
            var filesFull = await container.GetFilesAsync(@"folder1/subfolder/", StorageSearchOption.Default);

            var resultStream1 = await container.GetFileStreamAsync(StorageConsts.File1Name);
            var resultData1 = new byte[resultStream1.Length];
            resultStream1.Read(resultData1, 0, resultData1.Length);

            var resultStream2 = new MemoryStream();
            await container.GetFileStreamAsync(resultStream2, @"\folder1/subfolder/fileText1.dat");
            var resultData2 = new byte[resultStream1.Length];
            resultStream2.Read(resultData2, 0, resultData2.Length);

            // Assert
            Assert.True(resultData1.SequenceEqual(sourceData));
            Assert.True(resultData2.SequenceEqual(sourceData));
            Assert.True(dirs1.Count() == 1);
            Assert.Equal("folder1", dirs1.First());
            Assert.True(dirs2.Count() == 1);
            Assert.Equal("subfolder", dirs2.First());
            Assert.True(files.Count() == 1);
            Assert.Equal("fileText1.dat", files.First());
            Assert.Equal(StorageConsts.File1Name, filesFull.First());
        }

        [Fact]
        public async Task AzureStorageCheсkFileUrl()
        {
            // Arrange 
            await this._fixture.ContainersCleanup();

            byte[] sourceData = this.CreateDataArray(255);
            IStorage storage = new AzureBlobStorage(StorageConsts.ConnectionString);
            var container = await storage.CreateOrGetContainerAsync(StorageConsts.Container1Name, ContainerPermission.Private, failIfExists: true);

            // Act 
            await container.WriteFileAsync(new MemoryStream(sourceData), StorageConsts.File1Name);
            var url = container.GetUrlForFile(StorageConsts.File1Name);
            var uri = new Uri(url);

            // Assert
            Assert.True(url.StartsWith("http://") || url.StartsWith("https://"));
            Assert.True(url.EndsWith("folder1/subfolder/fileText1.dat"));
            Assert.True(StorageConsts.File1Name.Length + "http://".Length < url.Length);
            Assert.NotNull(uri);
        }

        private byte[] CreateDataArray(int length)
        {
            var array = new byte[length];
            var rnd = new Random(DateTime.Now.Millisecond);

            for (var i = 0; i < length; i++)
                array[i] = (byte)rnd.Next(255);

            return array;
        }

        // ReSharper restore PossibleMultipleEnumeration
    }
}