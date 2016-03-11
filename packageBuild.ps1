$destinationFolder = "$env:STAGING_DIR\ReleaseContents\Plugins\"
if (!(Test-Path -path $destinationFolder)) {New-Item $destinationFolder -Type Directory}
Copy-Item $env:APPVEYOR_BUILD_FOLDER\ProceduralVoxelMesh\bin\Release\ProceduralVoxelMesh.dll -Destination $destinationFolder -Force 

$destinationFolder = "$env:STAGING_DIR\ReleaseContents\Plugins\Editor\"
if (!(Test-Path -path $destinationFolder)) {New-Item $destinationFolder -Type Directory}
Copy-Item $env:APPVEYOR_BUILD_FOLDER\ProceduralVoxelMeshEditor\bin\Release\ProceduralVoxelMeshEditor.dll -Destination $destinationFolder -Recurse -Force 

$destinationFolder = "$env:STAGING_DIR\ReleaseContents\Resources\"
if (!(Test-Path -path $destinationFolder)) {New-Item $destinationFolder -Type Directory}
Copy-Item $env:APPVEYOR_BUILD_FOLDER\Resources -Destination $destinationFolder -Recurse -Force 

$destinationFolder = "$env:STAGING_DIR\ReleaseContents\"
if (!(Test-Path -path $destinationFolder)) {New-Item $destinationFolder -Type Directory}
Copy-Item $env:APPVEYOR_BUILD_FOLDER\LICENSE -Destination $destinationFolder -Force 
