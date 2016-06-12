# Run unit test
Copy-Item $env:APPVEYOR_BUILD_FOLDER\ProceduralVoxelMesh\bin\Release\ProceduralVoxelMesh.* -Destination $env:APPVEYOR_BUILD_FOLDER\ProceduralVoxelMeshTest\bin\Release\ -Force
vstest.console $env:APPVEYOR_BUILD_FOLDER\ProceduralVoxelMeshTest\bin\Release\ProceduralVoxelMeshTest.dll /Settings:$env:APPVEYOR_BUILD_FOLDER\ProceduralVoxelMeshTest\test.runsettings /logger:Appveyor
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }
