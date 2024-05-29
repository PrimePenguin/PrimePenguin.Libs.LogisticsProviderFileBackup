dotnet restore;
dotnet build -c Release;
dotnet pack --no-build -c Release PrimePenguin.Libs.LogisticsProviderFileBackup/PrimePenguin.Libs.LogisticsProviderFileBackup.csproj;

$nupkg = (Get-ChildItem PrimePenguin.Libs.LogisticsProviderFileBackup/bin/Release/*.nupkg)[0];

# Push the nuget package to AppVeyor's artifact list.
Push-AppveyorArtifact $nupkg.FullName -FileName $nupkg.Name -DeploymentName "PrimePenguin.Libs.LogisticsProviderFileBackup.nupkg"