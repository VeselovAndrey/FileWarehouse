# FileWarehouse
FileWarehouse allows you to work with files in different storages using common interface.

Build status: 
[![AppVeyor](https://ci.appveyor.com/api/projects/status/bbwsyoqaiy9gom22/branch/master?svg=true&passingText=branch:%20master%20-%20OK&failingText=branch:%20master%20-%20Failed&pendingText=branch:%20master%20-%20In%20progress)](https://ci.appveyor.com/project/VeselovAndrey/filewarehouse/branch/master)
[![AppVeyor](https://ci.appveyor.com/api/projects/status/bbwsyoqaiy9gom22/branch/dev?svg=true&passingText=branch:%20dev%20-%20OK&failingText=branch:%20dev%20-%20Failed&pendingText=branch:%20dev%20-%20In%20progress)](https://ci.appveyor.com/project/VeselovAndrey/filewarehouse/branch/dev)

## Supported storages

* File system
* Azure Blob storage

## Stable releases ##
[![NuGet - FileWarehouse](https://img.shields.io/nuget/v/FileWarehouse.svg?label=FileWarehouse&style=flat-square)](https://www.nuget.org/packages/FileWarehouse/)
[![NuGet - FileWarehouse.AzureBlob](https://img.shields.io/nuget/v/FileWarehouse.AzureBlob.svg?label=FileWarehouse.AzureBlob&style=flat-square)](https://www.nuget.org/packages/FileWarehouse.AzureBlob/)
[![NuGet - FileWarehouse.FileSystem](https://img.shields.io/nuget/v/FileWarehouse.FileSystem.svg?label=FileWarehouse.FileSystem&style=flat-square)](https://www.nuget.org/packages/FileWarehouse.FileSystem/)

## Latest releases ##

[![MyGet - FileWarehouse](https://img.shields.io/myget/filewarehouse/vpre/FileWarehouse.svg?label=FileWarehouse&style=flat-square)](https://www.myget.org/packages/FileWarehouse/)
[![MyGet - FileWarehouse.AzureBlob](https://img.shields.io/myget/filewarehouse/vpre/FileWarehouse.AzureBlob.svg?label=FileWarehouse.AzureBlob&style=flat-square)](https://www.myget.org/packages/FileWarehouse.AzureBlob/)
[![MyGet - FileWarehouse.FileSystem](https://img.shields.io/myget/filewarehouse/vpre/FileWarehouse.FileSystem.svg?label=FileWarehouse.FileSystem&style=flat-square)](https://www.myget.org/packages/FileWarehouse.FileSystem/)


## Installation

Use NuGet to install FileWarehouse common interfaces library to your project:
```
Install-Package FileWarehouse
```

The next step is to install required storage implementation. 
* Windows file storage:
```PS
Install-Package FileWarehouse.FileSystem
```
* Azure Blob storage:
```PS
Install-Package FileWarehouse.AzureBlob
```

## Examples

### Windows

```C#
// Connect to storage and get list of containers
IStorage storage = new FileSystemStorage("path to folder");

// Create new or get existing container
IStorageContainer container = await storage.CreateOrGetContainerAsync("MyContainer", ContainerPermission.Private, failIfExists: false);

// Write content to a file
using (var memStream = new MemoryStream()) {
    using (var writer = new StreamWriter(memStream)) {
        writer.Write(content);
        writer.Flush();
        memStream.Position = 0;
    }

    await container.WriteAsync(memStream, fileName);
}

// Read file content from the container
using (Stream stream = await container.ReadAsync(fileName)) {
    var buffer = new byte[stream.Length];
    await stream.ReadAsync(buffer, 0, (int)stream.Length);
    result = Encoding.UTF8.GetString(buffer);
}

// Delete a file
await container.DeleteAsync(fileName);

// Get files list
IEnumerable<string> files = await container.GetFilesAsync("", StorageSearchOption.Default);
```

#### Azure
```C#
// Connect to Azure storage ...
IStorage storage = new AzureBlobStorage("<connection string>");

// ... and use same code as above
```