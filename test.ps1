# Run unit test
Copy-Item $env:APPVEYOR_BUILD_FOLDER\ProceduralVoxelMesh\bin\Release\ProceduralVoxelMesh.* -Destination $env:APPVEYOR_BUILD_FOLDER\ProceduralVoxelMeshTest\bin\Release\ -Force
vstest.console $env:APPVEYOR_BUILD_FOLDER\ProceduralVoxelMeshTest\bin\Release\ProceduralVoxelMeshTest.dll /Settings:$env:APPVEYOR_BUILD_FOLDER\ProceduralVoxelMeshTest\test.runsettings /logger:Appveyor

# Run image compare
Copy-Item $env:APPVEYOR_BUILD_FOLDER\ProceduralVoxelMesh\bin\Release\ProceduralVoxelMesh.* -Destination $env:APPVEYOR_BUILD_FOLDER\ProceduralVoxelMeshTester\Assets\Plugins\ -Force
Copy-Item $env:APPVEYOR_BUILD_FOLDER\ProceduralVoxelMeshEditor\bin\Release\ProceduralVoxelMeshEditor.* -Destination $env:APPVEYOR_BUILD_FOLDER\ProceduralVoxelMeshTester\Assets\Plugins\Editor\ -Force

Invoke-Expression "& `"$env:APPVEYOR_BUILD_FOLDER\ProceduralVoxelMeshTester\CompareImages.ps1`""
