Copy-Item $(env:APPVEYOR_BUILD_FOLDER)\ProceduralVoxelMesh\bin\Release\ProceduralVoxelMesh.dll -Destination $(env:STAGING_DIR)\ReleaseContents\Plugins\ -Force 
Copy-Item $(env:APPVEYOR_BUILD_FOLDER)\ProceduralVoxelMeshEditor\bin\Release\ProceduralVoxelMeshEditor.dll -Destination $(env:STAGING_DIR)\ReleaseContents\Plugins\Editor\ -Recurse -Force 
Copy-Item $(env:APPVEYOR_BUILD_FOLDER)\Resources -Destination $(env:STAGING_DIR)\ReleaseContents\Resources\ -Recurse -Force 
Copy-Item $(env:APPVEYOR_BUILD_FOLDER)\LICENSE -Destination $(env:STAGING_DIR)\ReleaseContents\ -Force 