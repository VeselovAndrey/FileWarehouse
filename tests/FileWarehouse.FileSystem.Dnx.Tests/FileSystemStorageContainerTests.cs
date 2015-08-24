// -----------------------------------------------------------------------
// <copyright file="DesktopStorageContainerTests.cs">
// Copyright (c) 2013-2015 Andrey Veselov. All rights reserved.
// License: Apache License 2.0
// Contacts: http://andrey.moveax.com  andrey@moveax.com
// </copyright>
// -----------------------------------------------------------------------

namespace FileWarehouse.FileSystem.Dnx.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using FileWarehouse.FileSystem;
    using Xunit;

    //[DeploymentItem("LocalStorage", "LocalStorage")]
    public class DesktopStorageContainerTests
    {
        private readonly string _storagePath;
        private const string _precreatedContainerName = "Container1";
        private const string _precreatedFolderName = "Folder1";

        public DesktopStorageContainerTests()
        {
            // {AppDomain.CurrentDomain.BaseDirectory}\\
            this._storagePath = $"LocalStorage";
        }

        [Fact]
        public async Task DesktopStorageGetDirectories()
        {
            // Arrange
            IStorage storage = new FileSystemStorage(this._storagePath);
            IStorageContainer container = await storage.GetContainerAsync(DesktopStorageContainerTests._precreatedContainerName);

            // Act
            IEnumerable<string> dirs = await container.GetDirectoriesAsync("");

            // Assert
            Assert.NotNull(dirs);
            Assert.True(dirs.Any(d => string.Equals(d, DesktopStorageContainerTests._precreatedFolderName, StringComparison.OrdinalIgnoreCase)));
        }
        
        [Fact]
        public async Task DesktopStorageContainerCreateDirectory()
        {
            // Arrange
            string dirName = "CreateDirectory";
            IStorage storage = new FileSystemStorage(this._storagePath);
            IStorageContainer container = await storage.GetContainerAsync(DesktopStorageContainerTests._precreatedContainerName);

            // Act
            IEnumerable<string> initialDirs = await container.GetDirectoriesAsync("");
            await container.CreateDirectoryAsync(dirName);
            IEnumerable<string> finalDirs = await container.GetDirectoriesAsync("");

            // Assert
            Assert.False(initialDirs.Any(d => string.Equals(d, dirName, StringComparison.OrdinalIgnoreCase)));
            Assert.True(finalDirs.Any(d => string.Equals(d, dirName, StringComparison.OrdinalIgnoreCase)));
        }

        [Fact]
        public async Task DesktopStorageReadFile()
        {
            // Arrange
            IStorage storage = new FileSystemStorage(this._storagePath);
            IStorageContainer container = await storage.GetContainerAsync(DesktopStorageContainerTests._precreatedContainerName);

            string result;

            // Act
            using (Stream stream = await container.GetFileStreamAsync("TestFile1.txt")) {
                var buffer = new byte[stream.Length];
                await stream.ReadAsync(buffer, 0, (int)stream.Length);
                result = Encoding.UTF8.GetString(buffer);
            }

            // Assert
            Assert.NotNull(result);
            Assert.Equal("﻿This is test file 1.", result);
        }

        [Fact]
        public async Task DesktopStorageWriteReadDeleteFile()
        {
            // Arrange
            IStorage storage = new FileSystemStorage(this._storagePath);
            IStorageContainer container = await storage.GetContainerAsync(DesktopStorageContainerTests._precreatedContainerName);
            const string text = "This is R/W test";
            const string fileName = "rw.txt";

            var memStream = new MemoryStream();
            var writer = new StreamWriter(memStream);
            writer.Write(text);
            writer.Flush();
            memStream.Position = 0;

            string result;

            // Act
            await container.WriteFileAsync(memStream, fileName);

            using (Stream stream = await container.GetFileStreamAsync(fileName)) {
                var buffer = new byte[stream.Length];
                await stream.ReadAsync(buffer, 0, (int)stream.Length);
                result = Encoding.UTF8.GetString(buffer);
            }

            await container.DeleteFileAsync(fileName);
            var files = await container.GetFilesAsync("", StorageSearchOption.Default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(text, result);
            Assert.False(files.Any(d => string.Equals(d, fileName, StringComparison.OrdinalIgnoreCase)));
        }

        [Fact]
        public async Task DesktopStorageGetFilesList()
        {
            // Arrange
            IStorage storage = new FileSystemStorage(this._storagePath);
            IStorageContainer container = await storage.GetContainerAsync(DesktopStorageContainerTests._precreatedContainerName);

            // Act
            var files = await container.GetFilesAsync("", StorageSearchOption.AllDirectories | StorageSearchOption.StripPaths);
            var filesFull = await container.GetFilesAsync("", StorageSearchOption.AllDirectories);

            // Assert
            Assert.NotNull(files);
            Assert.NotNull(filesFull);
            Assert.True(files.Any());
            Assert.True(filesFull.Any());
        }
    }
}