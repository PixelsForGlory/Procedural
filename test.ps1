# Run unit test
Copy-Item $env:APPVEYOR_BUILD_FOLDER\Procedural\bin\Release\Procedural.* -Destination $env:APPVEYOR_BUILD_FOLDER\ProceduralTest\bin\Release\ -Force
vstest.console $env:APPVEYOR_BUILD_FOLDER\ProceduralTest\bin\Release\ProceduralTest.dll /Settings:$env:APPVEYOR_BUILD_FOLDER\ProceduralTest\test.runsettings /logger:Appveyor
if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }
