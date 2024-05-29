# Introduction 
This small library is a file upload util used by logistic provider implementations
to upload backup files to the Prime Penguin cloud.  

# Building
You need to authenticate to the Prime Penguin DevOps "primepenguin-libs" artifact feed in order to build.

1. If you don't already have it installed, get the `artifacts-credprovider`: https://github.com/microsoft/artifacts-credprovider
2. Run `dotnet restore --interactive`

Then build normally.

# Publishing
Here's how to publish a new version:

1. `dotnet build --configuration Release`
2. Update the `Version` in the "PrimePenguin.Libs.LogisticsProviderFileBackup.csproj" file
3. `dotnet pack`
4. `dotnet nuget push --source "primepenguin-libs" --api-key az PrimePenguin.Libs.LogisticsProviderFileBackup/bin/Release/PrimePenguin.Libs.LogisticsProviderFileBackup.{Version}.nupkg`

NB: Don't forget to commit/push the version update!