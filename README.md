# FileWarehouse
FileWarehouse allows to work with file in different storages using common interface.

Build status: 
[![AppVeyor](https://ci.appveyor.com/api/projects/status/bbwsyoqaiy9gom22/branch/master?svg=true&passingText=branch:%20master%20-%20OK&failingText=branch:%20master%20-%20Failed&pendingText=branch:%20master%20-%20In%20progress)](https://ci.appveyor.com/project/VeselovAndrey/filewarehouse/branch/master)
[![AppVeyor](https://ci.appveyor.com/api/projects/status/bbwsyoqaiy9gom22/branch/dev?svg=true&passingText=branch:%20dev%20-%20OK&failingText=branch:%20dev%20-%20Failed&pendingText=branch:%20dev%20-%20In%20progress)](https://ci.appveyor.com/project/VeselovAndrey/filewarehouse/branch/dev)

## Supported storages

* File system
* Azure Blob storage

## Installation

You should use NuGet in order to install FileWarehouse to your project:
```
Install-Package FileWarehouse
```
This will install common interfaces.

You should install storage implementation too. 
* Windows file storage:
```PS
Install-Package FileWarehouse.FileSystem
```
* Azure Blob storage:
```PS
Install-Package FileWarehouse.Azure.Blob
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