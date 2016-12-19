// -----------------------------------------------------------------------
// <copyright file="IStorageContainer.cs">
// Copyright (c) 2013-2016 Andrey Veselov. All rights reserved.
// License: Apache License 2.0
// Contacts: http://andrey.moveax.com  andrey@moveax.com
// </copyright>
// -----------------------------------------------------------------------
 
namespace FileWarehouse
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using System.Diagnostics.Contracts;
    
    [ContractClass(typeof(IStorageContainerContract))]
    public interface IStorageContainer
    {
        /// <summary>Gets the permission asynchronously.</summary>
        /// <returns>The current container permissions.</returns>
        Task<ContainerPermission> GetPermissionAsync();

        /// <summary>Sets the permission for the container asynchronously.</summary>
        /// <param name="permission">The permission.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task SetPermissionAsync(ContainerPermission permission);

        /// <summary>Creates the directory asynchronously.</summary>
        /// <param name="path">A relative path and name for the new directory.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task CreateDirectoryAsync(string path);

        /// <summary>Deletes the directory asynchronously.</summary>
        /// <param name="path">A relative path and name of the directory.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task DeleteDirectoryAsync(string path);

        /// <summary>Gets the directories list asynchronously.</summary>
        /// <param name="path">A relative path and name for the  directory.</param>
        /// <returns>The task object representing the asynchronous operation. The Result property on the task object returns a list of directories.</returns>
        Task<IEnumerable<string>> GetDirectoriesAsync(string path);

        /// <summary>Writes the file asynchronously.</summary>
        /// <param name="source">The source stream.</param>
        /// <param name="path">The file to be opened for writing.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task WriteFileAsync(Stream source, string path);

        /// <summary>Reads the contents of the file into a stream asynchronous.</summary>
        /// <param name="path">The file to be opened for reading.</param>
        /// <returns>The task object representing the asynchronous operation. The Result property on the task object returns a file stream.</returns>
        Task<Stream> GetFileStreamAsync(string path);

        /// <summary>Reads the contents of the file into a stream asynchronous.</summary>
        /// <param name="target">The target stream.</param>
        /// <param name="path">The file to be opened for reading.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task GetFileStreamAsync(Stream target, string path);

        /// <summary>Delete the file asynchronously.</summary>
        /// <param name="path">The file to be deleted.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task DeleteFileAsync(string path);

        /// <summary>Returns all file names asynchronously. </summary>
        /// <param name="path">The directory to be scanned for files.</param>
        /// <param name="searchOption">Search options.</param>
        /// <returns>The task object representing the asynchronous operation. The Result property on the task object returns a list of files.</returns>
        Task<IEnumerable<string>> GetFilesAsync(string path, StorageSearchOption searchOption);

        /// <summary>Gets the file url.</summary>
        /// <param name="path">The file.</param>
        /// <returns>The absolute url for specified file. Client can use this url to download file from web.</returns>
        string GetUrlForFile(string path);
    }
}