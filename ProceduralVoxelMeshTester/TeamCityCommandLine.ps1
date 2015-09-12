Write-Host "##teamcity[testStarted name='RenderTest']"

Copy-Item -Force %system.teamcity.build.workingDir%\ProceduralVoxelMesh\bin\Release\ProceduralVoxelMesh.dll %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\Assets\Plugins\

$testResult = Test-Path %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\Assets\Plugins\ProceduralVoxelMesh.dll

if($testResult -eq $false)
{
	Write-Host "##teamcity[testFailed name='RenderTest' message='Library not found' details='ProceduralVoxelMesh.dll file not found in the %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\Assets\ folder']"
}
else
{
    Start-Process -ArgumentList @("-batchmode","-projectpath %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\", "-executeMethod Assets.Test.StartTest") -Wait -NoNewWindow "C:\Program Files\Unity\Editor\Unity.exe"

    for($i = 0; $i -lt 6; $i++)
    {
        $testResult = Test-Path "%system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\$i.png"
        if($testResult -eq $false)
        {
	        Write-Host "##teamcity[testFailed name='RenderTest' message='Screenshot not found' details='%system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\$i.png file not found']"
        }
        else
        {
            fc.exe /b %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\$i.png %system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\original$i.png > $null
            if ($LASTEXITCODE -eq 0)
            {
                Write-Host "%system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\$i.png PASS"
            }
	        else
            {
                Write-Host "##teamcity[testFailed name='RenderTest' message='Screenshot did not match' details='%system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\$i.png did not match the original']"
            }
        }

        Remove-Item "%system.teamcity.build.workingDir%\ProceduralVoxelMeshTester\$i.png"
    }
}

Write-Host "##teamcity[testFinished name='RenderTest']"
