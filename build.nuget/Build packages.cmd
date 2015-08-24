@echo off 
del release /F /Q
rmdir release
mkdir release
nuget pack FileWarehouse.nuspec -Symbols -OutputDirectory release
nuget pack FileWarehouse.Azure.Blob.nuspec -Symbols -OutputDirectory release
nuget pack FileWarehouse.FileSystem.nuspec -Symbols -OutputDirectory release
pause