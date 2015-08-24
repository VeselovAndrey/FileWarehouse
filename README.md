# FileWarehouse
FileWarehouse allows to work with file in different storages using common interface.

## Supported storages

* File system
* Azure Blob storage

## Installation

You should use NuGet in order to install FileWarehouse to your project:
```
Install-Package FileWarehouse
```
This will install common interfaces. You should install storage implementation too. 

For windows file storage use:
```
Install-Package FileWarehouse.FileSystem
```
For Azure Blob storage:
```
Install-Package FileWarehouse.Azure.Blob
```

## Examples

### Windows

```c#
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
```c#
// Connect to Azure storage ...
IStorage storage = new AzureBlobStorage("<connection string>");

// ... and use same code as above
```